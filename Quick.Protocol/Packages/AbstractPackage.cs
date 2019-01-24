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

        public virtual byte[] Output()
        {
            var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, this);
            var packageBody = ms.ToArray();
            var packageLength = packageBody.Length;
            ms.Dispose();
            ms = null;

            using (var msOutput = new MemoryStream())
            {
                var packageLengthBuffer = BitConverter.GetBytes(packageLength);
                msOutput.Write(packageLengthBuffer, 0, packageLengthBuffer.Length);
                msOutput.WriteByte(PackageType);
                msOutput.Write(packageBody, 0, packageBody.Length);
                return msOutput.ToArray();
            }
        }
    }
}
