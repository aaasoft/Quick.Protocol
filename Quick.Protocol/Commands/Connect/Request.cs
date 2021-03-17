using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands.Connect
{
    /// <summary>
    /// 连接请求命令
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public string ProtocolVersion { get; set; } = "2";
        /// <summary>
        /// 传输超时(默认15秒)
        /// </summary>
        public int TransportTimeout { get; set; } = 15000;
        /// <summary>
        /// 启用加密(默认为false)
        /// </summary>
        public bool EnableEncrypt { get; set; } = false;
        /// <summary>
        /// 启用压缩(默认为false)
        /// </summary>
        public bool EnableCompress { get; set; } = false;
        /// <summary>
        /// 需要的指令集编号数组
        /// </summary>
        public string[] NeededInstructionIds { get; set; }
    }
}
