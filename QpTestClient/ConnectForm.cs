using Quick.Protocol;
using Quick.Protocol.Tcp;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QpTestClient
{
    public partial class ConnectForm : Form
    {
        public string ConnectionInfo { get; private set; }
        public QpClient QpClient { get; private set; }
        public QpInstruction[] QpInstructions { get; private set; }

        public ConnectForm()
        {
            InitializeComponent();
        }

        private T handleClientOptions<T>(T clientOptions)
            where T : QpClientOptions
        {
            clientOptions.EnableCompress = cbCompress.Checked;
            clientOptions.EnableEncrypt = cbEncrypt.Checked;
            clientOptions.Password = txtPassword.Text;
            return clientOptions;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            try
            {
                switch (tcConnectMethod.SelectedIndex)
                {
                    case 0:
                        var tcpHost = txtTcpHost.Text.Trim();
                        var tcpPort = Convert.ToInt32(nudTcpPort.Value);
                        ConnectionInfo = $"[TCP]{tcpHost}:{tcpPort}";
                        QpClient = new QpTcpClient(handleClientOptions(new QpTcpClientOptions()
                        {
                            Host = tcpHost,
                            Port = tcpPort
                        }));
                        break;
                    case 1:
                        {

                            break;
                        }
                }
                await QpClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnConnect.Enabled = true;
            }
            //获取指令集信息
            try
            {
                btnConnect.Enabled = false;
                var rep = await QpClient.SendCommand(new Quick.Protocol.Commands.GetQpInstructions.Request());
                QpInstructions = rep.Data;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取指令集时出错，原因：" + ExceptionUtils.GetExceptionMessage(ex), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnConnect.Enabled = true;
            }
            //关闭窗口
            Close();
        }
    }
}
