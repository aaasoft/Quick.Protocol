using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public class QpClientOptions
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 是否压缩
        /// </summary>
        public bool Compress { get; set; }
        /// <summary>
        /// 是否加密
        /// </summary>
        public bool Encrypt { get; set; }

        private Dictionary<byte, IPackage> packageDict;
        /// <summary>
        /// 支持的数据包
        /// </summary>
        public IPackage[] SupportPackages
        {
            set
            {
                packageDict = value.ToDictionary(t => t.PackageType, t => t);
            }
        }

        /// <summary>
        /// 需要支持的指令集
        /// </summary>
        public string[] NeededInstructionSet { get; set; }
        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 发送超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 5 * 1000;

        /// <summary>
        /// 检查配置是否正确
        /// </summary>
        public void Check()
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentNullException(nameof(Host));
            if (packageDict == null)
                throw new ArgumentNullException(nameof(SupportPackages));
        }

        public IPackage ParsePackage(byte packageType, byte[] buffer, int offset, int size)
        {
            if (!packageDict.ContainsKey(packageType))
                return null;
            var package = packageDict[packageType];

            //解密
            //buffer = xxxx
            //解压缩
            //buffer = xxxx
            return package.Parse(buffer);
        }
    }
}
