using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quick.Protocol.Packages;

namespace Quick.Protocol.Commands
{
    /// <summary>
    /// 未知命令
    /// </summary>
    public class UnknownCommand : AbstractCommand<UnknownCommand.CommandContent, object>
    {
        public class CommandContent
        {
            public string Message { get; set; }
        }

        public UnknownCommand() { }
        public UnknownCommand(CommandContent content) : base(content) { }
    }
}
