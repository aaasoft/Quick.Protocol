using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public abstract class AbstractPackage : IPackage
    {
        public abstract byte PackageType { get; }
        public IPackage Parse(byte[] packageBody)
        {
            using (var ms = new MemoryStream(packageBody))
                return (IPackage)ProtoBuf.Serializer.Deserialize(this.GetType(), ms);
        }

        public byte[] Output()
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
