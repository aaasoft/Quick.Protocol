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
        private TreeNode rootNode;

        public MainForm()
        {
            InitializeComponent();
            Text = Application.ProductName;
            rootNode = tvQpInstructions.Nodes.Add("root", "全部连接", 0, 0);
            rootNode.ContextMenuStrip = cmsAllConnections;
            cmsBtnAddConnect.Click += BtnAddConnection_Click;
            btnAddConnection.Click += BtnAddConnection_Click;
            btnDisconnectConnection.Click += BtnDisconnectConnection_Click;
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
            foreach(TreeNode connectNode in rootNode.Nodes)
            {
                var client = (QpClient)connectNode.Tag;
                client.Close();
                client = null;
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
            control.ToolbarVisible = false;
            control.PropertySort = PropertySort.Categorized;
            return control;
        }

        private void tvQpInstructions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            var nodeObj = node.Tag;

            gbNodeInfo.Text = node.Text;
            if (nodeObj == null)
            {
                showContent(null);
            }
            else if (nodeObj is QpClient)
            {
                var qpClient = (QpClient)nodeObj;
                showContent(getPropertyGridControl(qpClient.Options));
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
                showContent(getPropertyGridControl(nodeObj));
            }
        }
        private void addConnection(string connectionInfo, QpClient qpClient, QpInstruction[] qpInstructions)
        {
            var connectNode = rootNode.Nodes.Add(connectionInfo, connectionInfo, 1, 1);
            connectNode.Tag = qpClient;
            connectNode.ContextMenuStrip = cmsConnection;
            try
            {
                foreach (var instruction in qpInstructions)
                {
                    var instructionNode = connectNode.Nodes.Add(instruction.Id, instruction.Name, 2, 2);
                    instructionNode.Tag = instruction;
                    var noticesNode = instructionNode.Nodes.Add("Notice", "通知", 3, 3);
                    foreach (var noticeInfo in instruction.NoticeInfos)
                    {
                        var noticeNode = noticesNode.Nodes.Add(noticeInfo.NoticeTypeName, noticeInfo.Name, 4, 4);
                        noticeNode.Tag = noticeInfo;
                    }
                    var commandsNode = instructionNode.Nodes.Add("Command", "命令", 3, 3);
                    foreach (var commandInfo in instruction.CommandInfos)
                    {
                        var commandNode = commandsNode.Nodes.Add(commandInfo.RequestTypeName, commandInfo.Name, 5, 5);
                        commandNode.Tag = commandInfo;
                    }
                }
                connectNode.ExpandAll();
                rootNode.Expand();
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

        private void BtnAddConnection_Click(object sender, EventArgs e)
        {
            var form = new ConnectForm();
            var ret = form.ShowDialog();
            if (ret != DialogResult.OK)
                return;
            addConnection(form.ConnectionInfo, form.QpClient, form.QpInstructions);
        }

        private void BtnDisconnectConnection_Click(object sender, EventArgs e)
        {
            var client =  tvQpInstructions.SelectedNode.Tag as QpClient;
            if (client == null)
                return;
            client.Close();
            tvQpInstructions.SelectedNode.Parent.Nodes.Remove(tvQpInstructions.SelectedNode);
        }
    }
}
