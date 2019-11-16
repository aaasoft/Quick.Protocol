using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quick.Protocol.Packages;

namespace Quick.Protocol.Commands
{
    /// <summary>
    /// 未知命令
    /// </summary>
    public class UnknownCommand : AbstractCommand<object,object>
    {
        public static UnknownCommand Instance = new UnknownCommand();
    }
}
