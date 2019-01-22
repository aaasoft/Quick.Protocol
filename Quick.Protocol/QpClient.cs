using Newtonsoft.Json;
using Quick.Protocol.Commands;
using Quick.Protocol.Exceptions;
using Quick.Protocol.Packages;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpClient : QpCommandHandler
    {
        private CancellationTokenSource cts = null;
        public QpClientOptions Options { get; private set; }
        private TcpClient tcpClient;
        private bool authPassed = false;

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
            this.Options = options;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public async Task ConnectAsync()
        {
            //清理
            Close();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (tcpClient != null)
                Close();
            //开始连接
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = Options.SendTimeout;
            tcpClient.ReceiveTimeout = Options.ReceiveTimeout;
            await TaskUtils.TaskWait(tcpClient.ConnectAsync(Options.Host, Options.Port), Options.ConnectionTimeout);

            if (!tcpClient.Connected)
                throw new IOException($"Failed to connect to {Options.Host}:{Options.Port}.");
            
            //初始化网络
            InitQpPackageHandler_Stream(tcpClient.GetStream());

            //读取服务端发来的欢迎信息
            var welcomePackage = await ReadPackageAsync(token) as CommandRequestPackage;
            if (welcomePackage == null || string.IsNullOrEmpty(welcomePackage.Content))
                throw new InvalidDataException("Could't read welcome command,protocol error.");

            //验证指令集
            var welcomeCommand = welcomePackage.ToCommand<WelcomeCommand, WelcomeCommand.CommandContent, object>();
            WelcomeContent = welcomeCommand.ContentT;
            if (WelcomeContent == null)
            {
                var errorMessage = "WelcomContent is null,protocol error.";
                SendResponsePackage(welcomePackage.Id, -1, errorMessage);
                throw new InvalidDataException(errorMessage);
            }
            var notSupportInstructionNames = Options.InstructionSet.Select(t => t.Id).Except(WelcomeContent.InstructionSet.Select(t => t.Id)).ToArray();
            if (notSupportInstructionNames.Length > 0)
            {
                var errorMessage = $"Client need instruction[{string.Join(",", notSupportInstructionNames)}] not support by server.";
                SendResponsePackage(welcomePackage.Id, -1, errorMessage);
                throw new InstructionNotSupportException(errorMessage);
            }
            SendResponsePackage(welcomePackage.Id, 0, "OK");

            //开始读取其他数据包
            BeginReadPackage(token);

            //开始认证
            var authenticateResult = await SendCommand(new AuthenticateCommand(new AuthenticateCommand.CommandContent()
            {
                Compress = Options.EnableCompress,
                Encrypt = Options.EnableEncrypt,
                Answer = Utils.CryptographyUtils.ComputeMD5Hash(welcomeCommand.Id + Options.Password)
            }));

            if (authenticateResult.Code != 0)
            {
                var exception = new AuthenticateFailedException(authenticateResult.Message);
                Close();
                throw exception;
            }
            authPassed = true;
            Options.OnAuthPassed();
            //开始心跳
            BeginHeartBeat(token);
        }

        protected override void OnReadError(Exception exception)
        {
            base.OnReadError(exception);
            Options.Init();
            var tmpAuthPassed = authPassed;
            cancellAll();
            disconnect();
            if (tmpAuthPassed)
                Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void cancellAll()
        {
            authPassed = false;
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
#if NETSTANDARD2_0
                tcpClient.Dispose();
#endif
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

        public void SendResponsePackage(string id, int code, string message) => SendResponsePackage(id, code, message, null);
        public void SendResponsePackage(string id, int code, string message, string content)
        {
            SendPackage(new CommandResponsePackage()
            {
                Id = id,
                Code = code,
                Message = message,
                Content = content
            }).Wait();
        }
    }
}
