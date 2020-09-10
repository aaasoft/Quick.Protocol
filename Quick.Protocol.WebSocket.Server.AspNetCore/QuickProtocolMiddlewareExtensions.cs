using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Quick.Protocol.WebSocket.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Microsoft.AspNetCore.Builder
{
    public static class QuickProtocolMiddlewareExtensions
    {
        public static IApplicationBuilder UseQuickProtocol(this IApplicationBuilder app, QpWebSocketServerOptions options, out QpWebSocketServer server)
        {
            var innerServer = new QpWebSocketServer(options);
            server = innerServer;
            server.Start();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == options.Path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await innerServer.OnNewConnection(webSocket, context.Connection);
                        return;
                    }
                    else
                    {
                        await context.Response.WriteAsync($@"Quick.Protocol
------------
ServerProgram:{options.ServerProgram}
InstructionSet: {string.Join(" | ", options.InstructionSet.Select(t => $"{t.Name}({t.Id})"))}
SendTimeout:{options.SendTimeout}
ReceiveTimeout:{options.ReceiveTimeout}
MaxPackageSize:{options.MaxPackageSize}
BufferSize:{options.BufferSize}
HeartBeatInterval:{options.HeartBeatInterval}
Path:{options.Path}");
                    }
                }
                else
                {
                    await next();
                }
            });
            return app;
        }
    }
}