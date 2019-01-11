using Newtonsoft.Json;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpClient
    {
        private CancellationTokenSource cts = null;
        private QpClientOptions options;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1 * 1024 * 1024];
        /// <summary>
        /// 服务端欢迎信息
        /// </summary>
        public WelcomeCommand.CommandContent WelcomeContent { get; private set; }
        /// <summary>
        /// 收到数据包事件
        /// </summary>
        public event EventHandler<IPackage> PackageReceived;
        /// <summary>
        /// 连接已经断开事件
        /// </summary>
        public event EventHandler Disconnected;

        public QpClient(QpClientOptions options)
        {
            options.Check();
            this.options = options;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public async Task ConnectAsync()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (tcpClient != null)
                Close();
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = options.SendTimeout;
            tcpClient.ReceiveTimeout = options.ReceiveTimeout;
            await tcpClient.ConnectAsync(options.Host, options.Port);
            stream = tcpClient.GetStream();

            //读取服务端发来的欢迎信息
            var welcomePackage = await readPackageAsync(token) as CommandRequestPackage;
            if (welcomePackage == null || string.IsNullOrEmpty(welcomePackage.Content))
                throw new IOException("Could't read welcome command,protocol error.");
            //验证指令集
            var welcomeCommand = welcomePackage.ToCommand<WelcomeCommand, WelcomeCommand.CommandContent, object>();
            WelcomeContent = welcomeCommand.Content;
            if (WelcomeContent == null)
                throw new IOException("WelcomContent is null,protocol error.");
            var notSupportInstructionNames = options.NeededInstructionSet.Except(WelcomeContent.InstructionSet.Select(t => t.Id)).ToArray();
            if (notSupportInstructionNames.Length > 0)
                throw new IOException($"Client need instruction[{string.Join(",", notSupportInstructionNames)}] not support by server.");

            //开始读取其他数据包
            beginReadPackage(token);
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <typeparam name="TRequestContent"></typeparam>
        /// <typeparam name="TResponseData"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public CommandResponse<TResponseData> SendCommand<TRequestContent, TResponseData>(AbstractCommand<TRequestContent, TResponseData> command)
        {
            lock (this)
            {
                System.Threading.Mutex mutex = new System.Threading.Mutex();
                CommandResponsePackage responsePackage = null;
                EventHandler<IPackage> packageHandler = null;
                packageHandler = (sender, package) =>
                 {
                     if (package is null)
                         return;
                     if (package is CommandResponsePackage)
                     {
                         PackageReceived -= packageHandler;
                         responsePackage = (CommandResponsePackage)package;
                         mutex.ReleaseMutex();
                     }
                 };
                PackageReceived += packageHandler;

                var request = new CommandRequestPackage()
                {
                    Action = command.Action,
                    Content = JsonConvert.SerializeObject(command.Content)
                };
                var requestBuffer = request.Output();
                stream.Write(requestBuffer, 0, requestBuffer.Length);
                //等待响应包
                mutex.WaitOne();
                if (responsePackage == null)
                    return null;
                return new CommandResponse<TResponseData>()
                {
                    Code = responsePackage.Code,
                    Message = responsePackage.Message,
                    Data = string.IsNullOrEmpty(responsePackage.Content) ? default(TResponseData) : JsonConvert.DeserializeObject<TResponseData>(responsePackage.Content)
                };
            }
        }

        private void beginReadPackage(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            readPackageAsync(token).ContinueWith(t =>
            {
                //如果已经取消
                if (t.IsCanceled)
                    return;

                //如果读取出错
                if (t.IsFaulted)
                {
                    onDisconnected();
                    return;
                }
                try
                {
                    var package = t.Result;
                    PackageReceived?.Invoke(this, package);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //读取下一个数据包
                    beginReadPackage(token);
                }
            });
        }

        private async Task<IPackage> readPackageAsync(CancellationToken token)
        {
            int ret = 0;
            //读取包头
            ret = await stream.ReadAsync(buffer, 0, 5, token);
            if (ret < 5)
                throw new IOException($"包头读取错误！读取数据长度：{ret}");

            var packageLength = BitConverter.ToInt32(buffer, 0);
            var packageType = buffer[4];
            //读取包体
            ret = await stream.ReadAsync(buffer, 0, packageLength);
            if (ret < packageLength)
                throw new IOException($"包体读取错误！包长度：{packageLength}，包类型：{packageType}，读取数据长度：{ret}");

            //解析包
            var package = options.ParsePackage(packageType, buffer, 0, packageLength);
            return package;
        }

        private void onDisconnected()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }
    }
}
