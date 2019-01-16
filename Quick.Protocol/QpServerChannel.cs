﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpServerChannel : QpCommandHandler
    {
        private TcpClient tcpClient;
        private QpServerOptions options;
        private string question;
        private bool isAuthSuccess = false;
        public EndPoint EndPoint { get; private set; }

        /// <summary>
        /// 连接断开时
        /// </summary>
        public event EventHandler Disconnected;

        public QpServerChannel(TcpClient tcpClient, CancellationToken token, QpServerOptions options) : base(options)
        {
            this.tcpClient = tcpClient;
            this.options = options;
            this.EndPoint = tcpClient.Client.RemoteEndPoint;

            tcpClient.ReceiveTimeout = options.ReceiveTimeout;
            tcpClient.SendTimeout = options.SendTimeout;

            this.CommandReceived += QpServerChannel_CommandReceived;

            InitQpPackageHandler_Stream(tcpClient.GetStream());
            BeginReadPackage(token);
        }

        public void Start()
        {
            var welcomeCmd = new Commands.WelcomeCommand(new Commands.WelcomeCommand.CommandContent()
            {
                ProtocolVersion = QpConsts.QUICK_PROTOCOL_VERSION,
                ServerProgram = options.ServerProgram,
                InstructionSet = options.SupportInstructionSet
            });
            question = welcomeCmd.Id;

            //发送欢迎指令
            var ret = SendCommand(welcomeCmd).Result;

            if (ret.Code != 0)
                throw new IOException(ret.Message);
        }

        private void QpServerChannel_CommandReceived(object sender, Commands.ICommand e)
        {
            if (e == null)
                return;
            var authCmd = e as Commands.AuthenticateCommand;
            if (authCmd == null)
            {
                if (!isAuthSuccess)
                {
                    OnReadError(new IOException("No authenticated"));
                }
                return;
            }

            var authCmdContent = authCmd.ContentT;
            if (Utils.CryptographyUtils.ComputeMD5Hash(question + options.Password) != authCmdContent.Answer)
            {
                SendPackage(new Packages.CommandResponsePackage(e.Id)
                {
                    Code = -1,
                    Message = "认证失败！"
                }).ContinueWith(t =>
                {
                    if (tcpClient.Connected)
                        OnReadError(new IOException("认证失败！"));
                });
                return;
            }
            SendPackage(new Packages.CommandResponsePackage(e.Id)
            {
                Code = 0,
                Message = "认证通过！"
            }).ContinueWith(t =>
            {
                isAuthSuccess = true;
                options.Compress = authCmdContent.Compress;
                options.Encrypt = authCmdContent.Encrypt;
            });
        }

        protected override void OnReadError(Exception exception)
        {
            base.OnReadError(exception);
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
