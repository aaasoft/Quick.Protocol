using Quick.Protocol.Commands;
using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpCommandHandlerOptions : QpPackageHandlerOptions
    {
        private Dictionary<string, ICommand> commandDict = new Dictionary<string, ICommand>();

        /// <summary>
        /// 支持的指令
        /// </summary>
        public ICommand[] SupportCommands
        {
            set
            {
                foreach (var item in value)
                    commandDict[item.Action] = item;
            }
        }

        public ICommand ParseCommand(CommandRequestPackage package)
        {
            if (!commandDict.ContainsKey(package.Action))
                return null;
            var srcCommand = commandDict[package.Action];
            return srcCommand.Parse(package);
        }

        public QpCommandHandlerOptions()
        {
            SupportCommands = new ICommand[]
            {
                new WelcomeCommand(),
                new AuthenticateCommand()
            };
        }
    }
}
