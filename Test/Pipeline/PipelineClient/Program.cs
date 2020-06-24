using System;

namespace PipelineClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Quick.Protocol.Utils.LogUtils.AddConsole();

            var client = new Quick.Protocol.Pipeline.QpPipelineClient(new Quick.Protocol.Pipeline.QpPipelineClientOptions()
            {
                PipeName = "Quick.Protocol",
                Password = "HelloQP",
                EnableCompress = false,
                EnableEncrypt = false
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
