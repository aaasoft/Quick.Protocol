using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpCommandHandlerOptions : QpPackageHandlerOptions
    {
        // <summary>
        // 指令执行器管理器
        // </summary>
        public CommandExecuterManager CommandExecuterManager { get; set; }
    }
}
