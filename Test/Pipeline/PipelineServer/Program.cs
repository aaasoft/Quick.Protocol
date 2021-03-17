﻿using Quick.Protocol;
using System;

namespace PipelineServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            Quick.Protocol.Utils.LogUtils.LogHeartbeat = true;
            Quick.Protocol.Utils.LogUtils.LogNotice = true;

            var server = new Quick.Protocol.Pipeline.QpPipelineServer(new Quick.Protocol.Pipeline.QpPipelineServerOptions()
            {
                PipeName = "Quick.Protocol",
                Password = "HelloQP",
                ServerProgram = nameof(PipelineServer) + " 1.0"
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
        }


        private static void Server_ChannelDisconnected(object sender, QpServerChannel e)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: 通道[{e.ChannelName}]已断开!");
        }
    }
}
