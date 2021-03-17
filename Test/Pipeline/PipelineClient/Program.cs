using System;

namespace PipelineClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();
            Quick.Protocol.Utils.LogUtils.LogHeartbeat = true;
            Quick.Protocol.Utils.LogUtils.LogNotice = true;

            var client = new Quick.Protocol.Pipeline.QpPipelineClient(new Quick.Protocol.Pipeline.QpPipelineClientOptions()
            {
                PipeName = "Quick.Protocol",
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

                client.SendNoticePackage(new Quick.Protocol.Notices.Ping() { Content = "Hello Quick.Protocol V2!" });
            });
            Console.ReadLine();
        }
    }
}
