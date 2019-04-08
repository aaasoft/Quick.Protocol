﻿using Microsoft.Extensions.Logging;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpTcpServer : QpServer
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private TcpListener tcpListener;
        private QpTcpServerOptions options;

        public QpTcpServer(QpTcpServerOptions options) : base(options)
        {
            this.options = options;
        }

        public override void Start()
        {
            tcpListener = new TcpListener(options.Address, options.Port);
            tcpListener.Start();

            base.Start();
        }

        public override void Stop()
        {
            tcpListener.Stop();
            tcpListener = null;
            base.Stop();
        }

        protected override Task InnerAcceptAsync(CancellationToken token)
        {
            return tcpListener.AcceptTcpClientAsync().ContinueWith(task =>
            {
                var tcpClient = task.Result;
                if (tcpClient == null)
                    return;
                try
                {
                    var remoteEndPointStr = "TCP/" + tcpClient.Client.RemoteEndPoint.ToString();
                    logger.LogTrace("[Connection]{0} connected.", remoteEndPointStr);
                    OnNewChannelConnected(tcpClient.GetStream(), remoteEndPointStr, token);
                }
                catch (Exception ex)
                {
                    logger.LogDebug("[Connection]Init&Start Channel error,reason:{0}", ex.ToString());
                    try { tcpClient.Close(); }
                    catch { }
                }
            });
        }
    }
}
