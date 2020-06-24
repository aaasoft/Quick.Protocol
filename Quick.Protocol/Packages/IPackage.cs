using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quick.Protocol.Packages
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
        /// <param name="targetBuffer"></param>
        /// <param name="outBuffer"></param>
        /// <returns></returns>
        int Output(byte[] targetBuffer,out byte[] outBuffer);
        /// <summary>
        /// 解析包
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IPackage Parse(byte[] buffer, int index, int count);
    }
}
