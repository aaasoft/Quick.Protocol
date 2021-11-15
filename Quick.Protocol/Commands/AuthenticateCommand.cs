﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.Commands
{
    public class AuthenticateCommand : AbstractCommand<AuthenticateCommand.CommandContent, object>
    {
        public class CommandContent
        {
            /// <summary>
            /// 是否压缩
            /// </summary>
            public bool Compress { get; set; }
            /// <summary>
            /// 是否加密
            /// </summary>
            public bool Encrypt { get; set; }
            /// <summary>
            /// 认证回答
            /// </summary>
            public string Answer { get; set; }
            /// <summary>
            /// 缓存大小
            /// </summary>
            public int BufferSize { get; set; }
            /// <summary>
            /// 传输超时时间(毫秒)
            /// </summary>
            public int TransportTimeout { get; set; } = 15000;
        }
        public AuthenticateCommand() { }
        public AuthenticateCommand(CommandContent content) : base(content) { }
    }
}