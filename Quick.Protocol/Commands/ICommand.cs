using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// 动作
        /// </summary>
        string Action { get; }
        /// <summary>
        /// 内容
        /// </summary>
        object Content { get; }
    }
}
