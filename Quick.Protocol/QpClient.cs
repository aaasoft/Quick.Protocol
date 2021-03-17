using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public abstract class QpClient : QpPackageHandler
    {
        private CancellationTokenSource cts = null;
        public QpClientOptions Options { get; private set; }
        private bool authPassed = false;

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

        protected abstract Task<Stream> InnerConnectAsync();

        /// <summary>
        /// 连接
        /// </summary>
        public async Task ConnectAsync()
        {
            //清理
            Close();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            var stream = await InnerConnectAsync();

            //初始化网络
            InitQpPackageHandler_Stream(stream);

            ////读取服务端发来的欢迎信息
            //var welcomePackage = await ReadPackageAsync(token) as CommandRequestPackage;
            //if (welcomePackage == null || string.IsNullOrEmpty(welcomePackage.Content))
            //    throw new InvalidDataException("Could't read welcome command,protocol error.");

            ////转为欢迎指令
            //var welcomeCommand = welcomePackage.ToCommand<WelcomeCommand, WelcomeCommand.CommandContent, object>();
            //WelcomeContent = welcomeCommand.ContentT;
            //if (WelcomeContent == null)
            //{
            //    var errorMessage = "WelcomContent is null,protocol error.";
            //    await SendCommandResponse(welcomeCommand, -1, errorMessage);
            //    throw new InvalidDataException(errorMessage);
            //}

            ////设置缓存大小为与服务端同样的大小
            //ChangeBufferSize(WelcomeContent.BufferSize);

            ////验证指令集
            //var notSupportInstructionNames = Options.InstructionSet.Select(t => t.Id).Except(WelcomeContent.InstructionSet.Select(t => t.Id)).ToArray();
            //if (notSupportInstructionNames.Length > 0)
            //{
            //    var errorMessage = $"Client need instruction[{string.Join(",", notSupportInstructionNames)}] not support by server.";
            //    await SendCommandResponse(welcomeCommand, -1, errorMessage);
            //    throw new InstructionNotSupportException(errorMessage);
            //}
            //await SendCommandResponse(welcomeCommand, 0, "OK");

            //开始读取其他数据包
            BeginReadPackage(token);

            ////开始认证
            //var authenticateResult = await SendCommand(new AuthenticateCommand(new AuthenticateCommand.CommandContent()
            //{
            //    Compress = Options.EnableCompress,
            //    Encrypt = Options.EnableEncrypt,
            //    TransportTimeout = Options.TransportTimeout,
            //    Answer = CryptographyUtils.ComputeMD5Hash(welcomeCommand.Id + Options.Password)
            //}));

            //if (authenticateResult.Code != 0)
            //{
            //    var exception = new AuthenticateFailedException(authenticateResult.Message);
            //    Close();
            //    throw exception;
            //}
            //authPassed = true;
            //Options.OnAuthPassed();
            //开始心跳
            if (Options.HeartBeatInterval > 0)
            {
                //定时发送心跳包
                BeginHeartBeat(token);
            }
        }

        protected override void OnReadError(Exception exception)
        {
            base.OnReadError(exception);
            Options.Init();
            var tmpAuthPassed = authPassed;
            cancellAll();
            Disconnect();
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

        protected virtual void Disconnect()
        {
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            cancellAll();
            Disconnect();
        }
    }
}
