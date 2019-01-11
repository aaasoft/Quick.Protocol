using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class AuthenticateCommand : AbstractCommand<AuthenticateCommand.CommandContent, object>
    {
        public override string Action => "/Authenticate";

        public class CommandContent
        {
            public string Answer { get; set; }
        }
    }
}
