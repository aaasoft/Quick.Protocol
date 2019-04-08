using Microsoft.Extensions.Logging;
using Quick.Protocol.Core;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.Pipeline
{
    public class QpPipelineServer : QpServer
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private QpPipelineServerOptions options;
        private NamedPipeServerStream serverStream;
        public QpPipelineServer(QpPipelineServerOptions options) : base(options)
        {
            this.options = options;
        }

        public override void Start()
        {
            newServerStream();
            base.Start();
        }

        private void newServerStream()
        {
            serverStream = new NamedPipeServerStream(options.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public override void Stop()
        {
            base.Stop();
            serverStream.Dispose();
            serverStream = null;
        }

        protected override Task InnerAcceptAsync(CancellationToken token)
        {
            //如果管道已经连接，则返回等待1秒
            if (serverStream.IsConnected)
                return Task.Delay(TimeSpan.FromSeconds(1));

            Task waitForConnectionTask = null;
            try
            {
#if NETSTANDARD2_0
                waitForConnectionTask = serverStream.WaitForConnectionAsync(token);
#else
            waitForConnectionTask = Task.Run(() => serverStream.WaitForConnection());
#endif
            }
            catch (ObjectDisposedException)
            {
                serverStream.Dispose();
                newServerStream();
            }
            if (waitForConnectionTask == null)
                return Task.FromResult(0);
            return waitForConnectionTask.ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return;
                if (task.IsFaulted)
                    return;
                OnNewChannelConnected(serverStream, $"Pipe:{options.PipeName}", token);
            });
        }
    }
}
