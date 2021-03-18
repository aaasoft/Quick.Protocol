using System;
using System.Collections.Generic;
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
        /// 传输超时(默认15秒)
        /// </summary>
        public int TransportTimeout
        {
            get { return InternalTransportTimeout; }
            set { InternalTransportTimeout = value; }
        }

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
            InternalCompress = EnableCompress;
            InternalEncrypt = EnableEncrypt;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InternalCompress = false;
            InternalEncrypt = false;
        }
    }
}
