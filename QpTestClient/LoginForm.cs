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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            Text += " - " + Application.ProductName;
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
            try
            {
                QpClient client = null;
                switch (tcConnectMethod.SelectedIndex)
                {
                    case 0:
                        client = new QpTcpClient(handleClientOptions(new QpTcpClientOptions()
                        {
                            Host = txtTcpHost.Text.Trim(),
                            Port = Convert.ToInt32(nudTcpPort.Value)
                        }));
                        break;
                    case 1:
                        {

                            break;
                        }
                }
                this.Enabled = false;
                await client.ConnectAsync();
                this.Hide();
                new MainForm().ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Enabled = true;
            }
        }
    }
}
