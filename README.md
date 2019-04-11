Quick.Protocol
=====

A simple protocol for TCP,Pipeline,SerialPort.
The easiest way how to use Quick.Protocol is via the Quick.Protocol NuGet package. 

Server Sample:

```csharp
static void Main(string[] args)
{
    var server = new QpServer(new QpServerOptions()
    {
        Address = IPAddress.Loopback,
        Port = 3011,
        Password = "HelloQP",
        ServerProgram = nameof(TcpServer) + " 1.0"
    });
    //server.ChannelConnected += Server_ChannelConnected;
    //server.ChannelDisconnected += Server_ChannelDisconnected;
    try
    {
        server.Start();
        Console.WriteLine($"Server start success!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Server start failed!" + ex.ToString());
    }
    Console.ReadLine();
    server.Stop();
}
```


Client Sample:

```csharp
static void Main(string[] args)
{
    //Quick.Protocol.Utils.LogUtils.AddConsole();
    var client = new QpClient(new QpClientOptions()
    {
        Host = "127.0.0.1",
        Port = 3011,
        Password = "HelloQP",
        //EnableCompress = true,
        //EnableEncrypt = true
    });
    client.Disconnected += (sender, e) =>
      {
          Console.WriteLine("Connection Disconnected");
      };
    client.ConnectAsync().ContinueWith(t =>
    {
        if (t.IsCanceled)
        {
            Console.WriteLine("Connect was canceled.");
            return;
        }
        if (t.IsFaulted)
        {
            Console.WriteLine("Connect failed,reason:" + t.Exception.InnerException.ToString());
            return;
        }
        Console.WriteLine("Connect success.");
    });
    Console.ReadLine();
}
```
