using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quick.Protocol.Packages;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public virtual string Action => this.GetType().FullName;
        public abstract object Content { get; set; }

        public abstract ICommand Parse(CommandRequestPackage package);
        public abstract void SetResponse(CommandResponsePackage responsePackage);
    }

    public abstract class AbstractCommand<TRequestContent, TResponseData> : AbstractCommand
        where TRequestContent : class
        where TResponseData : class
    {
        private static readonly ILogger logger = LogUtils.GetCurrentClassLogger();

        public override object Content
        {
            get { return ContentT; }
            set
            {
                ContentT = (TRequestContent)value;
            }
        }
        public TRequestContent ContentT { get; set; }

        private bool isTimeout = false;
        private CommandResponse<TResponseData> response;
        public Task<CommandResponse<TResponseData>> ResponseTask { get; private set; }

        public AbstractCommand()
        {
            ResponseTask = new Task<CommandResponse<TResponseData>>(() =>
            {
                if (isTimeout)
                    throw new TimeoutException($"Command[{this.ToString()}] is timeout.");
                return response;
            });
        }

        public void Timeout()
        {
            isTimeout = true;
            if (ResponseTask.Status == TaskStatus.Created)
                ResponseTask.Start();
        }

        public AbstractCommand(TRequestContent content)
            : this()
        {
            ContentT = content;
        }

        /// <summary>
        /// 设置响应
        /// </summary>
        /// <param name="response"></param>
        public override void SetResponse(CommandResponsePackage responsePackage)
        {
            if (isTimeout)
                return;
            TResponseData responseData;
            if (string.IsNullOrEmpty(responsePackage.Content))
                responseData = default(TResponseData);
            else
            {
                if (typeof(TResponseData) == typeof(string))
                    responseData = responsePackage.Content as TResponseData;
                else
                {
                    Stopwatch stopwatch = null;
                    if (LogUtils.LogCommand)
                    {
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                    }
                    responseData = JsonConvert.DeserializeObject<TResponseData>(responsePackage.Content);
                    if (LogUtils.LogCommand)
                    {
                        stopwatch.Stop();
                        logger.LogTrace("[Parse-CommandResp]Id:{0} Code:{1} Message:{2} UseTime:{3}", responsePackage.Id, responsePackage.Code, responsePackage.Message, stopwatch.Elapsed);
                    }
                }
            }
            this.response = new CommandResponse<TResponseData>()
            {
                Code = responsePackage.Code,
                Message = responsePackage.Message,
                Data = responseData
            };
            if (ResponseTask.Status == TaskStatus.Created)
                ResponseTask.Start();
        }

        public override ICommand Parse(CommandRequestPackage package)
        {
            var cmd = Activator.CreateInstance(this.GetType()) as ICommand;

            if (cmd.Action != Action)
                throw new IOException($"Action not match.Package's Action is '{Action}' and Command's Action is '{cmd.Action}'");
            cmd.Id = package.Id;
            if (!string.IsNullOrEmpty(package.Content))
                cmd.Content = JsonConvert.DeserializeObject<TRequestContent>(package.Content);
            return cmd;
        }
    }
}
