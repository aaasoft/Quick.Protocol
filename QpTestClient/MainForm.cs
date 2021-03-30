using Quick.Protocol;
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
    public partial class MainForm : Form
    {
        private QpClient client;
        private TreeNode rootNode;

        public MainForm(QpClient client)
        {
            this.client = client;
            InitializeComponent();
            Text = Application.ProductName;

            client.Disconnected += Client_Disconnected;
        }

        private void pushState(string state)
        {
            this.Invoke(new Action(() =>
            {
                lblStatus.Text = state;
            }));
        }

        private void pushState_Ready() => pushState("就绪");

        private void Client_Disconnected(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show("连接已经断开！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
            client = null;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            rootNode = tvQpInstructions.Nodes.Add("root", "连接", 1, 1);
            rootNode.Tag = client;
            rootNode.Nodes.Add("loading", "加载中...", 0, 0);
            rootNode.ExpandAll();

            pushState("正在刷新指令集中...");
            tvQpInstructions.Enabled = false;
            try
            {
                var rep = await client.SendCommand(new Quick.Protocol.Commands.GetQpInstructions.Request());
                rootNode.Nodes.Clear();
                foreach (var item in rep.Data)
                {
                    var instructionNode = rootNode.Nodes.Add(item.Id, item.Name, 2, 2);
                    instructionNode.Tag = item;
                    var noticesNode = instructionNode.Nodes.Add("Notice", "通知", 3, 3);
                    foreach (var noticeInfo in item.NoticeInfos)
                    {
                        var noticeNode = noticesNode.Nodes.Add(noticeInfo.NoticeTypeName, noticeInfo.Name, 4, 4);
                        noticeNode.Tag = noticeInfo;
                    }
                    var commandsNode = instructionNode.Nodes.Add("Command", "命令", 3, 3);
                    foreach (var commandInfo in item.CommandInfos)
                    {
                        var commandNode = commandsNode.Nodes.Add(commandInfo.RequestTypeName, commandInfo.Name, 5, 5);
                        commandNode.Tag = commandInfo;
                    }
                }
                rootNode.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("刷新指令集时出错，原因：" + ExceptionUtils.GetExceptionMessage(ex), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
            finally
            {
                pushState_Ready();
                tvQpInstructions.Enabled = true;
            }
        }

        private void showContent(Control item)
        {
            gbNodeInfo.Controls.Clear();
            if (item != null)
            {
                item.Dock = DockStyle.Fill;
                gbNodeInfo.Controls.Add(item);
            }
        }

        private Control getPropertyGridControl(object obj)
        {
            var control = new PropertyGrid();
            control.SelectedObject = obj;
            control.DisabledItemForeColor = SystemColors.WindowText;
            control.PropertySort = PropertySort.NoSort;
            control.ToolbarVisible = false;
            return control;
        }

        private void tvQpInstructions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            var nodeObj = node.Tag;

            gbNodeInfo.Text = node.FullPath;
            if (nodeObj == null)
            {
                showContent(null);
            }
            else if (nodeObj is QpClient)
            {
                showContent(getPropertyGridControl(client.Options));
            }
            else if (nodeObj is QpInstruction)
            {
                showContent(getPropertyGridControl(nodeObj));
            }
            else if (nodeObj is QpNoticeInfo)
            {
                showContent(getPropertyGridControl(nodeObj));
            }
            else if (nodeObj is QpCommandInfo)
            {
                var item = (QpCommandInfo)nodeObj;
                showContent(new Label() { Text = $"[命令]Id:{item.RequestTypeName},Name:{item.Name}" });
            }
        }
    }
}
