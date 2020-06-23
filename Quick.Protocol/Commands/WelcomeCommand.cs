using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class WelcomeCommand : AbstractCommand<WelcomeCommand.CommandContent, object>
    {
        public class CommandContent
        {
            /// <summary>
            /// 协议版本
            /// </summary>
            public string ProtocolVersion { get; set; }
            /// <summary>
            /// 服务端程序
            /// </summary>
            public string ServerProgram { get; set; }
            /// <summary>
            /// 缓存大小
            /// </summary>
            public int BufferSize { get; set; }
            /// <summary>
            /// 支持的指令集
            /// </summary>
            public QpInstruction[] InstructionSet { get; set; }
        }

        public WelcomeCommand() { }
        public WelcomeCommand(CommandContent content) : base(content) { }
    }
}
