using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.WebSocket
{
    public class QpWebSocketClient : QpClient
    {
        private QpWebSocketClientOptions options;
        private System.Net.WebSockets.ClientWebSocket client;

        public QpWebSocketClient(QpWebSocketClientOptions options) : base(options)
        {
            this.options = options;
        }

        protected override async Task<Stream> InnerConnectAsync()
        {
            client = new System.Net.WebSockets.ClientWebSocket();
            await client.ConnectAsync(new Uri($"ws://{options.Host}:{options.Port}"), CancellationToken.None);
            return new WebSocketClientStream(client);
        }
    }
}
