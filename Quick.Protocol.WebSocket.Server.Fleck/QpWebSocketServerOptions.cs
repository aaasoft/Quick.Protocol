using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Quick.Protocol.WebSocket.Server.Fleck
{
    public class QpWebSocketServerOptions : QpServerOptions
    {
        /// <summary>
        /// WebSocket的URL地址
        /// </summary>
        public string Url { get; set; }

        public override void Check()
        {
            base.Check();
            if (Url == null)
                throw new ArgumentNullException(nameof(Url));
            if (!Url.StartsWith("ws://"))
                throw new ArgumentException("Url must start with ws://", nameof(Url));
        }
    }
}
