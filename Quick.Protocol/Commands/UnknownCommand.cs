using System;
using System.Collections.Generic;
using System.Text;
using Quick.Protocol.Packages;

namespace Quick.Protocol.Commands
{
    /// <summary>
    /// 未知命令
    /// </summary>
    public class UnknownCommand : ICommand
    {
        public static UnknownCommand Instance = new UnknownCommand();

        public string Action { get; private set; }
        public string Id { get; set; }
        public object Content { get; set; }

        public ICommand Parse(CommandRequestPackage package)
        {
            return new UnknownCommand()
            {
                Action = package.Action,
                Id = package.Id,
                Content = package.Content
            };
        }
    }
}
