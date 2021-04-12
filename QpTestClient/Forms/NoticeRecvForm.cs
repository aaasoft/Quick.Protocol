using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QpTestClient.Forms
{
    public partial class NoticeRecvForm : Form
    {
        private ConnectionContext connectionContext;
        private QpNoticeInfo item;
        private QpClient client;

        public NoticeRecvForm(ConnectionContext connectionContext, QpNoticeInfo item)
        {
            this.connectionContext = connectionContext;
            this.item = item;
            InitializeComponent();
            this.Text += $" - {item.Name} - {connectionContext.ConnectionInfo.Name}";
        }


        private void NoticeRecvForm_Load(object sender, EventArgs e)
        {
            client = connectionContext.QpClient;
            if (client == null)
            {
                txtLog.Text = $"{DateTime.Now.ToLongTimeString()}: 当前未连接，无法接收！{Environment.NewLine}";
                return;
            }
            txtLog.Text = $"{DateTime.Now.ToLongTimeString()}: 开始接收..{Environment.NewLine}";
            client.RawNoticePackageReceived += Client_RawNoticePackageReceived;
        }

        private void Client_RawNoticePackageReceived(object sender, RawNoticePackageReceivedEventArgs e)
        {
            if (e.TypeName != item.NoticeTypeName)
                return;

            Invoke(new Action(() =>
            {
                txtLog.Clear();
                txtLog.AppendText($"{DateTime.Now.ToLongTimeString()}: 接收到通知{Environment.NewLine}");
                txtLog.AppendText($"类型：{e.TypeName}{Environment.NewLine}");
                txtLog.AppendText($"内容{Environment.NewLine}");
                txtLog.AppendText($"--------------------------{Environment.NewLine}");
                txtLog.AppendText(e.Content);
            }));
        }

        private void NoticeRecvForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
            {
                client.RawNoticePackageReceived -= Client_RawNoticePackageReceived;
                client = null;
            }
        }
    }
}
