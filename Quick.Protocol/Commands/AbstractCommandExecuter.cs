using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public abstract class AbstractCommandExecuter<TCommand> : ICommandExecuter
        where TCommand : ICommand
    {
        public void Execute(QpCommandHandler channel, ICommand cmd)
        {
            Execute(channel, (TCommand)cmd);
        }
        public abstract void Execute(QpCommandHandler channel, TCommand cmd);
    }
}
