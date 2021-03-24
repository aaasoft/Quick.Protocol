﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands.HandShake
{
    public class Request
    {
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
    }
}
