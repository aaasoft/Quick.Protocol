using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class AuthenticateCommand : AbstractCommand<AuthenticateCommand.CommandContent, object>
    {
        public override string Action { get; internal set; } = "/Authenticate";

        public class CommandContent
        {
            /// <summary>
            /// 是否压缩
            /// </summary>
            public bool Compress { get; set; }
            /// <summary>
            /// 是否加密
            /// </summary>
            public bool Encrypt { get; set; }
            /// <summary>
            /// 认证回答
            /// </summary>
            public string Answer { get; set; }
        }
        public AuthenticateCommand() { }
        public AuthenticateCommand(CommandContent content)
        {
            ContentT = content;
        }
    }
}
