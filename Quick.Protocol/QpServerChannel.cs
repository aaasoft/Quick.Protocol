﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpServerChannel : QpCommandHandler
    {
        private QpServer server;
        private Stream stream;
        private CancellationToken cancellationToken;
        private QpServerOptions options;
        private string question;
        //通过认证后，才允许使用的命令执行管理器列表
        private List<CommandExecuterManager> authedCommandExecuterManagerList = null;

        public string ChannelName { get; private set; }

        /// <summary>
        /// 通过认证时
        /// </summary>
        public event EventHandler Auchenticated;

        /// <summary>
        /// 连接断开时
        /// </summary>
        public event EventHandler Disconnected;

        public QpServerChannel(QpServer server, Stream stream, string channelName, CancellationToken cancellationToken, QpServerOptions options) : base(options)
        {
            this.server = server;
            this.stream = stream;
            this.ChannelName = channelName;
            this.cancellationToken = cancellationToken;
            this.options = options;
            this.authedCommandExecuterManagerList = options.CommandExecuterManagerList;

            //修改缓存大小
            ChangeBufferSize(options.BufferSize);

            InitQpPackageHandler_Stream(stream);
            //开始读取其他数据包
            BeginReadPackage(cancellationToken);
        }

        private Commands.Connect.Response connect(Commands.Connect.Request request)
        {
            question = Guid.NewGuid().ToString("N");
            return new Commands.Connect.Response()
            {
                BufferSize = options.BufferSize,
                Question = question
            };
        }

        private Commands.Authenticate.Response authenticate(Commands.Authenticate.Request request)
        {
            if (Utils.CryptographyUtils.ComputeMD5Hash(question + options.Password) != request.Answer)
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    Stop();
                });
                throw new CommandException(1, "认证失败！");
            }

            options.CommandExecuterManagerList = authedCommandExecuterManagerList;
            options.InternalCompress = request.EnableCompress;
            options.InternalEncrypt = request.EnableEncrypt;
            options.InternalTransportTimeout = request.TransportTimeout;
            Auchenticated?.Invoke(this, EventArgs.Empty);

            //改变传输超时时间
            ChangeTransportTimeout();

            //开始心跳
            if (options.HeartBeatInterval > 0)
                BeginHeartBeat(cancellationToken);
            return new Commands.Authenticate.Response();
        }


        public void Start()
        {
            var connectAndAuthCommandExecuterManager = new CommandExecuterManager();
            connectAndAuthCommandExecuterManager.Register<Commands.Connect.Request, Commands.Connect.Response>(connect);
            connectAndAuthCommandExecuterManager.Register<Commands.Authenticate.Request, Commands.Authenticate.Response>(authenticate);
            options.CommandExecuterManagerList = new List<CommandExecuterManager>() { connectAndAuthCommandExecuterManager };
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            try 
            {
                stream?.Close();
                stream?.Dispose();
            } catch { }
        }

        protected override void OnReadError(Exception exception)
        {
            if (options.ProtocolErrorHandler != null)
            {
                if (exception is ProtocolException)
                {
                    var protocolException = (ProtocolException)exception;
                    server.RemoveChannel(this);
                    options.ProtocolErrorHandler.Invoke(stream, protocolException.ReadBuffer);
                    return;
                }
            }
            base.OnReadError(exception);
            Disconnected?.Invoke(this, QpEventArgs.Empty);
        }
    }
}
