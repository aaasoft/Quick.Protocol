using Quick.Protocol.Core;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            Quick.Protocol.Utils.LogUtils.LogPackage = true;
            Quick.Protocol.Utils.LogUtils.LogHeartbeat = true;

            var client = new Quick.Protocol.WebSocket.Client.QpWebSocketClient(new Quick.Protocol.WebSocket.Client.QpWebSocketClientOptions()
            {
                Url = "ws://127.0.0.1:3011/qp_test",
                Password = "HelloQP"
            });
            client.CommandReceived += Client_CommandReceived;
            client.Disconnected += (sender, e) =>
            {
                Console.WriteLine("连接已断开");
            };
            client.ConnectAsync().ContinueWith(async t =>
            {
                if (t.IsCanceled)
                {
                    Console.WriteLine("连接已取消");
                    return;
                }
                if (t.IsFaulted)
                {
                    Console.WriteLine("连接出错，原因：" + t.Exception.InnerException.ToString());
                    return;
                }
                Console.WriteLine("连接成功");
                Console.WriteLine("准备发送未知命令。。。");
                try
                {
                    var rep = await client.SendCommand(new Quick.Protocol.Commands.UnknownCommand(
                        new Quick.Protocol.Commands.UnknownCommand.CommandContent()
                        {
                            Message = string.Empty.PadRight(5 * 1024 * 1024)
                        }), 5 * 1000);
                    Console.WriteLine("发送未知命令成功！");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("发送未知命令出错，原因：" + ex.Message);
                }
            });
            Console.ReadLine();
        }

        private static void Client_CommandReceived(object sender, Quick.Protocol.Commands.ICommand e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 收到指令[{e.GetType().FullName}]!");
        }
    }
}
