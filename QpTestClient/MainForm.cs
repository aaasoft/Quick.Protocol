using QpTestClient.Utils;
using Quick.Protocol;
using Quick.Protocol.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QpTestClient
{
    public partial class MainForm : Form
    {
        public const string QPDFILE_FILTER = "Quick.Protocol连接文件(*.qpd)|*.qpd";

        private TreeNodeCollection treeNodeCollection;

        public MainForm()
        {
            InitializeComponent();
            Text = Application.ProductName;
            treeNodeCollection = tvQpInstructions.Nodes;
            cmsConnection.Opening += CmsConnection_Opening;
            btnAddConnection.Click += BtnAddConnection_Click;
            btnImportConnectionFile.Click += BtnImportConnectionFile_Click;
            btnExit.Click += BtnExit_Click;
            btnDisconnectConnection.Click += BtnDisconnectConnection_Click;
            btnConnectConnection.Click += BtnConnectConnection_Click;
            btnDelConnection.Click += BtnDelConnection_Click;
            btnExportConnectionFile.Click += BtnExportConnectionFile_Click;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var connectionInfos = QpdFileUtils.GetConnectionInfosFromQpbFileFolder();
            if (connectionInfos != null)
            {
                foreach (var connectionInfo in connectionInfos)
                    addConnection(connectionInfo);
            }
        }

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
            if (connectionNode == null)
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
                btnDelConnection.Visible = false;
            }
            else
            {
                btnConnectConnection.Visible = true;
                btnDisconnectConnection.Visible = false;
                btnDelConnection.Visible = true;
            }
        }

        private void displayInstructions(TreeNode connectionNode, QpInstruction[] instructions)
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
            var connectionInfo = form.ConnectionInfo;
            addConnection(connectionInfo);
            QpdFileUtils.SaveQpbFile(connectionInfo);
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

            var ret = MessageBox.Show($"确定要删除连接[{connectionContext.ConnectionInfo.Name}]?", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (ret == DialogResult.Cancel)
                return;

            connectionContext.Dispose();
            treeNodeCollection.Remove(connectionNode);
            QpdFileUtils.DeleteQpbFile(connectionContext.ConnectionInfo);
        }

        private async void BtnConnectConnection_Click(object sender, EventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
                return;

            this.Enabled = false;
            try
            {
                var preConnectionInfoContent = Quick.Xml.XmlConvert.Serialize(connectionContext.ConnectionInfo);

                await connectionContext.Connect();
                connectionNode.ImageIndex = connectionNode.SelectedImageIndex = 1;
                connectionContext.QpClient.Disconnected += (sender, e) =>
                {
                    Invoke(new Action(() => connectionNode.ImageIndex = connectionNode.SelectedImageIndex = 0));
                };
                displayInstructions(connectionNode, connectionContext.ConnectionInfo.Instructions);
                connectionNode.ExpandAll();

                var currentConnectionInfoContent = Quick.Xml.XmlConvert.Serialize(connectionContext.ConnectionInfo);
                if (currentConnectionInfoContent != preConnectionInfoContent)
                    QpdFileUtils.SaveQpbFile(connectionContext.ConnectionInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Enabled = true;
        }

        private void BtnExportConnectionFile_Click(object sender, EventArgs e)
        {
            var connectionNode = tvQpInstructions.SelectedNode;
            var connectionContext = connectionNode.Tag as ConnectionContext;
            if (connectionContext == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = QPDFILE_FILTER;
            var ret = sfd.ShowDialog();
            if (ret == DialogResult.Cancel)
                return;
            var file = sfd.FileName;
            QpdFileUtils.SaveQpbFile(connectionContext.ConnectionInfo, file);
            MessageBox.Show("导出成功！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnImportConnectionFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = QPDFILE_FILTER;
            var ret = ofd.ShowDialog();
            if (ret == DialogResult.Cancel)
                return;
            try
            {
                var file = ofd.FileName;
                ConnectionInfo connectionInfo = QpdFileUtils.Load(file);
                connectionInfo.Name = Path.GetFileNameWithoutExtension(file);
                addConnection(connectionInfo);
                QpdFileUtils.SaveQpbFile(connectionInfo);
                MessageBox.Show("导入成功！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败，原因：{ExceptionUtils.GetExceptionMessage(ex)}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
