using Quick.Protocol;
using System;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new QpClient(new QpClientOptions()
            {
                Host = "127.0.0.1",
                Port = 3011,
                Password = "HelloQP",
                Compress = true,
                Encrypt = true,
                NeededInstructionSet = new[] { "Quick.Protocol.Base" },
                SendTimeout = 5000,
                ReceiveTimeout = 5000,
                SupportPackages = new Quick.Protocol.Packages.IPackage[]{
                    Quick.Protocol.Packages.HeartBeatPackage.Instance,
                    new Quick.Protocol.Packages.CommandRequestPackage(),
                    new Quick.Protocol.Packages.CommandResponsePackage()
                },
                SupportCommands = new Quick.Protocol.Commands.ICommand[]
                {
                    new Quick.Protocol.Commands.WelcomeCommand(),
                    new Quick.Protocol.Commands.AuthenticateCommand()
                }
            });
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
