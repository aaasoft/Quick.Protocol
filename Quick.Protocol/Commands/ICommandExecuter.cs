using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public interface ICommandExecuter
    {
        Type CommandType { get; }
        void Execute(QpCommandHandler channel, ICommand cmd);
    }
}
