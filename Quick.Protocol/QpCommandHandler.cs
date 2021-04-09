using Newtonsoft.Json;
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
        private QpCommandHandlerOptions options;

        private Dictionary<string, Type> commandRequestTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, Type> commandResponseTypeDict = new Dictionary<string, Type>();
        private Dictionary<Type, Type> commandRequestTypeResponseTypeDict = new Dictionary<Type, Type>();

        private ConcurrentDictionary<string, CommandContext> commandDict = new ConcurrentDictionary<string, CommandContext>();

        /// <summary>
        /// 原始收到命令请求数据包事件
        /// </summary>
        public event EventHandler<RawCommandRequestPackageReceivedEventArgs> RawCommandRequestPackageReceived;
        /// <summary>
        /// 收到命令请求数据包事件
        /// </summary>
        public event EventHandler<CommandRequestPackageReceivedEventArgs> CommandRequestPackageReceived;
        /// <summary>
        /// 原始收到命令响应数据包事件
        /// </summary>
        public event EventHandler<RawCommandResponsePackageReceivedEventArgs> RawCommandResponsePackageReceived;
        /// <summary>
        /// 收到命令响应数据包事件
        /// </summary>
        public event EventHandler<CommandResponsePackageReceivedEventArgs> CommandResponsePackageReceived;

        protected QpCommandHandler(QpCommandHandlerOptions options) : base(options)
        {
            this.options = options;
            foreach (var instructionSet in options.InstructionSet)
            {
                //添加命令数据包信息
                if (instructionSet.CommandInfos != null && instructionSet.CommandInfos.Length > 0)
                {
                    foreach (var item in instructionSet.CommandInfos)
                    {
                        var requestType = item.GetRequestType();
                        var responseType = item.GetResponseType();
                        commandRequestTypeDict[item.RequestTypeName] = requestType;
                        commandResponseTypeDict[item.ResponseTypeName] = responseType;
                        commandRequestTypeResponseTypeDict[requestType] = responseType;
                    }
                }
            }
        }

        public async Task<CommandResponseTypeNameAndContent> SendCommand(string requestTypeName, string requestContent, int timeout = 30 * 1000, Action afterSendHandler = null)
        {
            var commandContext = new CommandContext(requestTypeName);
            commandDict.TryAdd(commandContext.Id, commandContext);

            if (timeout <= 0)
            {
                SendCommandRequestPackage(commandContext.Id, requestTypeName, requestContent, afterSendHandler);
                return await commandContext.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(Task.Run(() => SendCommandRequestPackage(commandContext.Id, requestTypeName, requestContent, afterSendHandler)), timeout);
                }
                catch
                {
                    if (LogUtils.LogCommand)
                        Console.WriteLine("{0}: [Send-CommandRequestPackage-Timeout]CommandId:{1},Type:{2},Content:{3}", DateTime.Now, commandContext.Id, requestTypeName, LogUtils.LogContent ? requestContent : LogUtils.NOT_SHOW_CONTENT_MESSAGE);

                    if (commandContext.ResponseTask.Status == TaskStatus.Created)
                    {
                        commandContext.Timeout();
                        commandDict.TryRemove(commandContext.Id, out _);
                    }
                }
                return await await TaskUtils.TaskWait(commandContext.ResponseTask, timeout);
            }
        }

        public async Task<TCmdResponse> SendCommand<TCmdResponse>(IQpCommandRequest<TCmdResponse> request, int timeout = 30 * 1000, Action afterSendHandler = null)
        {
            var requestType = request.GetType();
            var typeName = requestType.FullName;
            var requestContent = JsonConvert.SerializeObject(request);

            var commandContext = new CommandContext(typeName);
            commandDict.TryAdd(commandContext.Id, commandContext);

            CommandResponseTypeNameAndContent ret = null;
            if (timeout <= 0)
            {
                SendCommandRequestPackage(commandContext.Id, typeName, requestContent, afterSendHandler);
                ret = await commandContext.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(Task.Run(() => SendCommandRequestPackage(commandContext.Id, typeName, requestContent, afterSendHandler)), timeout);
                }
                catch
                {
                    if (LogUtils.LogCommand)
                        Console.WriteLine("{0}: [Send-CommandRequestPackage-Timeout]CommandId:{1},Type:{2},Content:{3}", DateTime.Now, commandContext.Id, typeName, LogUtils.LogContent ? requestContent : LogUtils.NOT_SHOW_CONTENT_MESSAGE);

                    if (commandContext.ResponseTask.Status == TaskStatus.Created)
                    {
                        commandContext.Timeout();
                        commandDict.TryRemove(commandContext.Id, out _);
                    }
                }
                ret = await await TaskUtils.TaskWait(commandContext.ResponseTask, timeout);
            }
            return JsonConvert.DeserializeObject<TCmdResponse>(ret.Content);
        }

        protected override void OnCommandRequestReceived(string commandId, string typeName, string content)
        {
            RawCommandRequestPackageReceived?.Invoke(this, new RawCommandRequestPackageReceivedEventArgs()
            {
                CommandId = commandId,
                TypeName = typeName,
                Content = content
            });
            //如果在字典中未找到此类型名称，则直接返回
            if (!commandRequestTypeDict.ContainsKey(typeName))
                return;

            var cmdRequestType = commandRequestTypeDict[typeName];
            var cmdResponseType = commandRequestTypeResponseTypeDict[cmdRequestType];

            var contentModel = JsonConvert.DeserializeObject(content, cmdRequestType);
            CommandRequestPackageReceived?.Invoke(this, new CommandRequestPackageReceivedEventArgs()
            {
                CommandId = commandId,
                TypeName = typeName,
                ContentModel = contentModel
            });

            try
            {
                var hasCommandExecuter = false;
                if (options.CommandExecuterManagerList != null)
                    foreach (var commandExecuterManager in options.CommandExecuterManagerList)
                    {
                        if (commandExecuterManager.CanExecuteCommand(typeName))
                        {
                            hasCommandExecuter = true;
                            var responseModel = commandExecuterManager.ExecuteCommand(typeName, contentModel);
                            SendCommandResponsePackage(commandId, 0, null, cmdResponseType.FullName, JsonConvert.SerializeObject(responseModel));
                            break;
                        }
                    }
                if (!hasCommandExecuter)
                    throw new CommandException(255, $"No CommandExecuter for RequestType:{typeName}");
            }
            catch (CommandException ex)
            {
                SendCommandResponsePackage(commandId, ex.Code, ExceptionUtils.GetExceptionMessage(ex), null, null);
            }
            catch (Exception ex)
            {
                SendCommandResponsePackage(commandId, 255, ExceptionUtils.GetExceptionMessage(ex), null, null);
            }
        }

        protected override void OnCommandResponseReceived(string commandId, byte code, string message, string typeName, string content)
        {
            RawCommandResponsePackageReceived?.Invoke(this, new RawCommandResponsePackageReceivedEventArgs()
            {
                CommandId = commandId,
                Code = code,
                Message = message,
                TypeName = typeName,
                Content = content
            });

            CommandResponsePackageReceived?.Invoke(this, new CommandResponsePackageReceivedEventArgs()
            {
                CommandId = commandId,
                Code = code,
                Message = message,
                TypeName = typeName,
                Content = content
            });
            //设置指令响应
            CommandContext commandContext;
            if (!commandDict.TryRemove(commandId, out commandContext))
                return;
            if (code == 0)
                commandContext.SetResponse(typeName, content);
            else
                commandContext.SetResponse(new CommandException(code, message));
        }
    }
}
