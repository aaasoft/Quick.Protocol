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

namespace Quick.Protocol
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
        /// 发送指令
        /// </summary>
        /// <typeparam name="TRequestContent"></typeparam>
        /// <typeparam name="TResponseData"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<CommandResponse<TResponseData>> SendCommand<TRequestContent, TResponseData>(AbstractCommand<TRequestContent, TResponseData> command)
        {
            commandDict.TryAdd(command.Id, command);
            var request = new CommandRequestPackage()
            {
                Id = command.Id,
                Action = command.Action,
                Content = JsonConvert.SerializeObject(command.Content)
            };
            SendPackage(request);
            return command.ResponseTask;
        }
    }
}
