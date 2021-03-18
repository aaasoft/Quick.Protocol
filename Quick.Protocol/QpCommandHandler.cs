using Newtonsoft.Json;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Protocol
{
    public abstract class QpCommandHandler : QpPackageHandler
    {
        private QpCommandHandlerOptions options;

        private Dictionary<string, Type> commandRequestTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, Type> commandResponseTypeDict = new Dictionary<string, Type>();
        private Dictionary<Type, Type> commandRequestTypeResponseTypeDict = new Dictionary<Type, Type>();

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

            if (options.CommandExecuterManager == null)
                return;
            try
            {
                var responseModel = options.CommandExecuterManager.ExecuteCommand(typeName, contentModel);
                SendCommandResponsePackage(commandId, 0, null, cmdResponseType.FullName, JsonConvert.SerializeObject(responseModel));
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
            //如果在字典中未找到此类型名称，则直接返回
            if (!commandResponseTypeDict.ContainsKey(typeName))
                return;
            var contentModel = JsonConvert.DeserializeObject(content, commandResponseTypeDict[typeName]);
            CommandResponsePackageReceived?.Invoke(this, new CommandResponsePackageReceivedEventArgs()
            {
                CommandId = commandId,
                Code = code,
                Message = message,
                TypeName = typeName,
                ContentModel = contentModel
            });

        }
    }
}
