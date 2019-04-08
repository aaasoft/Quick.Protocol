using Newtonsoft.Json;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Quick.Protocol
{
    public class QpServerOptions : QpCommandHandlerOptions
    {
        /// <summary>
        /// 服务端程序
        /// </summary>
        public string ServerProgram { get; set; }

        internal new bool Compress { get => base.Compress; set => base.Compress = value; }
        internal new bool Encrypt { get => base.Encrypt; set => base.Encrypt = value; }


        public virtual QpServerOptions Clone()
        {
            var ret = JsonConvert.DeserializeObject<QpServerOptions>(JsonConvert.SerializeObject(this));
            ret.InstructionSet = InstructionSet;
            return ret;
        }
    }
}
