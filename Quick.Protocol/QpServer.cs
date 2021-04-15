using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.Protocol
{
    public abstract class QpServer
    {
        private CancellationTokenSource cts;
        private QpServerOptions options;
        private List<QpServerChannel> channelList = new List<QpServerChannel>();

        /// <summary>
        /// 增加Tag属性，用于引用与QpServer相关的对象
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// 获取全部的通道
        /// </summary>
        public QpServerChannel[] Channels
        {
            get
            {
                lock (channelList)
                    return channelList.ToArray();
            }
        }
        /// <summary>
        /// 通道连接上时
        /// </summary>
        public event EventHandler<QpServerChannel> ChannelConnected;

        /// <summary>
        /// 通道连接断开时
        /// </summary>
        public event EventHandler<QpServerChannel> ChannelDisconnected;

        public QpServer(QpServerOptions options)
        {
            options.Check();
            this.options = options;
        }

        public virtual void Start()
        {
            cts = new CancellationTokenSource();
            beginAccept(cts.Token);
        }

        internal void RemoveChannel(QpServerChannel channel)
        {
            lock (channelList)
                if (channelList.Contains(channel))
                    channelList.Remove(channel);
        }

        protected void OnNewChannelConnected(Stream stream, string channelName, CancellationToken token)
        {
            var channel = new QpServerChannel(this, stream, channelName, token, options.Clone());
            lock (channelList)
                channelList.Add(channel);
            channel.Disconnected += (sender, e) =>
            {
                if (LogUtils.LogConnection)
                    Console.WriteLine("[Connection]{0} Disconnected.", channelName);
                RemoveChannel(channel);
                try { stream.Dispose(); }
                catch { }
                ChannelDisconnected?.Invoke(this, channel);
            };
            Task.Run(() =>
            {
                channel.Start();
                ChannelConnected?.Invoke(this, channel);
            });
        }

        protected abstract Task InnerAcceptAsync(CancellationToken token);

        private void beginAccept(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            InnerAcceptAsync(token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return;
                if (task.IsFaulted)
                    return;
                beginAccept(token);
            });
        }

        public virtual void Stop()
        {
            cts.Cancel();
            cts = null;
        }
    }
}
