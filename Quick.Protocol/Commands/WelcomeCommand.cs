using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class WelcomeCommand : AbstractCommand<WelcomeCommand.CommandContent, object>
    {
        public override string Action => "/Welcome";

        public class CommandContent
        {
            public string ProtocolVersion { get; set; }
            public string ServerProgram { get; set; }
            public QpInstruction[] InstructionSet { get; set; }
        }

        public WelcomeCommand() { }
        public WelcomeCommand(CommandContent content) : base(content) { }
    }
}
