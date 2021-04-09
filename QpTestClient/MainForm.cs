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
        private TreeNodeCollection treeNodeCollection;

        public MainForm()
        {
            InitializeComponent();
            Text = Application.ProductName;
            treeNodeCollection = tvQpInstructions.Nodes;
            cmsConnection.Opening += CmsConnection_Opening;
            btnAddConnection.Click += BtnAddConnection_Click;
            btnOpenConnectionFile.Click += BtnOpenConnectionFile_Click;
            btnExit.Click += BtnExit_Click;
            btnDisconnectConnection.Click += BtnDisconnectConnection_Click;
            btnConnectConnection.Click += BtnConnectConnection_Click;
            btnDelConnection.Click += BtnDelConnection_Click;
            btnSaveConnectionFile.Click += BtnSaveConnectionFile_Click;
        }

        private void pushState(string state)
        {
            this.Invoke(new Action(() =>
            {
                lblStatus.Text = state;
            }));
        }

        private void pushState_Ready() => pushState("就绪");

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TreeNode connectNode in treeNodeCollection)
            {
                var connectionContext = (ConnectionContext)connectNode.Tag;
                connectionContext.Dispose();
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
            TypeDescriptor.AddAttributes(obj, new Attribute[] { new ReadOnlyAttribute(true) });

            var control = new PropertyGrid();
            control.SelectedObject = obj;
            control.ToolbarVisible = false;
            control.PropertySort = PropertySort.NoSort;
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
            else if (nodeObj is ConnectionContext)
            {
                var connectionContext = (ConnectionContext)nodeObj;
                showContent(getPropertyGridControl(connectionContext.ConnectionInfo.QpClientOptions));
            }
            else if (nodeObj is QpInstruction)
            {
                showContent(getPropertyGridControl(nodeObj));
            }
            else if (nodeObj is QpNoticeInfo)
            {
                showContent(new Controls.NoticeInfoControl((QpNoticeInfo)nodeObj));
            }
            else if (nodeObj is QpCommandInfo)
            {
                ConnectionContext connectionContext;
                var currentNode = e.Node;
                while (true)
                {
                    if (currentNode.Tag is ConnectionContext)
                    {
                        connectionContext = (ConnectionContext)currentNode.Tag;
                        break;
                    }
                    currentNode = currentNode.Parent;
                }
                showContent(new Controls.CommandInfoControl(connectionContext.QpClient, (QpCommandInfo)nodeObj));
            }
        }


        private void CmsConnection_Opening(object sender, CancelEventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            if(connectionNode==null)
            {
                e.Cancel = true;
                return;
            }
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
            {
                e.Cancel = true;
                return;
            }
            if (connectionContext.Connected)
            {
                btnConnectConnection.Visible = false;
                btnDisconnectConnection.Visible = true;
            }
            else
            {
                btnConnectConnection.Visible = true;
                btnDisconnectConnection.Visible = false;
            }
        }

        private void displayInstructions(TreeNode connectionNode ,QpInstruction[] instructions)
        {
            connectionNode.Nodes.Clear();
            foreach (var instruction in instructions)
            {
                var instructionNode = connectionNode.Nodes.Add(instruction.Id, instruction.Name, 2, 2);
                instructionNode.Tag = instruction;
                var noticesNode = instructionNode.Nodes.Add("Notice", "通知", 3, 3);
                foreach (var noticeInfo in instruction.NoticeInfos)
                {
                    var noticeNode = noticesNode.Nodes.Add(noticeInfo.NoticeTypeName, noticeInfo.Name, 3, 3);
                    noticeNode.Tag = noticeInfo;
                }
                var commandsNode = instructionNode.Nodes.Add("Command", "命令", 4, 4);
                foreach (var commandInfo in instruction.CommandInfos)
                {
                    var commandNode = commandsNode.Nodes.Add(commandInfo.RequestTypeName, commandInfo.Name, 4, 4);
                    commandNode.Tag = commandInfo;
                }
            }
        }

        private void addConnection(ConnectionInfo connectionInfo)
        {
            var connectionNode = treeNodeCollection.Add(connectionInfo.Name, connectionInfo.Name, 0, 0);
            connectionNode.Tag = new ConnectionContext(connectionInfo);
            connectionNode.ContextMenuStrip = cmsConnection;
            if (connectionInfo.Instructions != null)
                displayInstructions(connectionNode, connectionInfo.Instructions);
        }

        private void BtnAddConnection_Click(object sender, EventArgs e)
        {
            var form = new ConnectForm();
            var ret = form.ShowDialog();
            if (ret != DialogResult.OK)
                return;
            addConnection(form.ConnectionInfo);
        }

        private void BtnDisconnectConnection_Click(object sender, EventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
                return;
            connectionContext.Dispose();
            connectionNode.ImageIndex = connectionNode.SelectedImageIndex = 0;
        }

        private void BtnDelConnection_Click(object sender, EventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
                return;
            connectionContext.Dispose();
            treeNodeCollection.Remove(connectionNode);
        }

        private async void BtnConnectConnection_Click(object sender, EventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
                return;
            try
            {
                await connectionContext.Connect();
                connectionNode.ImageIndex = connectionNode.SelectedImageIndex = 1;
                connectionContext.QpClient.Disconnected += (sender, e) =>
                {
                    Invoke(new Action(() => connectionNode.ImageIndex = connectionNode.SelectedImageIndex = 0));
                };
                displayInstructions(connectionNode, connectionContext.ConnectionInfo.Instructions);
                connectionNode.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSaveConnectionFile_Click(object sender, EventArgs e)
        {

        }

        private void BtnOpenConnectionFile_Click(object sender, EventArgs e)
        {

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
