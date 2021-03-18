using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quick.Protocol
{
    public class CommandExecuterManager
    {
        private Dictionary<string, Delegate> commandExecuterDict = new Dictionary<string, Delegate>();

        public void Register(string cmdRequestTypeName, Delegate commandExecuter)
        {
            commandExecuterDict[cmdRequestTypeName] = commandExecuter;
        }

        public void Register<TCmdRequest, TCmdResponse>(Func<TCmdRequest, TCmdResponse> commandExecuter)
            where TCmdRequest : class, new()
            where TCmdResponse : class, new()
        {
            var cmdRequestTypeName = typeof(TCmdRequest).FullName;
            Register(cmdRequestTypeName, commandExecuter);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="cmdRequestTypeName"></param>
        /// <param name="cmdRequestModel"></param>
        /// <returns></returns>
        public object ExecuteCommand(string cmdRequestTypeName, object cmdRequestModel)
        {
            if (!commandExecuterDict.ContainsKey(cmdRequestTypeName))
                throw new IOException($"Command Request Type[{cmdRequestTypeName}] has no executer.");
            Delegate commandExecuter = commandExecuterDict[cmdRequestTypeName];
            return commandExecuter.DynamicInvoke(new object[] { cmdRequestModel });
        }
    }
}
