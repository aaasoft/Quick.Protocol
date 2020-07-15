using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Packages
{
    [ProtoContract]
    public class SplitPackage : AbstractPackage
    {
        public const byte PACKAGE_TYPE = 2;
        public override byte PackageType => PACKAGE_TYPE;
    }
}
