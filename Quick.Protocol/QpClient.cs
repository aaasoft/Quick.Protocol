using Newtonsoft.Json;
using Quick.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public class QpClient
    {
        private QpClientOptions options;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1 * 1024 * 1024];
        /// <summary>
        /// 收到数据包事件
        /// </summary>
        public event EventHandler<IPackage> PackageReceived;

        public QpClient(QpClientOptions options)
        {
            options.Check();
            this.options = options;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            if (tcpClient != null)
                Close();
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = options.SendTimeout;
            tcpClient.ReceiveTimeout = options.ReceiveTimeout;
            tcpClient.Connect(options.Host, options.Port);
            stream = tcpClient.GetStream();
            beginReadPackage();
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

        private void beginReadPackage()
        {
            Task.Run(() =>
            {
                int ret = 0;
                while (true)
                {
                    //读取包头
                    ret = stream.Read(buffer, 0, 5);
                    if (ret < 5)
                        throw new IOException($"包头读取错误！读取数据长度：{ret}");

                    var packageLength = BitConverter.ToInt32(buffer, 0);
                    var packageType = buffer[4];
                    //读取包体
                    ret = stream.Read(buffer, 0, packageLength);
                    if (ret < packageLength)
                        throw new IOException($"包体读取错误！包长度：{packageLength}，包类型：{packageType}，读取数据长度：{ret}");

                    //解析包
                    var package = options.ParsePackage(packageType, buffer, 0, packageLength);
                    PackageReceived?.Invoke(this, package);
                }
            }).ContinueWith(t =>
            {
                if(t.IsFaulted)
                {
                    onDisconnected();
                }
            });
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
            onDisconnected();
        }
    }
}
