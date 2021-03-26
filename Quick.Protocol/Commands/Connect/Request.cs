using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands.Connect
{
    /// <summary>
    /// 连接请求命令
    /// </summary>
    public class Request : IQpCommandRequest<Response>
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public string ProtocolVersion { get; set; } = "2";
        /// <summary>
        /// 需要的指令集编号数组
        /// </summary>
        public string[] NeededInstructionIds { get; set; }
    }
}
