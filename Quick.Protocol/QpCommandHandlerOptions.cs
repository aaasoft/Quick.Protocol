using Newtonsoft.Json;
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
        private QpInstruction[] _InstructionSet;
        [JsonIgnore]
        public QpInstruction[] InstructionSet
        {
            get { return _InstructionSet; }
            set
            {
                _InstructionSet = value;
                foreach (var item in value)
                {
                    AddSupportPackages(item.SupportPackages);
                    AddSupportCommands(item.SupportCommands);
                }
            }
        }

        private Dictionary<string, ICommand> commandDict = new Dictionary<string, ICommand>();

        public void AddSupportCommands(ICommand[] commands)
        {
            foreach (var item in commands)
                commandDict[item.Action] = item;
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
            InstructionSet = new[] { Quick.Protocol.Base.Instruction };
            InstructionSet = new QpInstruction[0];
        }
    }
}
