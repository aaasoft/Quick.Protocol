﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class WelcomeCommand : AbstractCommand<WelcomeCommand.CommandContent, object>
    {
        public override string Action { get; internal set; } = "/Welcome";

        public class CommandContent
        {
            public string ProtocolVersion { get; set; }
            public string ServerProgram { get; set; }
            public string Question { get; set; }
            public Instruction[] InstructionSet { get; set; }
        }
    }
}
