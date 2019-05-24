using Fleck;
using Microsoft.Extensions.Logging;
using Quick.Protocol.Core;
using Quick.Protocol.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.WebSocket
{
    public class QpWebSocketServer : QpServer
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private QpWebSocketServerOptions options;
        private IWebSocketServer server;
        private Queue<IWebSocketConnection> connectionQueue = new Queue<IWebSocketConnection>();
        private AutoResetEvent waitForConnectionAutoResetEvent;

        public QpWebSocketServer(QpWebSocketServerOptions options) : base(options)
        {
            this.options = options;
        }
        public override void Start()
        {
            waitForConnectionAutoResetEvent = new AutoResetEvent(false);
            lock (connectionQueue)
                connectionQueue.Clear();
            server = new WebSocketServer($"ws://{options.Address}:{options.Port}");
            server.Start(onNewConnection);
            base.Start();
        }

        private void onNewConnection(IWebSocketConnection connection)
        {
            lock (connectionQueue)
                connectionQueue.Enqueue(connection);
            waitForConnectionAutoResetEvent.Set();
        }

        public override void Stop()
        {
            server.Dispose();
            lock (connectionQueue)
                connectionQueue.Clear();
            waitForConnectionAutoResetEvent.Dispose();
            base.Stop();
        }
        
        protected override Task InnerAcceptAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                waitForConnectionAutoResetEvent.WaitOne();
                IWebSocketConnection[] connections = null;
                lock (connectionQueue)
                {
                    connections = connectionQueue.ToArray();
                    connectionQueue.Clear();
                }
                foreach (var connection in connections)
                {
                    try
                    {
                        var remoteEndPointStr = $"WebSocket:{connection.ConnectionInfo.ClientIpAddress}:{connection.ConnectionInfo.ClientPort}";
                        logger.LogTrace("[Connection]{0} connected.", remoteEndPointStr);
                        OnNewChannelConnected(new WebSocketStream(connection), remoteEndPointStr, token);
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug("[Connection]Init&Start Channel error,reason:{0}", ex.ToString());
                        try { connection.Close(); }
                        catch { }
                    }
                }
            });
        }
    }
}
