using Quick.Protocol;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        }

        private class QpClientTypeInfo
        {
            public string Name { get; set; }
            public Type QpClientType { get; set; }
            public Type QpClientOptionsType { get; set; }
            public override string ToString() => Name;
        }


        private void ConnectForm_Load(object sender, EventArgs e)
        {
            foreach (var dllFile in Directory.GetFiles(".", $"{nameof(Quick)}.{nameof(Quick.Protocol)}.*.dll"))
            {
                var assembly = Assembly.Load(Path.GetFileNameWithoutExtension(dllFile));
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || !typeof(QpClient).IsAssignableFrom(type))
                        continue;

                    var typeConstructor = type.GetConstructors()[0];
                    var typeConstructorParameters = typeConstructor.GetParameters();
                    if (typeConstructorParameters == null || typeConstructorParameters.Length != 1)
                        continue;
                    var optionsType = typeConstructorParameters[0].ParameterType;
                    var name = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;

                    cbConnectType.Items.Add(new QpClientTypeInfo()
                    {
                        Name = name,
                        QpClientType = type,
                        QpClientOptionsType = optionsType
                    });
                }
            }
            if (cbConnectType.Items.Count > 0)
                cbConnectType.SelectedIndex = 0;
            if (cbConnectType.Items.Count > 3)
                cbConnectType.SelectedIndex = 2;
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

            var qpClientTypeInfo = (QpClientTypeInfo)cbConnectType.SelectedItem;
            options = (QpClientOptions)Activator.CreateInstance(qpClientTypeInfo.QpClientOptionsType);
            getConnectionInfoFunc = () => $"[{qpClientTypeInfo.Name}]{options}";
            getQpClientFunc = () => (QpClient)Activator.CreateInstance(qpClientTypeInfo.QpClientType, options);

            pgOptions.SelectedObject = options;
            btnConnect.Enabled = pgOptions.SelectedObject != null;
        }
    }
}
