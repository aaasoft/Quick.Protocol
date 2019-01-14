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
        private Dictionary<string, ICommand> commandDict;

        /// <summary>
        /// 支持的指令
        /// </summary>
        public ICommand[] SupportCommands
        {
            set
            {
                commandDict = value.ToDictionary(t => t.Action, t => t);
            }
        }

        public ICommand ParseCommand(CommandRequestPackage package)
        {
            if (!commandDict.ContainsKey(package.Action))
                return null;
            var srcCommand = commandDict[package.Action];
            return srcCommand.Parse(package);
        }
    }
}
