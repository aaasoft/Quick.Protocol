using Quick.Protocol;
using System;
using System.Net;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new QpServer(new QpServerOptions()
            {
                Address = IPAddress.Loopback,
                Port = 3011,
                Password = "HelloQP",
                SendTimeout = 5000,
                ReceiveTimeout = 5000,
                ServerProgram = nameof(ServerTest) + " 1.0"
            });
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
    }
}
