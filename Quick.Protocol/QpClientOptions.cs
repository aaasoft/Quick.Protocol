using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public class QpClientOptions : QpCommandHandlerOptions
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 需要支持的指令集
        /// </summary>
        public string[] NeededInstructionSet { get; set; }

        /// <summary>
        /// 检查配置是否正确
        /// </summary>
        public void Check()
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentNullException(nameof(Host));
        }
    }
}
