using System;
using System.Collections.Generic;
using System.Text;
using Quick.Protocol.Core;

namespace Quick.Protocol.Commands
{
    public class CommandExecuterManager : ICommandExecuter
    {
        public Type CommandType => null;
        Dictionary<Type, ICommandExecuter> commandExecuterDict = new Dictionary<Type, ICommandExecuter>();

        public void Add(ICommandExecuter executer)
        {
            commandExecuterDict[executer.CommandType] = executer;
        }

        public void Add<TCommandHandler>(params object[] args)
            where TCommandHandler : ICommandExecuter, new()
        {
            var executer = (TCommandHandler)Activator.CreateInstance(typeof(TCommandHandler), args);
            Add(executer);
        }

        public void Execute(QpCommandHandler channel, ICommand cmd)
        {
            if (cmd == null)
                return;
            var type = cmd.GetType();
            if (!commandExecuterDict.ContainsKey(type))
                return;
            var executer = commandExecuterDict[type];
            executer.Execute(channel, cmd);
        }
    }
}
