using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    [ProtoContract]
    public class CommandRequestPackage : AbstractPackage
    {
        public override byte PackageType => 200;
        [ProtoMember(1)]
        public string Action { get; set; }
        [ProtoMember(2)]
        public string Content { get; set; }
    }
}
