using System;
using System.Collections.Generic;
using System.Text;
using Quick.Protocol.Core;

namespace Quick.Protocol.Commands
{
    public class CommandExecuterManager : ICommandExecuter
    {
        /// <summary>
        /// 其他命令执行器
        /// </summary>
        public ICommandExecuter OtherCommandExecuter { get; set; }

        Dictionary<Type, ICommandExecuter> commandExecuterDict = new Dictionary<Type, ICommandExecuter>();

        public void Add(Type commandType, ICommandExecuter executer)
        {
            commandExecuterDict[commandType] = executer;
        }

        public void Add<TCommand, TCommandHandler>(params object[] args)
            where TCommand : ICommand
            where TCommandHandler : ICommandExecuter
        {
            var executerType = typeof(TCommandHandler);
            var executer = (TCommandHandler)Activator.CreateInstance(executerType, args);
            Add(typeof(TCommand), executer);
        }

        public void Execute(QpCommandHandler handler, ICommand cmd)
        {
            if (cmd == null)
                return;

            var type = cmd.GetType();
            if (commandExecuterDict.ContainsKey(type))
            {
                var executer = commandExecuterDict[type];
                executer.Execute(handler, cmd);
                return;
            }
            OtherCommandExecuter?.Execute(handler, cmd);
        }
    }
}
