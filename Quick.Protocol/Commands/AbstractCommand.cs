using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public abstract class AbstractCommand<TRequestContent, TResponseData> : ICommand
    {
        public abstract string Action { get; }

        object ICommand.Content => Content;

        public TRequestContent Content { get; set; }

        public CommandResponse<TResponseData> ParseResponse(byte[] buffer)
        {
            return null;
        }
    }
}
