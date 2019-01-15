using Microsoft.Extensions.Logging;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Quick.Protocol
{
    public class QpServer
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private CancellationTokenSource cts;
        private QpServerOptions options;
        private TcpListener tcpListener;
        private List<QpServerChannel> channelList = new List<QpServerChannel>();

        public QpServer(QpServerOptions options)
        {
            this.options = options;
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(options.Address, options.Port);
            tcpListener.Start();
            beginAccept(cts.Token);
        }

        private void beginAccept(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            tcpListener?.AcceptTcpClientAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return;
                if (task.IsFaulted)
                    return;
                beginAccept(token);

                var tcpClient = task.Result;
                if (tcpClient == null)
                    return;
                try
                {
                    var remoteEndPointStr = tcpClient.Client.RemoteEndPoint.ToString();
                    logger.LogTrace("[Connection]{0} connected.", remoteEndPointStr);
                    var channel = new QpServerChannel(tcpClient, token, (QpServerOptions)options.Clone());
                    lock (channelList)
                        channelList.Add(channel);
                    channel.Disconnected += (sender, e) =>
                    {
                        logger.LogTrace("[Connection]{0} Disconnected.", remoteEndPointStr);
                        lock (channelList)
                            if (channelList.Contains(channel))
                                channelList.Remove(channel);
                        try { tcpClient.Close(); }
                        catch { }
                    };
                    channel.Start();
                }
                catch
                {
                    try { tcpClient.Close(); }
                    catch { }
                }
            });
        }

        public void Stop()
        {
            cts.Cancel();
            cts = null;
            tcpListener.Stop();
            tcpListener = null;
        }
    }
}
