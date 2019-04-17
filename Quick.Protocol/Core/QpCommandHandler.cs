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
        private ConcurrentDictionary<string, AbstractCommand> commandDict = new ConcurrentDictionary<string, AbstractCommand>();
        private QpCommandHandlerOptions options;
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
                CommandReceived?.Invoke(this, requestCmd);
            }
            //如果是指令响应包
            else if (package is CommandResponsePackage)
            {
                var responsePackage = (CommandResponsePackage)package;
                OnReceivedCommandResponse(responsePackage);
            }
        }
        protected virtual void OnReceivedCommandResponse(CommandResponsePackage package)
        {
            if (string.IsNullOrEmpty(package.Id))
                return;
            AbstractCommand cmd = null;
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
            commandDict.TryAdd(command.Id, command);
            var request = new CommandRequestPackage()
            {
                Id = command.Id,
                Action = command.Action,
                Content = JsonConvert.SerializeObject(command.Content)
            };

            if (timeout <= 0)
            {
                await SendPackage(request);
                return await command.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(SendPackage(request), timeout);
                }
                catch (TimeoutException)
                {
                    if (command.ResponseTask.Status == TaskStatus.Created)
                    {
                        command.Timeout();
                        commandDict.TryRemove(command.Id, out _);
                    }
                }
                return await command.ResponseTask;
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
            return SendPackage(new CommandResponsePackage()
            {
                Id = commandId,
                Code = code,
                Message = message,
                Content = content
            });
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
            return SendPackage(new CommandResponsePackage()
            {
                Id = commandId,
                Code = code,
                Message = message,
                Content = JsonConvert.SerializeObject(content)
            });
        }
    }
}
