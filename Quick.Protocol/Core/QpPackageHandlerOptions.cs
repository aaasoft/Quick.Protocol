using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol.Core
{
    public abstract class QpPackageHandlerOptions
    {
        /// <summary>
        /// 心跳间隔，为发送或接收超时中小的值的三分一
        /// </summary>
        public int HeartBeatInterval => InternalTransportTimeout / 3;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 内部是否压缩
        /// </summary>
        internal virtual bool InternalCompress { get; set; } = false;
        /// <summary>
        /// 内部是否加密
        /// </summary>
        internal virtual bool InternalEncrypt { get; set; } = false;
        /// <summary>
        /// 内部接收超时(默认15秒)
        /// </summary>
        internal int InternalTransportTimeout { get; set; } = 15 * 1000;

        /// <summary>
        /// 最大包大小(默认为：10MB)
        /// </summary>
        public int MaxPackageSize { get; set; } = 10 * 1024 * 1024;

        private Dictionary<byte, IPackage> packageDict = new Dictionary<byte, IPackage>();

        /// <summary>
        /// 添加支持的包
        /// </summary>
        /// <param name="packages"></param>
        public void AddSupportPackages(IPackage[] packages)
        {
            if (packages == null)
                return;
            foreach (var item in packages)
                packageDict[item.PackageType] = item;
        }

        public IPackage ParsePackage(byte[] buffer, int index, int count)
        {
            var packageType = buffer[4 + index];
            if (!packageDict.ContainsKey(packageType))
                return null;
            var package = packageDict[packageType];
            return package.Parse(buffer, 5 + index, count - 5);
        }

        public virtual void Check()
        {
            if (InternalTransportTimeout <= 0)
                throw new ArgumentException("ReceiveTimeout must larger than 0", nameof(InternalTransportTimeout));
            if (InternalTransportTimeout <= 0)
                throw new ArgumentException("SendTimeout must larger than 0", nameof(InternalTransportTimeout));
            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
        }
    }
}
