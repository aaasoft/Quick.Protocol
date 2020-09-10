using Microsoft.AspNetCore.Builder;
using Quick.Protocol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Core
{
    public class QpTestContext
    {
        private Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServer server;

        public QpTestContext(IApplicationBuilder app)
        {
            Quick.Protocol.Utils.LogUtils.LogHeartbeat = false;
            Quick.Protocol.Utils.LogUtils.AddConsole();
            app.UseQuickProtocol(new Quick.Protocol.WebSocket.Server.AspNetCore.QpWebSocketServerOptions()
            {
                Path = "qp_test",
                BufferSize = 128,
                Password = "HelloQP",
                ServerProgram = nameof(WebApplication1) + " 1.0"
            }, out server);

            server.ChannelConnected += Server_ChannelConnected;
            server.ChannelDisconnected += Server_ChannelDisconnected;
        }

        private static void Server_ChannelConnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.ChannelName}]已连接!");
            e.CommandReceived += E_CommandReceived;
            //e.Auchenticated += E_Auchenticated;
        }

        private static void E_Auchenticated(object sender, EventArgs e)
        {
            var channel = (QpServerChannel)sender;
            Console.WriteLine("准备发送未知命令。。。");
            try
            {
                channel.SendCommand(new Quick.Protocol.Commands.UnknownCommand(
                    new Quick.Protocol.Commands.UnknownCommand.CommandContent()
                    {
                        Message = string.Empty.PadRight(5 * 1024 * 1024)
                    }), 5 * 1000).Wait();
                Console.WriteLine("发送未知命令成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送未知命令出错，原因：" + ex.Message);
            }
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
