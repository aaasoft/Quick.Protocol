using Quick.Protocol;
using System;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
小数据量测试
------------------
加密  压缩  数据包大小
true  true  144
true  false 128
false true  135
false false 122

大数据量测试
------------------
加密  压缩  数据包大小
true  true  2880
true  false 5560
false true  2876
false false 5557
             */
            var client = new QpClient(new QpClientOptions()
            {
                Host = "127.0.0.1",
                Port = 3011,
                Password = "HelloQP",
                EnableCompress = true,
                EnableEncrypt = true,
                NeededInstructionSet = new[] { "Quick.Protocol.Base" },
                SendTimeout = 5000,
                ReceiveTimeout = 5000
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
                client.SendCommand(new Quick.Protocol.Commands.WelcomeCommand(new Quick.Protocol.Commands.WelcomeCommand.CommandContent()
                {
                    ProtocolVersion = "1.1",
                    ServerProgram = nameof(ClientTest)
                }));
            });
            Console.ReadLine();
        }
    }
}
