using Microsoft.Extensions.Logging;
using Quick.Protocol.Core;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.Pipeline
{
    public class QpPipelineClient : QpClient
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private QpPipelineClientOptions options;

        public QpPipelineClient(QpPipelineClientOptions options) : base(options)
        {
            this.options = options;
        }

        protected override async Task<Stream> InnerConnectAsync()
        {
            var pipeClientStream = new NamedPipeClientStream(options.ServerName, options.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
#if NETSTANDARD2_0
            await pipeClientStream.ConnectAsync(options.ConnectionTimeout);
#else
            await Task.Run(() => pipeClientStream.Connect(options.ConnectionTimeout));
#endif
            pipeClientStream.ReadMode = PipeTransmissionMode.Byte;
            logger.LogTrace("服务管道数量：" + pipeClientStream.NumberOfServerInstances);
            return pipeClientStream;
        }
    }
}
