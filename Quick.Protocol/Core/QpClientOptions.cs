using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol.Core
{
    public class QpClientOptions : QpCommandHandlerOptions
    {
        /// <summary>
        /// 连接超时时间(默认为5秒)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5 * 1000;

        /// <summary>
        /// 当认证通过时
        /// </summary>
        public void OnAuthPassed()
        {

        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {

        }
    }
}
