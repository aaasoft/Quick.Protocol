using Quick.Protocol;
using System;
using System.Net;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            var server = new QpServer(new QpServerOptions()
            {
                Address = IPAddress.Loopback,
                Port = 3011,
                Password = "HelloQP",
                ServerProgram = nameof(ServerTest) + " 1.0"
            });
            server.ChannelConnected += Server_ChannelConnected;
            server.ChannelAuchenticated += Server_ChannelAuchenticated;
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
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.EndPoint}]已连接!");
        }

        private static void Server_ChannelAuchenticated(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.EndPoint}]已通过认证!");
        }

        private static void Server_ChannelDisconnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.EndPoint}]已断开!");
        }
    }
}
