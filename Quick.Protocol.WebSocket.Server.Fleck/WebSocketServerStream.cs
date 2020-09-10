using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quick.Protocol.WebSocket.Server.Fleck
{
    internal class WebSocketServerStream : Stream
    {
        private IWebSocketConnection connection;
        private BufferedReadStream stream = new BufferedReadStream();

        public WebSocketServerStream(IWebSocketConnection connection)
        {
            this.connection = connection;
            connection.OnBinary = ret =>
            {
                stream.Write(ret, 0, ret.Length);
            };
            connection.OnMessage = ret =>
            {
                connection.OnBinary(Encoding.UTF8.GetBytes(ret));
            };
        }
        
        public override bool CanSeek => throw new NotImplementedException();
        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }

        public override long Length => stream.Length;
        public override long Position { get; set; }

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            var toSendBuffer = buffer;
            if (offset != 0 || count != buffer.Length)
                connection.Send(buffer.Skip(offset).Take(count).ToArray());
            connection.Send(toSendBuffer);
        }

        protected override void Dispose(bool disposing)
        {
            connection.Close();
            base.Dispose(disposing);
        }
    }
}
