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
                NoticeInfo.Create(new Notices.PrivateNotice())
            },
            CommandInfos = new CommandInfo[]
            {
                CommandInfo.Create(new Commands.Connect.Request()),
                CommandInfo.Create(new Commands.Authenticate.Request()),
                CommandInfo.Create(new Commands.HandShake.Request()),
                CommandInfo.Create(new Commands.PrivateCommand.Request()),
                CommandInfo.Create(new Commands.GetQpInstructions.Request()),
            }
        };
    }
}
