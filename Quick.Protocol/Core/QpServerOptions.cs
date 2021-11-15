﻿using Newtonsoft.Json;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Quick.Protocol.Core
{
    public class QpServerOptions : QpCommandHandlerOptions
    {
        /// <summary>
        /// 缓存大小(默认128KB)
        /// </summary>
        public int BufferSize = 128 * 1024;

        /// <summary>
        /// 服务端程序
        /// </summary>
        public string ServerProgram { get; set; }

        /// <summary>
        /// 协议错误处理器
        /// </summary>
        [JsonIgnore]
        public Action<Stream> ProtocolErrorHandler { get; set; }

        public virtual QpServerOptions Clone()
        {
            var ret = JsonConvert.DeserializeObject<QpServerOptions>(JsonConvert.SerializeObject(this));
            ret.InstructionSet = InstructionSet;
            ret.ProtocolErrorHandler = ProtocolErrorHandler;
            return ret;
        }
    }
}