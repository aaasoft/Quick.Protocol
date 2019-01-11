using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    [ProtoContract]
    public class CommandResponsePackage : AbstractPackage
    {
        public override byte PackageType => 201;
        [ProtoMember(1)]
        public int Code { get; set; }
        [ProtoMember(2)]
        public string Message { get; set; }
        [ProtoMember(3)]
        public string Content { get; set; }
    }
}
