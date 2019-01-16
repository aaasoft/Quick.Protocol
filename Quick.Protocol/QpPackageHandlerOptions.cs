using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpPackageHandlerOptions
    {
        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 发送超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 心跳间隔
        /// </summary>
        public int HeartBeatInterval { get; set; } = 2 * 1000;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 是否压缩(此参数仅客户端使用有效)
        /// </summary>
        internal virtual bool Compress { get; set; } = false;
        /// <summary>
        /// 是否加密(此参数仅客户端使用有效)
        /// </summary>
        internal virtual bool Encrypt { get; set; } = false;

        private Dictionary<byte, IPackage> packageDict = new Dictionary<byte, IPackage>();
        /// <summary>
        /// 支持的数据包
        /// </summary>
        public IPackage[] SupportPackages
        {
            set
            {
                foreach (var item in value)
                    packageDict[item.PackageType] = item;
            }
        }

        public IPackage ParsePackage(byte packageType, byte[] buffer, int index, int count)
        {
            if (!packageDict.ContainsKey(packageType))
                return null;
            var package = packageDict[packageType];
            return package.Parse(buffer, index, count);
        }

        public QpPackageHandlerOptions()
        {
            SupportPackages = new IPackage[]{
                HeartBeatPackage.Instance,
                new CommandRequestPackage(),
                new CommandResponsePackage()
            };
        }
    }
}
