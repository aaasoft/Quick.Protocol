using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public class Base
    {
        public static QpInstruction Instruction => new QpInstruction()
        {
            Id = typeof(Base).FullName,
            Name = "基础指令集",
            NoticeInfos = new NoticeInfo[] { },
            CommandInfos = new CommandInfo[]
            {
                CommandInfo.Create<Commands.Connect.Request,Commands.Connect.Response>(),
                CommandInfo.Create<Commands.Authenticate.Request,Commands.Authenticate.Response>()
            }
        };
    }
}
