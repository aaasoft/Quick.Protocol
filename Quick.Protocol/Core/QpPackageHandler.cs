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
        /// 对方缓存大小
        /// </summary>
        protected int OppositeBufferSize { get; set; } = 0;

        /// <summary>
        /// 增加Tag属性，用于引用与处理器相关的对象
        /// </summary>
        public Object Tag { get; set; }
        public QpPackageHandler(QpPackageHandlerOptions options)
        {
            this.options = options;
            recvBuffer = new byte[options.BufferSize];
            sendBuffer = new byte[options.BufferSize];

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

                //如果不压缩也不加密
                if (!options.Compress && !options.Encrypt)
                {
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
                                SplitPackage[] packages = SplitPackage.Split(packageBuffer, packageTotalLength, OppositeBufferSize);
                                foreach (var pakcage in packages)
                                {
                                    byte[] outBuffer;
                                    var ret = package.Output(sendBuffer, out outBuffer);
                                    stream.Write(outBuffer, 0, ret);
                                    stream.Flush();
                                }
                            }
                        }
                        catch { }
                    }).Wait(options.SendTimeout);
                    return;
                }

                byte[] tmpBuffer = packageBuffer;
                int bodyLength = packageBuffer.Length - 5;

                ////压缩
                //if (options.Compress)
                //{
                //    using (var sourceStream = new MemoryStream(tmpBuffer, 5, tmpBuffer.Length - 5))
                //    using (var compressStream = new MemoryStream())
                //    {
                //        compressStream.Write(tmpBuffer, 0, 5);
                //        using (var gzStream = new GZipStream(compressStream, CompressionMode.Compress))
                //        {
                //            sourceStream.CopyTo(gzStream);
                //        }
                //        tmpBuffer = compressStream.ToArray();
                //        bodyLength = tmpBuffer.Length - 5;
                //    }
                //}
                ////加密
                //if (options.Encrypt)
                //{
                //    using (var ms = new MemoryStream(tmpBuffer.Length))
                //    {
                //        ms.Write(tmpBuffer, 0, 5);
                //        var encryptBuffer = CryptographyUtils.DesEncrypt(options.Password, tmpBuffer, 5, bodyLength);
                //        ms.Write(encryptBuffer, 0, encryptBuffer.Length);
                //        tmpBuffer = ms.ToArray();
                //        bodyLength = encryptBuffer.Length;
                //    }
                //}
                if (bodyLength > options.BufferSize)
                    throw new IOException("要发送的数据大小超出了缓存的大小！");

                var packageLengthBytes = BitConverter.GetBytes(bodyLength);
                packageLengthBytes.CopyTo(tmpBuffer, 0);
                tmpBuffer[4] = packageBuffer[4];

                Task.Run(() =>
                {
                    try
                    {
                        stream.Write(tmpBuffer, 0, bodyLength + 5);
                        stream.Flush();
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

            int ret = 0;
            //读取包头
            ret = await readData(stream, recvBuffer, 0, 5, token);
            if (token.IsCancellationRequested)
                return null;
            if (ret < 5)
                throw new IOException($"包头读取错误！读取数据长度：{ret}");

            var packageLength = BitConverter.ToInt32(recvBuffer, 0);
            if (packageLength > recvBuffer.Length)
                throw new IOException($"数据包长度：{packageLength}，缓存大小：{recvBuffer.Length}");
            var packageType = recvBuffer[4];
            //读取包体
            ret = await readData(stream, recvBuffer, 0, packageLength, token);
            if (token.IsCancellationRequested)
                return null;
            if (ret < packageLength)
                throw new IOException($"包体读取错误！包长度：{packageLength}，包类型：{packageType}，读取数据长度：{ret}");

            var tmpBuffer = recvBuffer;
            var tmpPackageLength = packageLength;

            ////解密
            //if (options.Encrypt)
            //{
            //    var decrptyBuffer = CryptographyUtils.DesDecrypt(options.Password, tmpBuffer, 0, tmpPackageLength);
            //    decrptyBuffer.CopyTo(recvBuffer, 0);
            //    tmpBuffer = sendBuffer;
            //    tmpPackageLength = decrptyBuffer.Length;
            //}
            ////解压
            //if (options.Compress)
            //{
            //    using (var mStream = new MemoryStream(sendBuffer2))
            //    using (var gzStream = new GZipStream(new MemoryStream(tmpBuffer, 0, tmpPackageLength), CompressionMode.Decompress))
            //    {
            //        gzStream.CopyTo(mStream);
            //        tmpBuffer = sendBuffer2;
            //        tmpPackageLength = (int)mStream.Position;
            //    }
            //}
            //解析包
            var package = options.ParsePackage(packageType, tmpBuffer, 0, tmpPackageLength);
            if (package == null)
                logger.LogWarning("[Recv-Package][UnknownPackageType]PackageLength:{0} PackageType:{1}", packageLength, packageType);
            else
            {
                bool shouldLog = true;
                if (package is HeartBeatPackage && !LogUtils.LogHeartbeat)
                    shouldLog = false;
                if (!LogUtils.LogPackage)
                    shouldLog = false;
                if (shouldLog)
                    logger.LogTrace("[Recv-Package]PackageLength:{0} Package:{1}", packageLength, package.ToString());
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
