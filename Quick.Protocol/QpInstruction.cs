using Newtonsoft.Json;
using Quick.Protocol.Commands;
using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public class QpInstruction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public IPackage[] SupportPackages { get; set; }
        [JsonIgnore]
        public ICommand[] SupportCommands { get; set; }
    }
}
