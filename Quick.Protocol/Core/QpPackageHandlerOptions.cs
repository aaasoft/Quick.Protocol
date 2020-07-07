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
        /// 发送超时(默认10秒)
        /// </summary>
        public int SendTimeout { get; set; } = 10 * 1000;
        /// <summary>
        /// 接收超时(默认10秒)
        /// </summary>
        public int ReceiveTimeout { get; set; } = 10 * 1000;
        /// <summary>
        /// 心跳间隔，为发送或接收超时中小的值的一半
        /// </summary>
        public int HeartBeatInterval => Math.Min(SendTimeout, ReceiveTimeout) / 2;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最大包大小
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

        public IPackage ParsePackage(byte packageType, byte[] buffer, int index, int count)
        {
            if (!packageDict.ContainsKey(packageType))
                return null;
            var package = packageDict[packageType];
            return package.Parse(buffer, index, count);
        }

        public virtual void Check()
        {
            if (ReceiveTimeout <= 0)
                throw new ArgumentException("ReceiveTimeout must larger than 0", nameof(ReceiveTimeout));
            if (SendTimeout <= 0)
                throw new ArgumentException("SendTimeout must larger than 0", nameof(SendTimeout));
            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
        }
    }
}
