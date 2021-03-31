using Quick.Protocol;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QpTestClient.Controls
{
    public partial class CommandInfoControl : UserControl
    {
        private QpClient client;
        private QpCommandInfo item;
        public CommandInfoControl(QpClient client, QpCommandInfo item)
        {
            this.client = client;
            this.item = item;
            InitializeComponent();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"名称：{item.Name}");
            sb.AppendLine($"请求编号：{item.RequestTypeName}");
            sb.AppendLine($"响应编号：{item.ResponseTypeName}");
            if (!string.IsNullOrEmpty(item.Description))
            {
                sb.AppendLine("描述:");
                sb.AppendLine("---------------------");
                sb.AppendLine(item.Description);
            }
            txtBasic.Text = sb.ToString();
            txtRequestSchema.Text = item.RequestTypeSchema;
            txtRequestSchemaSample.Text = item.RequestTypeSchemaSample;
            txtResponseSchema.Text = item.ResponseTypeSchema;
            txtResponseSchemaSample.Text = item.ResponseTypeSchemaSample;

            txtTestRequest.Text = item.RequestTypeSchemaSample;
            btnExecuteTest.Click += BtnExecuteTest_Click;
        }

        private async void BtnExecuteTest_Click(object sender, EventArgs e)
        {
            btnExecuteTest.Enabled = false;
            txtTestResponse.Clear();
            txtTestResponse.AppendText($"{DateTime.Now.ToLongTimeString()}: 开始执行...{Environment.NewLine}");
            try
            {
                var ret = await client.SendCommand(item.RequestTypeName, txtTestRequest.Text.Trim());
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
                txtTestResponse.AppendText(ExceptionUtils.GetExceptionString(ex));
            }
            btnExecuteTest.Enabled = true;
        }
    }
}
