using Quick.Protocol.Core;
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
            return pipeClientStream;
        }
    }
}
