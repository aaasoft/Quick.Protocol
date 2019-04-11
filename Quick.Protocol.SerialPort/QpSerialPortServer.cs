﻿using Microsoft.Extensions.Logging;
using Quick.Protocol.Core;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.SerialPort
{
    public class QpSerialPortServer : QpServer
    {
        private readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private QpSerialPortServerOptions options;
        private System.IO.Ports.SerialPort serialPort;
        private bool isAccepted = false;

        public QpSerialPortServer(QpSerialPortServerOptions options) : base(options)
        {
            this.options = options;
        }

        public override void Start()
        {
            this.ChannelDisconnected += QpSerialPortServer_ChannelDisconnected;
            logger.LogTrace($"Opening SerialPort[{options.PortName}]...");
            serialPort = new System.IO.Ports.SerialPort(options.PortName,
                                                options.BaudRate,
                                                options.Parity,
                                                options.DataBits,
                                                options.StopBits);
            serialPort.Open();
            logger.LogTrace($"SerialPort[{options.PortName}] open success.");
            isAccepted = false;
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
            this.ChannelDisconnected -= QpSerialPortServer_ChannelDisconnected;
        }

        private void QpSerialPortServer_ChannelDisconnected(object sender, QpServerChannel e)
        {
            isAccepted = false;
        }

        protected override Task InnerAcceptAsync(CancellationToken token)
        {
            if (isAccepted)
                return Task.Delay(1000, token);
            isAccepted = true;
            
            return Task.Run(() =>
            {
                if (!serialPort.IsOpen)
                    serialPort.Open();
                serialPort.ReadByte();
            }, token)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled)
                        return;
                    if (task.IsFaulted)
                        return;
                    OnNewChannelConnected(serialPort.BaseStream, $"SerialPort:{options.PortName}", token);
                });
        }
    }
}
