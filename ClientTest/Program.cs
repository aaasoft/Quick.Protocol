using Quick.Protocol;
using System;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            var client = new QpClient(new QpClientOptions()
            {
                Host = "127.0.0.1",
                Port = 3011,
                Password = "HelloQP",
                //EnableCompress = true,
                //EnableEncrypt = true,
                NeededInstructionSet = new[] { "Quick.Protocol.Base" }
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
