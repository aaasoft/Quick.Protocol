using System;

namespace TcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.LogHeartbeat = true;
            Quick.Protocol.Utils.LogUtils.LogPackage = true;
            Quick.Protocol.Utils.LogUtils.LogContent = true;
            Quick.Protocol.Utils.LogUtils.AddConsole();

            var client = new Quick.Protocol.Tcp.QpTcpClient(new Quick.Protocol.Tcp.QpTcpClientOptions()
            {
                Host = "127.0.0.1",
                Port = 3011,
                EnableCompress = true,
                EnableEncrypt = true,
                //TransportTimeout = 120 * 1000,
                Password = "HelloQP"
            });

            client.Disconnected += (sender, e) =>
            {
                Console.WriteLine("连接已断开");
            };
            client.ConnectAsync().ContinueWith(t =>
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
            });
            Console.ReadLine();
        }
    }
}
