using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public interface IPackage
    {
        /// <summary>
        /// 包类型
        /// </summary>
        byte PackageType { get; }
        /// <summary>
        /// 输出Protobuf序列化结果
        /// </summary>
        /// <returns></returns>
        byte[] Output();
        /// <summary>
        /// 解析包
        /// </summary>
        /// <param name="packageBody"></param>
        /// <returns></returns>
        IPackage Parse(byte[] packageBody);
    }
}
