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

namespace Quick.Protocol
{
    public abstract class QpPackageHandler
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private byte[] buffer = new byte[1 * 1024 * 1024];
        private byte[] encryptBuffer = new byte[1 * 1024 * 1024];

        private Stream QpPackageHandler_Stream;
        private QpPackageHandlerOptions packageHandlerOptions;

        public QpPackageHandler(QpPackageHandlerOptions packageHandlerOptions)
        {
            this.packageHandlerOptions = packageHandlerOptions;
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
            logger.LogTrace("[Recv-Package]{0}", package.ToString());
            //触发收到数据包事件
            PackageReceived?.Invoke(this, package);
        }

        public void SendPackage(IPackage package)
        {
            var stream = QpPackageHandler_Stream;
            if (stream == null)
                throw new ArgumentNullException(nameof(QpPackageHandler_Stream));
            logger.LogTrace("[Send-Package]{0}", package.ToString());
            var requestBuffer = package.Output();
            stream.Write(requestBuffer, 0, requestBuffer.Length);
            stream.Flush();
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
            ret = await stream.ReadAsync(buffer, 0, 5, token);
            if (ret < 5)
                throw new IOException($"包头读取错误！读取数据长度：{ret}");

            var packageLength = BitConverter.ToInt32(buffer, 0);
            if (packageLength > buffer.Length)
                throw new IOException($"数据包长度：{packageLength}，缓存大小：{buffer.Length}");
            var packageType = buffer[4];
            //读取包体
            ret = await stream.ReadAsync(buffer, 0, packageLength);
            if (ret < packageLength)
                throw new IOException($"包体读取错误！包长度：{packageLength}，包类型：{packageType}，读取数据长度：{ret}");

            var tmpBuffer = buffer;
            var tmpPackageLength = packageLength;

            //解密
            if (packageHandlerOptions.Encrypt)
            {
                tmpBuffer = Utils.CryptographyUtils.DesDecrypt(packageHandlerOptions.Password, buffer, 0, packageLength);
                tmpPackageLength = tmpBuffer.Length;
            }
            //解压
            if (packageHandlerOptions.Compress)
            {
                using (var mStream = new MemoryStream(encryptBuffer))
                using (var gzStream = new GZipStream(new MemoryStream(buffer, 0, packageLength), CompressionMode.Decompress))
                {
                    gzStream.CopyTo(mStream);
                    tmpBuffer = encryptBuffer;
                    tmpPackageLength = (int)mStream.Length;
                }
            }
            //解析包
            var package = packageHandlerOptions.ParsePackage(packageType, tmpBuffer, 0, tmpPackageLength);
            return package;
        }

        protected void BeginReadPackage(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            ReadPackageAsync(token).ContinueWith(t =>
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
