using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.Packages
{
    [ProtoContract]
    public class CommandResponsePackage : AbstractPackage
    {
        public override byte PackageType => 255;
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public int Code { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public string Content { get; set; }

        public CommandResponsePackage() { }
        public CommandResponsePackage(string id)
        {
            this.Id = id;
        }

        public override string ToString()
        {
            return $"CommandResponsePackage[Id:{Id},Code:{Code},Message:{Message},Content:{Content}]";
        }
    }
}
