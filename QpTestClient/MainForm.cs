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
        private string connectInfo;
        private QpClient client;
        private TreeNode rootNode;

        public MainForm(string connectInfo, QpClient client)
        {
            this.connectInfo = connectInfo;
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            rootNode = tvQpInstructions.Nodes.Add("root", connectInfo, 1, 1);
            rootNode.Tag = client;
            addLoadingChildToNode(rootNode);
        }

        private void addLoadingChildToNode(TreeNode node)
        {
            node.Nodes.Add("loading", "加载中...", 0, 0);
        }

        private void showContent(Control item)
        {
            scMain.Panel2.Controls.Clear();
            item.Dock = DockStyle.Fill;
            scMain.Panel2.Controls.Add(item);
        }

        private void tvQpInstructions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            var nodeObj = node.Tag;
            if (nodeObj == null)
            {
                showContent(new Label() { Text = node.Name });
            }
            else if (nodeObj is QpClient)
            {
                showContent(new Label() { Text = connectInfo });
            }
        }

        private void tvQpInstructions_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            //收起后，清空子节点
            e.Node.Nodes.Clear();
        }

        private async void tvQpInstructions_AfterExpand(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            var nodeObj = node.Tag;
            if (nodeObj == null)
            {
                if (node.Name == "Notice")
                {

                }
                else if (node.Name == "Command")
                {

                }
            }
            else if (nodeObj is QpClient)
            {
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
                        addLoadingChildToNode(instructionNode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("刷新指令集时出错，原因：" + ExceptionUtils.GetExceptionMessage(ex), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Node.Collapse();
                    addLoadingChildToNode(e.Node);
                }
                finally
                {
                    pushState_Ready();
                    tvQpInstructions.Enabled = true;
                }
            }
            else if (nodeObj is Quick.Protocol.Commands.GetQpInstructions.QpInstructionInfo)
            {
                rootNode.Nodes.Clear();
                addLoadingChildToNode(e.Node.Nodes.Add("Notice", "通知数据包", 3, 3));
                addLoadingChildToNode(e.Node.Nodes.Add("Command", "命令数据包", 3, 3));
            }
        }
    }
}
