using Quick.Protocol;
using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            //var server = new Quick.Protocol.Tcp.QpTcpServer(new Quick.Protocol.Tcp.QpTcpServerOptions()
            //{
            //    Address = IPAddress.Loopback,
            //    Port = 3011,
            //    Password = "HelloQP",
            //    ServerProgram = nameof(ServerTest) + " 1.0"
            //});
            var server = new Quick.Protocol.Pipeline.QpPipelineServer(new Quick.Protocol.Pipeline.QpPipelineServerOptions()
            {
                PipeName = "Quick.Protocol",
                Password = "HelloQP",
                ServerProgram = nameof(ServerTest) + " 1.0"
            });
            server.ChannelConnected += Server_ChannelConnected;
            server.ChannelDisconnected += Server_ChannelDisconnected;
            try
            {
                server.Start();
                Console.WriteLine($"服务启动成功!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务启动失败!" + ex.ToString());
            }
            Console.ReadLine();
            server.Stop();
        }

        private static void Server_ChannelConnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.ChannelName}]已连接!");
            e.CommandReceived += E_CommandReceived;
        }

        private static void E_CommandReceived(object sender, Quick.Protocol.Commands.ICommand e)
        {
            QpServerChannel channel = (QpServerChannel)sender;
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{channel.ChannelName}]收到指令[{e.GetType().FullName}]!");
        }

        private static void Server_ChannelDisconnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.ChannelName}]已断开!");
        }
    }
}
