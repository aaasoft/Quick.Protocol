using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quick.Protocol.Packages
{
    public abstract class AbstractPackage : IPackage
    {
        public abstract byte PackageType { get; }
        public virtual IPackage Parse(byte[] buffer, int index, int count)
        {
            using (var ms = new MemoryStream(buffer, index, count))
            {
                var ret = (IPackage)ProtoBuf.Serializer.Deserialize(this.GetType(), ms);
                return ret;
            }
        }

        private int serializeToStream(Stream ms)
        {
            //前4字节先填0
            for (var i = 0; i < 4; i++)
                ms.WriteByte(0);
            //第5字节为包类型
            ms.WriteByte(PackageType);

            //开始序列化
            ProtoBuf.Serializer.Serialize(ms, this);
            //包头+包体长度
            var packageTotalLength = Convert.ToInt32(ms.Position);
            //包体长度
            var packageBodyLength = packageTotalLength - 5;
            //将包体长度写入前4个字节
            ms.Position = 0;
            var packageLengthBuffer = BitConverter.GetBytes(packageBodyLength);
            ms.Write(packageLengthBuffer, 0, packageLengthBuffer.Length);
            return packageTotalLength;
        }

        public virtual int Output(byte[] targetBuffer, out byte[] outBuffer)
        {
            outBuffer = targetBuffer;
            try
            {
                if (targetBuffer != null)
                {
                    using (var ms = new MemoryStream(targetBuffer, 0, targetBuffer.Length))
                        return serializeToStream(ms);
                }
            }
            //如果序列化的长度超出了目标缓存的大小
            catch (NotSupportedException) { }

            //序列化到一个新的内存流里
            using (var ms = new MemoryStream())
            {
                var ret = serializeToStream(ms);
                outBuffer = ms.ToArray();
                return ret;
            }
        }
    }
}
