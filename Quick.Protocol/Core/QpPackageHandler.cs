using Microsoft.Extensions.Logging;
using Quick.Protocol.Packages;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.Core
{
    public abstract class QpPackageHandler
    {
        private static readonly ILogger logger = LogUtils.GetCurrentClassLogger();

        //接收缓存
        private byte[] recvBuffer;
        //发送缓存
        private byte[] sendBuffer;
        //发送缓存2，用于压缩、加密
        //private byte[] sendBuffer2;

        private Stream QpPackageHandler_Stream;
        private QpPackageHandlerOptions options;
        private DateTime lastSendPackageTime = DateTime.MinValue;

        /// <summary>
        /// 缓存大小，初始大小为1KB
        /// </summary>
        protected int BufferSize { get; set; } = 1 * 1024;

        protected void ChangeBufferSize(int bufferSize)
        {
            BufferSize = bufferSize;
            recvBuffer = new byte[bufferSize];
            sendBuffer = new byte[bufferSize];
        }
        /// <summary>
        /// 增加Tag属性，用于引用与处理器相关的对象
        /// </summary>
        public Object Tag { get; set; }
        public QpPackageHandler(QpPackageHandlerOptions options)
        {
            this.options = options;
            ChangeBufferSize(BufferSize);
            //sendBuffer2 = new byte[options.BufferSize];
        }

        protected void InitQpPackageHandler_Stream(Stream stream)
        {
            QpPackageHandler_Stream = stream;
        }

        /// <summary>
        /// 收到数据包事件
        /// </summary>
        public event EventHandler<IPackage> PackageReceived;

        /// <summary>
        /// 当读取出错时
        /// </summary>
        protected virtual void OnReadError(Exception exception)
        {
            InitQpPackageHandler_Stream(null);
        }

        protected virtual void OnPackageReceived(IPackage package)
        {
            if (package is null)
                return;
            //触发收到数据包事件
            PackageReceived?.Invoke(this, package);
        }

        public void SendPackage(IPackage package)
        {
            lock (this)
            {
                var stream = QpPackageHandler_Stream;
                if (stream == null)
                    throw new ArgumentNullException(nameof(QpPackageHandler_Stream));

                bool shouldLog = true;
                if (package is HeartBeatPackage && !LogUtils.LogHeartbeat)
                    shouldLog = false;
                if (!LogUtils.LogPackage)
                    shouldLog = false;
                if (shouldLog)
                    logger.LogTrace("[Send-Package]{0}", package.ToString());

                byte[] packageBuffer;
                var packageTotalLength = package.Output(sendBuffer, out packageBuffer);


                Task.Run(() =>
                {
                    try
                    {
                            //如果包缓存是发送缓存
                            if (packageBuffer == sendBuffer)
                        {
                            stream.Write(packageBuffer, 0, packageTotalLength);
                            stream.Flush();
                        }
                            //否则，拆分为多个包发送
                            else
                        {
                                //每个包内容的最大长度为对方缓存大小减5
                                var maxTakeLength = BufferSize - 5;
                            var currentIndex = 0;
                            while (currentIndex < packageTotalLength)
                            {
                                var restLength = packageTotalLength - currentIndex;
                                int takeLength = 0;
                                if (restLength >= maxTakeLength)
                                    takeLength = maxTakeLength;
                                else
                                    takeLength = restLength;

                                stream.Write(BitConverter.GetBytes(takeLength), 0, 4);
                                stream.WriteByte(SplitPackage.PACKAGE_TYPE);
                                stream.Write(packageBuffer, currentIndex, takeLength);
                                stream.Flush();
                                currentIndex += takeLength;
                            }
                        }
                    }
                    catch { }
                }).Wait(options.SendTimeout);
                lastSendPackageTime = DateTime.Now;
            }
        }

        private async Task<int> readData(Stream stream, byte[] buffer, int startIndex, int totalCount, CancellationToken cancellationToken)
        {
            if (totalCount > buffer.Length - startIndex)
                throw new IOException("要接收的数据大小超出了缓存的大小！");
            int ret;
            var count = 0;
            while (count < totalCount)
            {
                ret = 0;
                var readTask = stream.ReadAsync(buffer, count, totalCount - count, cancellationToken);
                ret = await await TaskUtils.TaskWait(readTask, options.ReceiveTimeout);
                if (readTask.IsCanceled || ret == 0)
                    break;
                if (ret < 0)
                    throw new IOException("从网络流中读取错误！");
                count += ret;
            }
            return count;
        }

        /// <summary>
        /// 读取一个数据包
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task<IPackage> ReadPackageAsync(CancellationToken token)
        {
            var stream = QpPackageHandler_Stream;
            if (stream == null)
                throw new ArgumentNullException(nameof(QpPackageHandler_Stream));

            byte[] tmpBuffer = null;
            byte tmpPackageType = 0;
            int tmpPackageBodyLength = 0;

            //是否正在读取拆分包
            bool isReadingSplitPackage = false;
            int splitMsCapacity = 0;
            MemoryStream splitMs = null;
            while (true)
            {
                int ret = 0;
                //读取包头
                ret = await readData(stream, recvBuffer, 0, 5, token);
                if (token.IsCancellationRequested)
                    return null;
                if (ret < 5)
                    throw new IOException($"包头读取错误！读取数据长度：{ret}");

                var packageBodyLength = BitConverter.ToInt32(recvBuffer, 0);
                var packageTotalLength = packageBodyLength + 5;
                if (packageTotalLength > recvBuffer.Length)
                    throw new IOException($"数据包总长度[{packageTotalLength}]大于缓存大小[{recvBuffer.Length}]");

                var packageType = recvBuffer[4];
                //读取包体
                ret = await readData(stream, recvBuffer, 0, packageBodyLength, token);
                if (token.IsCancellationRequested)
                    return null;
                if (ret < packageBodyLength)
                    throw new IOException($"包体读取错误！包体长度：{packageBodyLength}，包类型：{packageType}，读取数据长度：{ret}");

                //如果当前包是拆分包
                if (packageType == SplitPackage.PACKAGE_TYPE)
                {
                    if (!isReadingSplitPackage)
                    {
                        tmpPackageBodyLength = BitConverter.ToInt32(recvBuffer, 0);
                        tmpPackageType = recvBuffer[4];

                        splitMsCapacity = tmpPackageBodyLength;
                        if (splitMsCapacity > options.MaxPackageSize)
                            throw new IOException($"拆分包中包长度[{splitMsCapacity}]大于最大包大小[{options.MaxPackageSize}]");
                        splitMs = new MemoryStream(splitMsCapacity);
                        isReadingSplitPackage = true;
                        //包头的5个字节就不写入了
                        splitMs.Write(recvBuffer, 5, packageBodyLength - 5);
                    }
                    else
                    {
                        splitMs.Write(recvBuffer, 0, packageBodyLength);
                    }
                    //如果拆分包已经读取完成
                    if (splitMs.Position >= splitMsCapacity)
                    {
                        tmpBuffer = splitMs.ToArray();
                        isReadingSplitPackage = false;
                        splitMs.Dispose();
                        break;
                    }
                }
                //如果是正在读取拆分包
                if (isReadingSplitPackage)
                    continue;

                tmpBuffer = recvBuffer;
                tmpPackageType = packageType;
                tmpPackageBodyLength = packageBodyLength;
                break;
            }

            //解析包
            var package = options.ParsePackage(tmpPackageType, tmpBuffer, 0, tmpPackageBodyLength);
            if (package == null)
                logger.LogWarning("[Recv-Package][UnknownPackageType]PackageBodyLength:{0} PackageType:{1}", tmpPackageBodyLength, tmpPackageType);
            else
            {
                bool shouldLog = true;
                if (package is HeartBeatPackage && !LogUtils.LogHeartbeat)
                    shouldLog = false;
                if (!LogUtils.LogPackage)
                    shouldLog = false;
                if (shouldLog)
                    logger.LogTrace("[Recv-Package]PackageLength:{0} Package:{1}", tmpPackageBodyLength, package.ToString());
            }
            return package;
        }

        protected void BeginHeartBeat(CancellationToken cancellationToken)
        {
            if (QpPackageHandler_Stream == null)
                return;

            if (options.HeartBeatInterval > 0)
                Task.Delay(options.HeartBeatInterval, cancellationToken).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return;
                    if (QpPackageHandler_Stream == null)
                        return;

                    var lastSendPackageToNowSeconds = (DateTime.Now - lastSendPackageTime).TotalMilliseconds;

                    //如果离最后一次发送数据包的时间大于心跳间隔，则发送心跳包
                    if (lastSendPackageToNowSeconds > options.HeartBeatInterval)
                    {
                        SendPackage(HeartBeatPackage.Instance);
                    }
                    BeginHeartBeat(cancellationToken);
                });
        }

        protected void BeginReadPackage(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            var readPackageTask = ReadPackageAsync(token);
            readPackageTask.ContinueWith(t =>
             {
                 if (QpPackageHandler_Stream == null)
                     return;
                 //如果已经取消
                 if (t.IsCanceled || token.IsCancellationRequested)
                 {
                     OnReadError(t.Exception.InnerException);
                     return;
                 }

                 //如果读取出错
                 if (t.IsFaulted)
                 {
                     logger.LogTrace("[ReadError]{0}", t.Exception.InnerException.Message);
                     try
                     {
                         if (QpPackageHandler_Stream.CanWrite)
                             SendPackage(new CommandResponsePackage()
                             {
                                 Code = -1,
                                 Message = t.Exception.InnerException.Message
                             });
                     }
                     catch { }
                     OnReadError(t.Exception.InnerException);
                     return;
                 }
                 //读取下一个数据包
                 BeginReadPackage(token);
                 var package = t.Result;
                 OnPackageReceived(package);
             });
        }
    }
}
