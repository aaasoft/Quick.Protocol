using Quick.Protocol;
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
            //Quick.Protocol.Utils.LogUtils.AddConsole();
            var server = new QpTcpServer(new QpTcpServerOptions()
            {
                Address = IPAddress.Loopback,
                Port = 3011,
                Password = "HelloQP",
                InstructionSet = new QpInstruction[] { SoftCloud.Connector.Protocol.V1.Instruction.Instance },
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

            Task.Run(async () =>
            {
                while (true)
                {
                    List<SoftCloud.Connector.Protocol.V1.Packages.DataPackageItem> list = new List<SoftCloud.Connector.Protocol.V1.Packages.DataPackageItem>();
                    for (var i = 0; i < 20; i++)
                    {
                        list.Add(new SoftCloud.Connector.Protocol.V1.Packages.DataPackageItem()
                        {
                            ControllerId = "001_" + i,
                            DeviceId = "002_" + i,
                            PointId = "003_" + i,
                            Value = i.ToString()
                        });
                    }
                    await e.SendPackage(new SoftCloud.Connector.Protocol.V1.Packages.DataPackage()
                    {
                        Items = list.ToArray()
                    });
                    //await Task.Delay(10);
                }
            });
        }

        private static void E_CommandReceived(object sender, Quick.Protocol.Commands.ICommand e)
        {
            QpServerChannel channel = (QpServerChannel)sender;
            if (e is SoftCloud.Connector.Protocol.V1.Commands.SubscribeCommand)
                channel.SendCommandResponse(e, 0, "OK");
        }

        private static void Server_ChannelDisconnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.ChannelName}]已断开!");
        }
    }
}
