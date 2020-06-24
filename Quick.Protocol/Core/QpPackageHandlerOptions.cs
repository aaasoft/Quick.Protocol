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
        /// 缓存大小(默认1MB)
        /// </summary>
        public int BufferSize = 1 * 1024 * 1024;
        /// <summary>
        /// 发送超时(默认5秒)
        /// </summary>
        public int SendTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 接收超时(默认5秒)
        /// </summary>
        public int ReceiveTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 心跳间隔(默认2秒)
        /// </summary>
        public int HeartBeatInterval { get; set; } = 2 * 1000;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 是否压缩
        /// </summary>
        internal virtual bool Compress { get; set; } = false;
        /// <summary>
        /// 是否加密
        /// </summary>
        internal virtual bool Encrypt { get; set; } = false;

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
            if (HeartBeatInterval > ReceiveTimeout / 2)
                throw new ArgumentException("HeartBeatInterval must smaller than (ReceiveTimeout/2)", nameof(HeartBeatInterval));
            if (HeartBeatInterval > SendTimeout / 2)
                throw new ArgumentException("HeartBeatInterval must smaller than (SendTimeout/2)", nameof(HeartBeatInterval));
            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
        }
    }
}
