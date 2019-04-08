using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.Protocol
{
    public class QpClientOptions : QpCommandHandlerOptions
    {
        /// <summary>
        /// 连接超时时间(默认为5秒)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5 * 1000;
        /// <summary>
        /// 启用加密(默认为false)
        /// </summary>
        public bool EnableEncrypt { get; set; }
        /// <summary>
        /// 启用压缩(默认为false)
        /// </summary>
        public bool EnableCompress { get; set; }

        /// <summary>
        /// 当认证通过时
        /// </summary>
        public void OnAuthPassed()
        {
            Compress = EnableCompress;
            Encrypt = EnableEncrypt;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Compress = false;
            Encrypt = false;
        }
    }
}
