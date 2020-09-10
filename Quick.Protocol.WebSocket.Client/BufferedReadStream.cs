using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Quick.Protocol.WebSocket.Client
{
    internal class BufferedReadStream : Stream
    {
        private AutoResetEvent readAutoResetEvent;
        private Queue<byte[]> dataQueue = new Queue<byte[]>();
        private int firstItemOffset = 0;
        private int avalibleCount = 0;

        public BufferedReadStream()
        {
            readAutoResetEvent = new AutoResetEvent(false);
        }
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => avalibleCount;

        public override long Position { get; set; }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            while (true)
            {
                var queueCount = dataQueue.Count;
                if (queueCount == 0)
                {
                    readAutoResetEvent.WaitOne();
                    continue;
                }

                lock (dataQueue)
                {
                    var item = dataQueue.Peek();
                    var itemAvalibleCount = item.Length - firstItemOffset;
                    var copyCount = Math.Min(count, itemAvalibleCount);
                    Buffer.BlockCopy(item, firstItemOffset, buffer, offset, copyCount);
                    //此item的数据已读取完毕,去除此item
                    if (copyCount == itemAvalibleCount)
                    {
                        dataQueue.Dequeue();
                        firstItemOffset = 0;
                    }
                    else
                    {
                        firstItemOffset += copyCount;
                    }
                    //如果复制的数据等于count，则读取完成
                    if (copyCount == count)
                        return copyCount;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] newItem = null;
            if (offset == 0 && count == buffer.Length)
                newItem = buffer;
            else
            {
                newItem = new byte[count];
                Buffer.BlockCopy(buffer, offset, newItem, 0, count);
            }
            lock (dataQueue)
                dataQueue.Enqueue(newItem);
            readAutoResetEvent.Set();
            Interlocked.Add(ref avalibleCount, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            readAutoResetEvent.Dispose();
            lock (dataQueue)
                dataQueue.Clear();
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }
    }
}
