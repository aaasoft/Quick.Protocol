using Newtonsoft.Json;
using Quick.Protocol.Commands;
using Quick.Protocol.Packages;
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

        protected QpCommandHandler(QpPackageHandlerOptions packageHandlerOptions)
            : base(packageHandlerOptions)
        {
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
                CommandReceived?.Invoke(this, null);
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
            var request = new CommandRequestPackage()
            {
                Action = command.Action,
                Content = JsonConvert.SerializeObject(command.Content)
            };
            SendPackage(request);
            return command.ResponseTask;
        }
    }
}
