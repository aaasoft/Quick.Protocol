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
    public class UnknownCommand : ICommand
    {
        public static UnknownCommand Instance = new UnknownCommand();

        public string Action { get; private set; }
        public string Id { get; set; }
        public object Content { get; set; }

        public Task<CommandResponsePackage> ResponseTask => null;

        public ICommand Parse(CommandRequestPackage package)
        {
            return new UnknownCommand()
            {
                Action = package.Action,
                Id = package.Id,
                Content = package.Content
            };
        }

        public void SetResponse(CommandResponsePackage responsePackage)
        {
        }

        public void Timeout()
        {
        }
    }
}
