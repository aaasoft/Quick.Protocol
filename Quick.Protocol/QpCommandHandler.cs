﻿using Microsoft.Extensions.Logging;
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
        private static readonly ILogger logger = LogUtils.GetCurrentClassLogger();

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
                        commandRequestTypeDict[item.RequestTypeName] = item.RequestType;
                        commandResponseTypeDict[item.ResponseTypeName] = item.ResponseType;
                        commandRequestTypeResponseTypeDict[item.RequestType] = item.ResponseType;
                    }
                }
            }
        }

        public async Task<TCmdResponse> SendCommand<TCmdRequest, TCmdResponse>(TCmdRequest request, int timeout = 30 * 1000, Action afterSendHandler = null)
        {
            var typeName = typeof(TCmdRequest).FullName;
            var requestContent = JsonConvert.SerializeObject(request);

            var commandContext = new CommandContext(typeName);
            commandDict.TryAdd(commandContext.Id, commandContext);

            if (timeout <= 0)
            {
                SendCommandRequestPackage(commandContext.Id, typeName, requestContent, afterSendHandler);
                return (TCmdResponse)await commandContext.ResponseTask;
            }
            //如果设置了超时
            else
            {
                try
                {
                    await TaskUtils.TaskWait(Task.Run(() => SendCommandRequestPackage(commandContext.Id, typeof(TCmdRequest).FullName, requestContent, afterSendHandler)), timeout);
                }
                catch
                {
                    if (LogUtils.LogCommand)
                        logger.LogTrace("{0}: [Send-CommandRequestPackage-Timeout]CommandId:{1},Type:{2},Content:{3}", DateTime.Now, commandContext.Id, typeName, LogUtils.LogContent ? requestContent : LogUtils.NOT_SHOW_CONTENT_MESSAGE);

                    if (commandContext.ResponseTask.Status == TaskStatus.Created)
                    {
                        commandContext.Timeout();
                        commandDict.TryRemove(commandContext.Id, out _);
                    }
                }
                return (TCmdResponse)await await TaskUtils.TaskWait(commandContext.ResponseTask, timeout);
            }
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

            if (options.CommandExecuterManagerList == null || options.CommandExecuterManagerList.Count == 0)
                return;
            try
            {
                foreach (var commandExecuterManager in options.CommandExecuterManagerList)
                {
                    if (commandExecuterManager.CanExecuteCommand(typeName))
                    {
                        var responseModel = commandExecuterManager.ExecuteCommand(typeName, contentModel);
                        SendCommandResponsePackage(commandId, 0, null, cmdResponseType.FullName, JsonConvert.SerializeObject(responseModel));
                        break;
                    }
                }
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
            object contentModel = null;
            if (code == 0)
            {
                //如果在字典中未找到此类型名称，则直接返回
                if (!commandResponseTypeDict.ContainsKey(typeName))
                    return;
                contentModel = JsonConvert.DeserializeObject(content, commandResponseTypeDict[typeName]);
            }

            CommandResponsePackageReceived?.Invoke(this, new CommandResponsePackageReceivedEventArgs()
            {
                CommandId = commandId,
                Code = code,
                Message = message,
                TypeName = typeName,
                ContentModel = contentModel
            });
            //设置指令响应
            CommandContext commandContext;
            if (!commandDict.TryRemove(commandId, out commandContext))
                return;
            if (code == 0)
                commandContext.SetResponse(contentModel);
            else
                commandContext.SetResponse(new CommandException(code, message));
        }
    }
}
