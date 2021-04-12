using Quick.Protocol;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QpTestClient.Forms
{
    public partial class CommandTestForm : Form
    {
        private ConnectionContext connectionContext;
        private QpCommandInfo item;

        public CommandTestForm(ConnectionContext connectionContext, QpCommandInfo item)
        {
            this.connectionContext = connectionContext;
            this.item = item;

            InitializeComponent();
            this.Text += $" - {item.Name} - {connectionContext.ConnectionInfo.Name}";
            txtCommandRequestTypeName.Text = item.RequestTypeName;
        }

        private void CommandTestForm_Load(object sender, EventArgs e)
        {
            txtTestRequest.Text = item.RequestTypeSchemaSample;
            btnExecuteTest.Click += BtnExecuteTest_Click;
        }

        private async void BtnExecuteTest_Click(object sender, EventArgs e)
        {
            btnExecuteTest.Enabled = false;
            txtTestResponse.Clear();
            var qpClient = connectionContext.QpClient;
            if (qpClient == null)
            {
                txtTestResponse.AppendText($"{DateTime.Now.ToLongTimeString()}: 当前未连接，无法执行！{Environment.NewLine}");
                return;
            }
            txtTestResponse.AppendText($"{DateTime.Now.ToLongTimeString()}: 开始执行...{Environment.NewLine}");
            try
            {
                var ret = await qpClient.SendCommand(item.RequestTypeName, txtTestRequest.Text.Trim());
                txtTestResponse.AppendText($"{DateTime.Now.ToLongTimeString()}: 执行成功{Environment.NewLine}");
                txtTestResponse.AppendText($"类型：{ret.TypeName}{Environment.NewLine}");
                txtTestResponse.AppendText($"内容{Environment.NewLine}");
                txtTestResponse.AppendText($"--------------------------{Environment.NewLine}");
                txtTestResponse.AppendText(ret.Content);
            }
            catch (Exception ex)
            {
                txtTestResponse.AppendText($"{DateTime.Now.ToLongTimeString()}: 执行失败{Environment.NewLine}");
                txtTestResponse.AppendText($"错误信息{Environment.NewLine}");
                txtTestResponse.AppendText($"--------------------------{Environment.NewLine}");
                txtTestResponse.AppendText(ExceptionUtils.GetExceptionMessage(ex));
            }
            btnExecuteTest.Enabled = true;
        }
    }
}
