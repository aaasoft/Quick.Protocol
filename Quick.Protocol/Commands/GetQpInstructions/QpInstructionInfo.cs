using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands.GetQpInstructions
{
    /// <summary>
    /// 指令集信息
    /// </summary>
    public class QpInstructionInfo
    {
        /// <summary>
        /// 指令集编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 指令集名称
        /// </summary>
        public string Name { get; set; }
    }
}
