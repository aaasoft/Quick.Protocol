using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public abstract class AbstractCommandExecuter : ICommandExecuter
    {
        public Type CommandType { get; private set; }
        public AbstractCommandExecuter(Type commandType)
        {
            CommandType = commandType;
        }
        public abstract void Execute(QpCommandHandler channel, ICommand cmd);
    }

    public abstract class AbstractCommandExecuter<TCommand> : AbstractCommandExecuter
        where TCommand : ICommand
    {
        public AbstractCommandExecuter() : base(typeof(TCommand))
        {
        }
        public override void Execute(QpCommandHandler channel, ICommand cmd)
        {
            Execute(channel, (TCommand)cmd);
        }
        public abstract void Execute(QpCommandHandler channel, TCommand cmd);
    }
}
