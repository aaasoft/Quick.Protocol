using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public abstract class QpPackageHandler
    {
        /// <summary>
        /// 包头长度
        /// </summary>
        public const int PACKAGE_HEAD_LENGTH = 5;

        private static readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        /// <summary>
        /// 心跳包
        /// </summary>
        private static byte[] HEARTBEAT_PACKAGHE = new byte[] { 0, 0, 0, 5, 0 };
        private static ArraySegment<byte> nullArraySegment = new ArraySegment<byte>();
        //接收缓存
        private byte[] recvBuffer;
        private byte[] recvBuffer2;
        //发送缓存
        private byte[] sendBuffer;
        private byte[] sendBuffer2;

        private Stream QpPackageHandler_Stream;
        private QpPackageHandlerOptions options;
        private DateTime lastSendPackageTime = DateTime.MinValue;

        private DES des = DES.Create();
        private byte[] passwordMd5Buffer;
        private ICryptoTransform enc;
        private ICryptoTransform dec;
        private JsonSerializer jsonSerializer = new JsonSerializer();
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 缓存大小，初始大小为1KB
        /// </summary>
        protected int BufferSize { get; set; } = 1 * 1024;

        protected void ChangeBufferSize(int bufferSize)
        {
            //缓存大小最小为1KB
            if (bufferSize < 1 * 1024)
                bufferSize = 1 * 1024;
            BufferSize = bufferSize;
            recvBuffer = new byte[bufferSize];
            recvBuffer2 = new byte[bufferSize];
            sendBuffer = new byte[bufferSize];
            sendBuffer2 = new byte[bufferSize];
        }

        protected void ChangeTransportTimeout()
        {
            var stream = QpPackageHandler_Stream;
            if (stream != null && stream.CanTimeout)
            {
                stream.WriteTimeout = options.InternalTransportTimeout;
                stream.ReadTimeout = options.InternalTransportTimeout;
            }
        }

        /// <summary>
        /// 增加Tag属性，用于引用与处理器相关的对象
        /// </summary>
        public Object Tag { get; set; }

        private Dictionary<string, Type> noticeTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, Type> commandRequestTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, Type> commandResponseTypeDict = new Dictionary<string, Type>();
        public QpPackageHandler(QpPackageHandlerOptions options)
        {
            this.options = options;
            ChangeBufferSize(BufferSize);
            passwordMd5Buffer = CryptographyUtils.ComputeMD5Hash(Encoding.UTF8.GetBytes(options.Password)).Take(8).ToArray();
            enc = des.CreateEncryptor(passwordMd5Buffer, passwordMd5Buffer);
            dec = des.CreateDecryptor(passwordMd5Buffer, passwordMd5Buffer);
            foreach (var instructionSet in options.InstructionSet)
            {
                //添加通知数据包信息
                if (instructionSet.NoticeInfos != null && instructionSet.NoticeInfos.Length > 0)
                {
                    foreach (var item in instructionSet.NoticeInfos)
                    {
                        noticeTypeDict[item.NoticeTypeName] = item.NoticeType;
                    }
                }
                //添加命令数据包信息
                if (instructionSet.CommandInfos != null && instructionSet.CommandInfos.Length > 0)
                {
                    foreach (var item in instructionSet.CommandInfos)
                    {
                        commandRequestTypeDict[item.RequestTypeName] = item.RequestType;
                        commandResponseTypeDict[item.ResponseTypeName] = item.ResponseType;
                    }
                }
            }
        }

        protected void InitQpPackageHandler_Stream(Stream stream)
        {
            QpPackageHandler_Stream = stream;
            ChangeTransportTimeout();
        }

        /// <summary>
        /// 当读取出错时
        /// </summary>
        protected virtual void OnReadError(Exception exception)
        {
            logger.LogTrace("[ReadError]{0}: {1}", DateTime.Now, ExceptionUtils.GetExceptionMessage(exception));
            InitQpPackageHandler_Stream(null);
        }

        private void sendPackage(Func<byte[], Tuple<int, byte[]>> getPackagePayloadFunc)
        {
            var stream = QpPackageHandler_Stream;
            if (stream == null)
                return;

            lock (this)
            {
                var ret = getPackagePayloadFunc(sendBuffer);
                var packageTotalLength = ret.Item1;
                byte[] packageBuffer = ret.Item2;

                try
                {
                    //如果包缓存是发送缓存
                    if (packageBuffer == sendBuffer)
                    {
                        sendPackageBuffer(stream,
                            new ArraySegment<byte>(packageBuffer, 0, packageTotalLength)
                            );
                    }
                    //否则，拆分为多个包发送
                    else
                    {
                        //每个包内容的最大长度为对方缓存大小减包头大小
                        var maxTakeLength = BufferSize - PACKAGE_HEAD_LENGTH;
                        var currentIndex = 0;
                        while (currentIndex < packageTotalLength)
                        {
                            var restLength = packageTotalLength - currentIndex;
                            int takeLength = 0;
                            if (restLength >= maxTakeLength)
                                takeLength = maxTakeLength;
                            else
                                takeLength = restLength;
                            //构造包头
                            BitConverter.GetBytes(takeLength).CopyTo(sendBuffer, 0);
                            //如果是小端字节序，则交换
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(sendBuffer, 0, sizeof(int));
                            sendBuffer[4] = (byte)QpPackageType.Split;
                            //复制包体
                            Array.Copy(packageBuffer, currentIndex, sendBuffer, PACKAGE_HEAD_LENGTH, takeLength);
                            //发送
                            sendPackageBuffer(
                                stream,
                                new ArraySegment<byte>(sendBuffer, 0, PACKAGE_HEAD_LENGTH + takeLength)
                                );
                            currentIndex += takeLength;
                        }
                    }
                    lastSendPackageTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    logger.LogError("[SendPackage]" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        public void SendHeartbeatPackage()
        {
            sendPackage(buffer =>
            {
                HEARTBEAT_PACKAGHE.CopyTo(buffer, 0);
                return new Tuple<int, byte[]>(HEARTBEAT_PACKAGHE.Length, buffer);
            });
        }

        /// <summary>
        /// 发送通知包
        /// </summary>
        public void SendNoticePackage(object package)
        {
            sendPackage(buffer =>
            {
                //设置包类型
                buffer[PACKAGE_HEAD_LENGTH - 1] = (byte)QpPackageType.Notice;
                var typeName = package.GetType().FullName;
                //写入类名
                var typeNameByteLength = encoding.GetEncoder().GetBytes(typeName.ToCharArray(), 0, typeName.Length, buffer, PACKAGE_HEAD_LENGTH + 1, true);
                //写入类名长度
                buffer[PACKAGE_HEAD_LENGTH] = Convert.ToByte(typeNameByteLength);

                var offset = PACKAGE_HEAD_LENGTH + 1 + typeNameByteLength;
                int jsonContentLength = 0;
                using (var ms = new MemoryStream(buffer, offset, buffer.Length - offset))
                {
                    using (var writer = new StreamWriter(ms, encoding))
                        jsonSerializer.Serialize(writer, package);
                    jsonContentLength = Convert.ToInt32(ms.Position);
                }
                //包总长度
                var packageTotalLength = PACKAGE_HEAD_LENGTH + 1 + typeNameByteLength + jsonContentLength;
                return new Tuple<int, byte[]>(packageTotalLength, buffer);
            });
        }

        /// <summary>
        /// 发送命令请求包
        /// </summary>
        public void SendCommandRequestPackage(object package)
        {

        }

        /// <summary>
        /// 发送命令响应包
        /// </summary>
        public void SendCommandResponsePackage(object package)
        {

        }

        //public void SendPackage(IPackage package)
        //{
        //    var stream = QpPackageHandler_Stream;
        //    if (stream == null)
        //        return;
        //    bool shouldLog = true;
        //    if (package is HeartBeatPackage && !LogUtils.LogHeartbeat)
        //        shouldLog = false;
        //    if (!LogUtils.LogPackage)
        //        shouldLog = false;

        //    lock (this)
        //    {
        //        if (shouldLog)
        //            logger.LogTrace("[Send-Package]{0}: {1}", DateTime.Now, package);

        //        byte[] packageBuffer;
        //        var packageTotalLength = package.Output(sendBuffer, out packageBuffer);

        //        try
        //        {
        //            //如果包缓存是发送缓存
        //            if (packageBuffer == sendBuffer)
        //            {
        //                sendPackageBuffer(stream,
        //                    new ArraySegment<byte>(packageBuffer, 0, packageTotalLength)
        //                    );
        //            }
        //            //否则，拆分为多个包发送
        //            else
        //            {
        //                //每个包内容的最大长度为对方缓存大小减5
        //                var maxTakeLength = BufferSize - 5;
        //                var currentIndex = 0;
        //                while (currentIndex < packageTotalLength)
        //                {
        //                    var restLength = packageTotalLength - currentIndex;
        //                    int takeLength = 0;
        //                    if (restLength >= maxTakeLength)
        //                        takeLength = maxTakeLength;
        //                    else
        //                        takeLength = restLength;
        //                    //构造包头
        //                    BitConverter.GetBytes(takeLength).CopyTo(sendBuffer, 0);
        //                    sendBuffer[4] = SplitPackage.PACKAGE_TYPE;
        //                    //复制包体
        //                    Array.Copy(packageBuffer, currentIndex, sendBuffer, 5, takeLength);
        //                    //发送
        //                    sendPackageBuffer(
        //                        stream,
        //                        new ArraySegment<byte>(sendBuffer, 0, 5 + takeLength)
        //                        );
        //                    currentIndex += takeLength;
        //                }
        //            }
        //            lastSendPackageTime = DateTime.Now;
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogError("[SendPackage]" + ex.ToString());
        //        }
        //    }
        //}

        //获取空闲的缓存
        private byte[] getFreeBuffer(byte[] usingBuffer, params byte[][] bufferArray)
        {
            foreach (var buffer in bufferArray)
            {
                if (usingBuffer != buffer)
                    return buffer;
            }
            return null;
        }

        private void sendPackageBuffer(Stream stream, ArraySegment<byte> packageBuffer)
        {
            //如果压缩或者加密
            if (options.InternalCompress || options.InternalEncrypt)
            {
                //如果压缩
                if (options.InternalCompress)
                {
                    var currentBuffer = getFreeBuffer(packageBuffer.Array, sendBuffer, sendBuffer2);
                    using (var ms = new MemoryStream(currentBuffer))
                    {
                        using (var gzStream = new GZipStream(ms, CompressionMode.Compress, true))
                            gzStream.Write(packageBuffer.Array, packageBuffer.Offset, packageBuffer.Count);
                        packageBuffer = new ArraySegment<byte>(currentBuffer, 0, Convert.ToInt32(ms.Position));
                    }
                }
                //如果加密
                if (options.InternalEncrypt)
                {
                    var currentBuffer = enc.TransformFinalBlock(packageBuffer.Array, packageBuffer.Offset, packageBuffer.Count);
                    packageBuffer = new ArraySegment<byte>(currentBuffer, 0, currentBuffer.Length);
                }
                //发送包头(加密包伪装成心跳包)
                stream.Write(BitConverter.GetBytes(packageBuffer.Count), 0, 4);
                stream.WriteByte(1);
            }
            //发送包内容
            stream.Write(packageBuffer.Array, packageBuffer.Offset, packageBuffer.Count);
            stream.Flush();
        }

        private async Task<int> readData(Stream stream, byte[] buffer, int startIndex, int totalCount, CancellationToken cancellationToken)
        {
            if (totalCount > buffer.Length - startIndex)
                throw new IOException($"要接收的数据大小[{totalCount}]超出了缓存的大小[{buffer.Length - startIndex}]！");
            int ret;
            var count = 0;
            while (count < totalCount)
            {
                var readTask = stream.ReadAsync(buffer, count + startIndex, totalCount - count, cancellationToken);
                ret = await await TaskUtils.TaskWait(readTask, options.InternalTransportTimeout);
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
        protected async Task<ArraySegment<byte>> ReadPackageAsync(CancellationToken token)
        {
            var stream = QpPackageHandler_Stream;
            if (stream == null)
                throw new ArgumentNullException(nameof(QpPackageHandler_Stream));

            //最终包缓存
            ArraySegment<byte> finalPackageBuffer;

            //是否正在读取拆分包
            bool isReadingSplitPackage = false;
            int splitMsCapacity = 0;
            MemoryStream splitMs = null;
            while (true)
            {
                //读取包头
                var ret = await readData(stream, recvBuffer, 0, PACKAGE_HEAD_LENGTH, token);
                if (token.IsCancellationRequested)
                    return nullArraySegment;

                if (ret < PACKAGE_HEAD_LENGTH)
                    throw new IOException($"包头读取错误！包头长度：{PACKAGE_HEAD_LENGTH}，读取数据长度：{ret}");

                //包总长度
                var packageTotalLength = ByteUtils.B2I_BE(recvBuffer, 0);
                if (packageTotalLength < PACKAGE_HEAD_LENGTH)
                    throw new IOException($"包长度[{packageTotalLength}]必须大于等于{PACKAGE_HEAD_LENGTH}！");
                if (packageTotalLength > recvBuffer.Length)
                    throw new IOException($"数据包总长度[{packageTotalLength}]大于缓存大小[{recvBuffer.Length}]");

                //包体长度
                var packageBodyLength = packageTotalLength - PACKAGE_HEAD_LENGTH;
                //读取包体
                ret = await readData(stream, recvBuffer, PACKAGE_HEAD_LENGTH, packageBodyLength, token);
                if (token.IsCancellationRequested)
                    return nullArraySegment;
                if (ret < packageBodyLength)
                    throw new IOException($"包体读取错误！包体长度：{packageBodyLength}，读取数据长度：{ret}");

                var currentPackageBuffer = new ArraySegment<byte>(recvBuffer, 0, packageTotalLength);

                //如果设置了压缩或者加密
                if (options.InternalCompress || options.InternalEncrypt)
                {
                    currentPackageBuffer = new ArraySegment<byte>(recvBuffer, PACKAGE_HEAD_LENGTH, packageBodyLength);

                    //如果设置了加密，则先解密
                    if (options.InternalEncrypt)
                    {
                        var currentBuffer = dec.TransformFinalBlock(currentPackageBuffer.Array, currentPackageBuffer.Offset, currentPackageBuffer.Count);
                        currentPackageBuffer = new ArraySegment<byte>(currentBuffer, 0, currentBuffer.Length);
                    }
                    //如果设置了压缩，则先解压
                    if (options.InternalCompress)
                    {
                        var currentBuffer = getFreeBuffer(currentPackageBuffer.Array, recvBuffer, recvBuffer2);
                        var count = 0;
                        using (var readMs = new MemoryStream(currentPackageBuffer.Array, currentPackageBuffer.Offset, currentPackageBuffer.Count, false))
                        using (var writeMs = new MemoryStream(currentBuffer, 0, currentBuffer.Length))
                        {
                            using (var gzStream = new GZipStream(readMs, CompressionMode.Decompress, true))
                                gzStream.CopyTo(writeMs);
                            count = Convert.ToInt32(writeMs.Position);
                        }
                        currentPackageBuffer = new ArraySegment<byte>(currentBuffer, 0, count);
                    }
                }
                var packageType = (QpPackageType)currentPackageBuffer.Array[currentPackageBuffer.Offset + PACKAGE_HEAD_LENGTH - 1];
                //如果当前包是拆分包
                if (packageType == QpPackageType.Split)
                {
                    if (!isReadingSplitPackage)
                    {
                        var tmpPackageBodyLength = BitConverter.ToInt32(currentPackageBuffer.Array, currentPackageBuffer.Offset + PACKAGE_HEAD_LENGTH);
                        splitMsCapacity = tmpPackageBodyLength;
                        if (splitMsCapacity <= 0)
                            throw new IOException($"拆分包中包长度[{splitMsCapacity}]必须为正数！");
                        if (splitMsCapacity > options.MaxPackageSize)
                            throw new IOException($"拆分包中包长度[{splitMsCapacity}]大于最大包大小[{options.MaxPackageSize}]");
                        splitMs = new MemoryStream(splitMsCapacity);
                        isReadingSplitPackage = true;
                    }
                    splitMs.Write(currentPackageBuffer.Array, currentPackageBuffer.Offset + PACKAGE_HEAD_LENGTH, currentPackageBuffer.Count - PACKAGE_HEAD_LENGTH);

                    //如果拆分包已经读取完成
                    if (splitMs.Position >= splitMsCapacity)
                    {
                        finalPackageBuffer = new ArraySegment<byte>(splitMs.ToArray());
                        splitMs.Dispose();
                        break;
                    }
                }
                else
                {
                    finalPackageBuffer = currentPackageBuffer;
                    break;
                }
            }

            return new ArraySegment<byte>(finalPackageBuffer.Array,
                    finalPackageBuffer.Offset,
                    finalPackageBuffer.Count);
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
                        SendHeartbeatPackage();
                    }
                    BeginHeartBeat(cancellationToken);
                });
        }

        /// <summary>
        /// 收到心跳数据包事件
        /// </summary>
        public event EventHandler HeartbeatPackageReceived;
        /// <summary>
        /// 收到通知数据包事件
        /// </summary>
        public event EventHandler NoticePackageReceived;
        /// <summary>
        /// 收到命令请求数据包事件
        /// </summary>
        public event EventHandler CommandRequestPackageReceived;
        /// <summary>
        /// 收到命令响应数据包事件
        /// </summary>
        public event EventHandler CommandResponsePackageReceived;

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
                    OnReadError(new TaskCanceledException());
                    return;
                }
                //如果读取出错
                if (t.IsFaulted)
                {
                    try
                    {
                        //if (QpPackageHandler_Stream.CanWrite)
                        //    SendPackage(new CommandResponsePackage()
                        //    {
                        //        Code = -1,
                        //        Message = ExceptionUtils.GetExceptionMessage(t.Exception)
                        //    });
                    }
                    catch { }
                    OnReadError(t.Exception);
                    return;
                }
                //读取下一个数据包
                BeginReadPackage(token);
                var package = t.Result;
                if (package.Count == 0)
                    return;

                var packageType = (QpPackageType)package.Array[package.Offset + 4];
                switch (packageType)
                {
                    case QpPackageType.Heartbeat:
                        logger.LogTrace("[Recv-HeartbetaPackage]{0}", DateTime.Now);
                        HeartbeatPackageReceived?.Invoke(this, EventArgs.Empty);
                        break;
                    case QpPackageType.Notice:
                        NoticePackageReceived?.Invoke(this, EventArgs.Empty);
                        break;
                    case QpPackageType.CommandRequest:
                        CommandRequestPackageReceived?.Invoke(this, EventArgs.Empty);
                        break;
                    case QpPackageType.CommandResponse:
                        CommandResponsePackageReceived?.Invoke(this, EventArgs.Empty);
                        break;
                }
            });
        }
    }
}
