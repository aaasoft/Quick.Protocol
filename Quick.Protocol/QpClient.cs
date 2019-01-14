﻿using Newtonsoft.Json;
using Quick.Protocol.Commands;
using Quick.Protocol.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpClient : QpCommandHandler
    {
        private CancellationTokenSource cts = null;
        private QpClientOptions options;
        private TcpClient tcpClient;

        /// <summary>
        /// 服务端欢迎信息
        /// </summary>
        public WelcomeCommand.CommandContent WelcomeContent { get; private set; }
        /// <summary>
        /// 连接断开时
        /// </summary>
        public event EventHandler Disconnected;

        public QpClient(QpClientOptions options)
            : base(options)
        {
            options.Check();
            this.options = options;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public async Task ConnectAsync()
        {
            //清理
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (tcpClient != null)
                Close();
            //开始连接
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = options.SendTimeout;
            tcpClient.ReceiveTimeout = options.ReceiveTimeout;
            await tcpClient.ConnectAsync(options.Host, options.Port);
            
            //初始化网络
            InitQpPackageHandler_Stream(tcpClient.GetStream());

            //读取服务端发来的欢迎信息
            var welcomePackage = await ReadPackageAsync(token) as CommandRequestPackage;
            if (welcomePackage == null || string.IsNullOrEmpty(welcomePackage.Content))
                throw new IOException("Could't read welcome command,protocol error.");

            //验证指令集
            var welcomeCommand = welcomePackage.ToCommand<WelcomeCommand, WelcomeCommand.CommandContent, object>();
            WelcomeContent = welcomeCommand.ContentT;
            if (WelcomeContent == null)
            {
                var errorMessage = "WelcomContent is null,protocol error.";
                SendPackage(welcomePackage.Id, -1, errorMessage);
                throw new IOException(errorMessage);
            }
            var notSupportInstructionNames = options.NeededInstructionSet.Except(WelcomeContent.InstructionSet.Select(t => t.Id)).ToArray();
            if (notSupportInstructionNames.Length > 0)
            {
                var errorMessage = $"Client need instruction[{string.Join(",", notSupportInstructionNames)}] not support by server.";
                SendPackage(welcomePackage.Id, -1, errorMessage);
                throw new IOException(errorMessage);
            }
            SendPackage(welcomePackage.Id, 0, "OK");

            //开始读取其他数据包
            BeginReadPackage(token);
            
            //开始认证
            var authenticateResult = await SendCommand(new AuthenticateCommand(new AuthenticateCommand.CommandContent()
            {
                Compress = options.Compress,
                Encrypt = options.Encrypt,
                Answer = Utils.CryptographyUtils.DesEncrypt(welcomeCommand.Id, options.Password)
            }));
            if (authenticateResult.Code != 0)
                throw new IOException(authenticateResult.Message);
        }

        protected override void OnReadError(Exception exception)
        {
            base.OnReadError(exception);
            cancellAll();
            disconnect();
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void cancellAll()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }

        private void disconnect()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            cancellAll();
            disconnect();
        }

        public void SendPackage(string id, int code, string message) => SendPackage(id, code, message, null);
        public void SendPackage(string id, int code, string message, string content)
        {
            SendPackage(new CommandResponsePackage()
            {
                Id = id,
                Code = code,
                Message = message,
                Content = content
            });
        }
    }
}
