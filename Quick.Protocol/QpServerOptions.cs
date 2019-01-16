using Newtonsoft.Json;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Quick.Protocol
{
    public class QpServerOptions : QpCommandHandlerOptions, ICloneable
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [JsonIgnore]
        public IPAddress Address { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 服务端程序
        /// </summary>
        public string ServerProgram { get; set; }

        internal new bool Compress { get => base.Compress; set => base.Compress = value; }
        internal new bool Encrypt { get => base.Encrypt; set => base.Encrypt = value; }

        private Dictionary<string, Instruction> _dictInstruction = new Dictionary<string, Instruction>();
        public Instruction[] InstructionSet
        {
            set
            {
                foreach (var item in value)
                    _dictInstruction[item.Id] = item;
            }
        }
        /// <summary>
        /// 支持的指令集
        /// </summary>
        public Instruction[] SupportInstructionSet => _dictInstruction.Values.ToArray();

        public QpServerOptions()
        {
            InstructionSet = new[] { Base.Instruction };
        }

        public override void Check()
        {
            base.Check();
            if (Address == null)
                throw new ArgumentNullException(nameof(Address));
            if (Port < 0 || Port > 65535)
                throw new ArgumentException("Port must between 0 and 65535", nameof(Port));
        }

        public object Clone()
        {
            var ret = JsonConvert.DeserializeObject<QpServerOptions>(JsonConvert.SerializeObject(this));
            ret.Address = Address;
            return ret;
        }
    }
}
