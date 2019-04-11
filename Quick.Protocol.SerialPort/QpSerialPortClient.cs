using Microsoft.Extensions.Logging;
using Quick.Protocol.Core;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.SerialPort
{
    public class QpSerialPortClient : QpClient
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private QpSerialPortClientOptions options;
        private System.IO.Ports.SerialPort serialPort;
        public QpSerialPortClient(QpSerialPortClientOptions options) : base(options)
        {
            this.options = options;
        }

        protected override async Task<Stream> InnerConnectAsync()
        {
            logger.LogTrace($"Opening SerialPort[{options.PortName}]...");
            serialPort = new System.IO.Ports.SerialPort(options.PortName,
                                                            options.BaudRate,
                                                            options.Parity,
                                                            options.DataBits,
                                                            options.StopBits);
            await Task.Run(() => serialPort.Open());
            logger.LogTrace($"SerialPort[{options.PortName}] open success.");
            serialPort.WriteTimeout = options.SendTimeout;
            serialPort.WriteLine(QpConsts.QuickProtocolNameAndVersion);
            return serialPort.BaseStream;
        }

        protected override void Disconnect()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
            base.Disconnect();
        }
    }
}
