using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpCommandHandlerOptions : QpPackageHandlerOptions
    {
        /// <summary>
        /// 指令执行器管理器列表
        /// </summary>
        [JsonIgnore]
        public List<CommandExecuterManager> CommandExecuterManagerList = new List<CommandExecuterManager>();

        // <summary>
        // 注册指令执行器管理器
        // </summary>
        public void RegisterCommandExecuterManager(CommandExecuterManager commandExecuterManager)
        {
            CommandExecuterManagerList.Add(commandExecuterManager);
        }
    }
}
