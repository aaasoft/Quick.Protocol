using Quick.Protocol;
using Quick.Protocol.Pipeline;
using Quick.Protocol.SerialPort;
using Quick.Protocol.Tcp;
using Quick.Protocol.Utils;
using Quick.Protocol.WebSocket.Client;
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

        private Func<string> getConnectionInfoFunc = null;
        private Func<QpClient> getQpClientFunc = null;

        public ConnectForm()
        {
            InitializeComponent();
            cbConnectType.SelectedIndex = 0;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            try
            {
                ConnectionInfo = getConnectionInfoFunc();
                QpClient = getQpClientFunc();
                await QpClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
                return;
            }
            finally
            {
                btnConnect.Enabled = true;
            }
            //关闭窗口
            Close();
        }

        private void cbConnectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            QpClientOptions options = null;
            getConnectionInfoFunc = null;
            getQpClientFunc = null;

            switch (cbConnectType.SelectedIndex)
            {
                //TCP
                case 0:
                    var tcpOptions = new QpTcpClientOptions()
                    {
                        Host = "127.0.0.1",
                        Port = 3011,
                        Password = "HelloQP"
                    };
                    getConnectionInfoFunc = () => $"[TCP]{tcpOptions.Host}:{tcpOptions.Port}";
                    getQpClientFunc = () => new QpTcpClient(tcpOptions);
                    options = tcpOptions;
                    break;
                //命名管道
                case 1:
                    var pipelineOptions = new QpPipelineClientOptions()
                    {
                        PipeName = "Quick.Protocol",
                        Password = "HelloQP"
                    };
                    getConnectionInfoFunc = () => $"[命名管道]{pipelineOptions.ServerName}\\{pipelineOptions.PipeName}";
                    getQpClientFunc = () => new QpPipelineClient(pipelineOptions);
                    options = pipelineOptions;
                    break;
                //串口
                case 2:
                    var serialPortOptions = new QpSerialPortClientOptions()
                    {
                        PortName = "COM1",
                        Password = "HelloQP"
                    };
                    getConnectionInfoFunc = () => $"[串口]{serialPortOptions.PortName}";
                    getQpClientFunc = () => new QpSerialPortClient(serialPortOptions);
                    options = serialPortOptions;
                    break;
                //WebSocket
                case 3:
                    var webSocketOptions = new QpWebSocketClientOptions()
                    {
                        Url = "ws://127.0.0.1:3011/qp_test",
                        Password = "HelloQP"
                    };
                    getConnectionInfoFunc = () => $"[WebSocket]{webSocketOptions.Url}";
                    getQpClientFunc = () => new QpWebSocketClient(webSocketOptions);
                    options = webSocketOptions;
                    break;
            }
            pgOptions.SelectedObject = options;
            btnConnect.Enabled = pgOptions.SelectedObject != null;
        }
    }
}
