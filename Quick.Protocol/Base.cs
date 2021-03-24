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
            NoticeInfos = new NoticeInfo[]
            {
                NoticeInfo.Create<Notices.Ping>()
            },
            CommandInfos = new CommandInfo[]
            {
                CommandInfo.Create<Commands.Connect.Request,Commands.Connect.Response>(),
                CommandInfo.Create<Commands.Authenticate.Request,Commands.Authenticate.Response>(),
                CommandInfo.Create<Commands.HandShake.Request,Commands.HandShake.Response>(),
                CommandInfo.Create<Commands.PrivateCommand.Request,Commands.PrivateCommand.Response>()
            }
        };
    }
}
