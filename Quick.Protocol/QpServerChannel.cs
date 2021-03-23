﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Quick.Protocol
{
    public class QpServerChannel : QpCommandHandler
    {
        private QpServer server;
        private Stream stream;
        private CancellationToken cancellationToken;
        private QpServerOptions options;
        private string question;
        private bool isAuthSuccess = false;
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
            //this.CommandReceived += QpServerChannel_CommandReceived;

            //修改缓存大小
            ChangeBufferSize(options.BufferSize);

            InitQpPackageHandler_Stream(stream);
            //开始读取其他数据包
            BeginReadPackage(cancellationToken);
        }

        public void Start()
        {
            //var welcomeCmd = new Commands.WelcomeCommand(new Commands.WelcomeCommand.CommandContent()
            //{
            //    ProtocolVersion = QpConsts.QUICK_PROTOCOL_VERSION,
            //    ServerProgram = options.ServerProgram,
            //    BufferSize = options.BufferSize,
            //    InstructionSet = options.InstructionSet
            //});
            //question = welcomeCmd.Id;

            ////发送欢迎指令
            //var ret = SendCommand(welcomeCmd).Result;

            //if (ret.Code != 0)
            //    throw new IOException(ret.Message);

            //开始心跳
            if (options.HeartBeatInterval > 0)
                BeginHeartBeat(cancellationToken);
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

        //private void QpServerChannel_CommandReceived(object sender, Commands.ICommand e)
        //{
        //    if (e == null)
        //        return;
        //    var authCmd = e as Commands.AuthenticateCommand;
        //    if (authCmd == null)
        //    {
        //        if (!isAuthSuccess)
        //        {
        //            OnReadError(new IOException("No authenticated"));
        //        }
        //        return;
        //    }

        //    var authCmdContent = authCmd.ContentT;
        //    if (Utils.CryptographyUtils.ComputeMD5Hash(question + options.Password) != authCmdContent.Answer)
        //    {
        //        SendCommandResponse(e, -1, "认证失败！").ContinueWith(t =>
        //        {
        //            OnReadError(new IOException("认证失败！"));
        //        });
        //        return;
        //    }

        //    SendCommandResponse(e, 0, "认证通过！").ContinueWith(t =>
        //    {
        //        isAuthSuccess = true;
        //        options.InternalCompress = authCmdContent.Compress;
        //        options.InternalEncrypt = authCmdContent.Encrypt;
        //        options.InternalTransportTimeout = authCmdContent.TransportTimeout;
        //        Auchenticated?.Invoke(this, EventArgs.Empty);

        //        //改变传输超时时间
        //        ChangeTransportTimeout();
                
        //    });
        //}

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