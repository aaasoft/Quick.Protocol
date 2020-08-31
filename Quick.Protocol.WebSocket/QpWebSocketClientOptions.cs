using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol.WebSocket
{
    public class QpWebSocketClientOptions : QpClientOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
