using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpTcpClient : QpClient
    {
        private TcpClient tcpClient;
        private QpTcpClientOptions options;

        public QpTcpClient(QpTcpClientOptions options) : base(options)
        {
            this.options = options;
        }

        protected override async Task<Stream> InnerConnectAsync()
        {
            if (tcpClient != null)
                Close();
            //开始连接
            tcpClient = new TcpClient();
            await TaskUtils.TaskWait(tcpClient.ConnectAsync(options.Host, options.Port), options.ConnectionTimeout);

            if (!tcpClient.Connected)
                throw new IOException($"Failed to connect to {options.Host}:{options.Port}.");
            return tcpClient.GetStream();
        }

        protected override void Disconnect()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
#if NETSTANDARD2_0
                tcpClient.Dispose();
#endif
                tcpClient = null;
            }

            base.Disconnect();
        }
    }
}
