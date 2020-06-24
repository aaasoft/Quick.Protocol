using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quick.Protocol.Commands;
using Quick.Protocol.Packages;
using Quick.Protocol.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol.Core
{
    public abstract class QpCommandHandler : QpPackageHandler
    {
        public const string RESPONSE_MESSAGE_OK = "OK";

        private static readonly ILogger logger = LogUtils.GetCurrentClassLogger();
        private ConcurrentDictionary<string, ICommand> commandDict = new ConcurrentDictionary<string, ICommand>();
        private QpCommandHandlerOptions options;
        /// <summary>
        /// 指令执行器
        /// </summary>
        public ICommandExecuter CommandExecuter { get; set; }
        protected QpCommandHandler(QpCommandHandlerOptions options)
            : base(options)
        {
            this.options = options;
        }

        /// <summary>
        /// 收到指令
        /// </summary>
        public event EventHandler<ICommand> CommandReceived;

        protected override void OnPackageReceived(IPackage package)
        {
            base.OnPackageReceived(package);
            //如果是指令请求包
            if (package is CommandRequestPackage)
            {
                var requestPackage = (CommandRequestPackage)package;
                var requestCmd = options.ParseCommand(requestPackage);
                if (requestCmd == null)
                    requestCmd = UnknownCommand.Instance.Parse(requestPackage);
                if (LogUtils.LogCommand)
                    logger.LogTrace("[Recv-Command]Action:{0} Content:{1}", requestCmd.Action, LogUtils.LogCommandContent ? requestCmd.Content : "...,ContentType:" + requestCmd.Content.GetType().FullName);
                CommandExecuter?.Execute(this, requestCmd);
                CommandReceived?.Invoke(this, requestCmd);
            }
            //如果是指令响应包
            else if (package is CommandResponsePackage)
            {
                var responsePackage = (CommandResponsePackage)package;
                OnReceivedCommandResponse(responsePackage);
                if (LogUtils.LogCommand)
                    logger.LogTrace("[Recv-CommandResp]Id:{0} Code:{1} Content:{2}", responsePackage.Id, responsePackage.Code, LogUtils.LogCommandContent ? responsePackage.Content : "...,ContentType:" + responsePackage.Content.GetType().FullName);
            }
        }
        protected virtual void OnReceivedCommandResponse(CommandResponsePackage package)
        {
            if (string.IsNullOrEmpty(package.Id))
                return;
            ICommand cmd = null;
            if (!commandDict.TryGetValue(package.Id, out cmd))
                return;
            cmd.SetResponse(package);
            commandDict.TryRemove(package.Id, out cmd);
        }

        /// <summary>
        /// 发送指令，默认超时时间为30秒
        /// </summary>
        /// <typeparam name="TRequestContent"></typeparam>
        /// <typeparam name="TResponseData"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<CommandResponse<TResponseData>> SendCommand<TRequestContent, TResponseData>(AbstractCommand<TRequestContent, TResponseData> command)
            where TRequestContent : class
            where TResponseData : class
        {
            return SendCommand(command, 30 * 1000);
        }

        public Task<CommandResponsePackage> SendCommand(ICommand command)
        {
            return SendCommand(command, 30 * 1000);
        }

        public async Task<CommandResponsePackage> SendCommand(ICommand command, int timeout)
        {
            if (LogUtils.LogCommand)
                logger.LogTrace("[Send-Command]Action:{0} Id:{1} Content:{2}", command.Action, command.Id, LogUtils.LogCommandContent ? command.Content : "...,ContentType:" + command.Content.GetType().FullName);

            commandDict.TryAdd(command.Id, command);
            var request = new CommandRequestPackage()
            {
                Id = command.Id,
                Action = command.Action,
                Content = JsonConvert.SerializeObject(command.Content)
            };

            if (timeout <= 0)
            {
                SendPackage(request);
                return await command.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(Task.Run(() => SendPackage(request)), timeout);
                }
                catch
                {
                    if (LogUtils.LogCommand)
                        logger.LogTrace("[Send-Command-Timeout]Action:{0} Id:{1} Content:{2}", command.Action, command.Id, LogUtils.LogCommandContent ? command.Content : "...,ContentType:" + command.Content.GetType().FullName);
                    if (command.ResponseTask.Status == TaskStatus.Created)
                    {
                        command.Timeout();
                        commandDict.TryRemove(command.Id, out _);
                    }
                }
                return await await TaskUtils.TaskWait(command.ResponseTask, timeout);
            }
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <typeparam name="TRequestContent"></typeparam>
        /// <typeparam name="TResponseData"></typeparam>
        /// <param name="command"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<CommandResponse<TResponseData>> SendCommand<TRequestContent, TResponseData>(AbstractCommand<TRequestContent, TResponseData> command, int timeout)
            where TRequestContent : class
            where TResponseData : class
        {
            if (LogUtils.LogCommand)
                logger.LogTrace("[Send-Command]Action:{0} Id:{1} Content:{2}", command.Action, command.Id, LogUtils.LogCommandContent ? command.Content : "...,ContentType:" + command.Content.GetType().FullName);

            commandDict.TryAdd(command.Id, command);
            var request = new CommandRequestPackage()
            {
                Id = command.Id,
                Action = command.Action,
                Content = JsonConvert.SerializeObject(command.Content)
            };

            if (timeout <= 0)
            {
                SendPackage(request);
                return await command.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(Task.Run(() => SendPackage(request)), timeout);
                }
                catch
                {
                    if (LogUtils.LogCommand)
                        logger.LogTrace("[Send-Command-Timeout]Action:{0} Id:{1} Content:{2}", command.Action, command.Id, LogUtils.LogCommandContent ? command.Content : "...,ContentType:" + command.Content.GetType().FullName);
                    if (command.ResponseTask.Status == TaskStatus.Created)
                    {
                        command.Timeout();
                        commandDict.TryRemove(command.Id, out _);
                    }
                }
                return await await TaskUtils.TaskWait(command.ResponseTask, timeout);
            }
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendCommandResponse(ICommand cmd, int code, string message)
        {
            return SendCommandResponse(cmd.Id, code, message);
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendCommandResponse(string commandId, int code, string message)
        {
            return SendCommandResponse(commandId, code, message, null);
        }

        public Task SendCommandResponse(ICommand cmd, int code, string message, string content)
        {
            return SendCommandResponse(cmd, code, message, content);
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task SendCommandResponse(string commandId, int code, string message, string content)
        {
            if (LogUtils.LogCommand)
                logger.LogTrace("[Send-Command-Resp]Id:{0} Code:{1} Message:{2} Content:{3}", commandId, code, message, LogUtils.LogCommandContent ? content : $"...,ContentType:string {content?.Length}");
            return Task.Run(() => SendPackage(new CommandResponsePackage()
            {
                Id = commandId,
                Code = code,
                Message = message,
                Content = content
            }));
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="code"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task SendCommandResponse(ICommand cmd, int code, object content)
        {
            return SendCommandResponse(cmd, code, RESPONSE_MESSAGE_OK, content);
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task SendCommandResponse(ICommand cmd, int code, string message, object content)
        {
            return SendCommandResponse(cmd.Id, code, message, content);
        }

        /// <summary>
        /// 发送指令响应
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task SendCommandResponse(string commandId, int code, string message, object content)
        {
            if (LogUtils.LogCommand)
                logger.LogTrace("[Send-Command-Resp]Id:{0} Code:{1} Message:{2} Content:{3}", commandId, code, message, LogUtils.LogCommandContent ? content : $"...,ContentType {content.GetType().FullName}");
            return Task.Run(() => SendPackage(new CommandResponsePackage()
            {
                Id = commandId,
                Code = code,
                Message = message,
                Content = content == null ? null : JsonConvert.SerializeObject(content)
            }));
        }
    }
}
