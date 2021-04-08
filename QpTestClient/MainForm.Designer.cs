
namespace QpTestClient
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.btnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenConnectionFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.tvQpInstructions = new System.Windows.Forms.TreeView();
            this.ilQpInstructions = new System.Windows.Forms.ImageList(this.components);
            this.gbNodeInfo = new System.Windows.Forms.GroupBox();
            this.cmsAllConnections = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsBtnAddConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsConnection = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnDisconnectConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnectConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDelConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveConnectionFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMain.SuspendLayout();
            this.ssMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.cmsAllConnections.SuspendLayout();
            this.cmsConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFile});
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(833, 27);
            this.tsMain.TabIndex = 2;
            this.tsMain.Text = "toolStrip1";
            // 
            // btnFile
            // 
            this.btnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddConnection,
            this.btnOpenConnectionFile,
            this.btnExit});
            this.btnFile.Image = ((System.Drawing.Image)(resources.GetObject("btnFile.Image")));
            this.btnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(71, 24);
            this.btnFile.Text = "文件(&F)";
            // 
            // btnAddConnection
            // 
            this.btnAddConnection.Name = "btnAddConnection";
            this.btnAddConnection.Size = new System.Drawing.Size(212, 26);
            this.btnAddConnection.Text = "添加连接(&A)...";
            // 
            // btnOpenConnectionFile
            // 
            this.btnOpenConnectionFile.Name = "btnOpenConnectionFile";
            this.btnOpenConnectionFile.Size = new System.Drawing.Size(212, 26);
            this.btnOpenConnectionFile.Text = "打开连接文件(&O)..";
            // 
            // btnExit
            // 
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(212, 26);
            this.btnExit.Text = "退出(&X)";
            // 
            // ssMain
            // 
            this.ssMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.ssMain.Location = new System.Drawing.Point(0, 513);
            this.ssMain.Name = "ssMain";
            this.ssMain.Size = new System.Drawing.Size(833, 26);
            this.ssMain.TabIndex = 4;
            this.ssMain.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 20);
            this.lblStatus.Text = "就绪";
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 27);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.tvQpInstructions);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.gbNodeInfo);
            this.scMain.Size = new System.Drawing.Size(833, 486);
            this.scMain.SplitterDistance = 277;
            this.scMain.TabIndex = 5;
            // 
            // tvQpInstructions
            // 
            this.tvQpInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvQpInstructions.HideSelection = false;
            this.tvQpInstructions.ImageIndex = 0;
            this.tvQpInstructions.ImageList = this.ilQpInstructions;
            this.tvQpInstructions.Location = new System.Drawing.Point(0, 0);
            this.tvQpInstructions.Name = "tvQpInstructions";
            this.tvQpInstructions.SelectedImageIndex = 0;
            this.tvQpInstructions.Size = new System.Drawing.Size(277, 486);
            this.tvQpInstructions.TabIndex = 0;
            this.tvQpInstructions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvQpInstructions_AfterSelect);
            // 
            // ilQpInstructions
            // 
            this.ilQpInstructions.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilQpInstructions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilQpInstructions.ImageStream")));
            this.ilQpInstructions.TransparentColor = System.Drawing.Color.Transparent;
            this.ilQpInstructions.Images.SetKeyName(0, "earth_16px_583348_easyicon.net.png");
            this.ilQpInstructions.Images.SetKeyName(1, "plug_connect_16px_514215_easyicon.net.png");
            this.ilQpInstructions.Images.SetKeyName(2, "plug_disconnect_slash_16px_514217_easyicon.net (1).png");
            this.ilQpInstructions.Images.SetKeyName(3, "archive_16px_1092529_easyicon.net.png");
            this.ilQpInstructions.Images.SetKeyName(4, "folder_12px_1084454_easyicon.net.png");
            this.ilQpInstructions.Images.SetKeyName(5, "speaker_volume_16px_514569_easyicon.net.png");
            this.ilQpInstructions.Images.SetKeyName(6, "command_16px_583324_easyicon.net.png");
            // 
            // gbNodeInfo
            // 
            this.gbNodeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbNodeInfo.Location = new System.Drawing.Point(0, 0);
            this.gbNodeInfo.Name = "gbNodeInfo";
            this.gbNodeInfo.Size = new System.Drawing.Size(552, 486);
            this.gbNodeInfo.TabIndex = 0;
            this.gbNodeInfo.TabStop = false;
            // 
            // cmsAllConnections
            // 
            this.cmsAllConnections.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsAllConnections.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsBtnAddConnect});
            this.cmsAllConnections.Name = "cmsAllConnections";
            this.cmsAllConnections.Size = new System.Drawing.Size(172, 28);
            // 
            // cmsBtnAddConnect
            // 
            this.cmsBtnAddConnect.Name = "cmsBtnAddConnect";
            this.cmsBtnAddConnect.Size = new System.Drawing.Size(171, 24);
            this.cmsBtnAddConnect.Text = "添加连接(&A)...";
            // 
            // cmsConnection
            // 
            this.cmsConnection.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsConnection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDisconnectConnection,
            this.btnConnectConnection,
            this.btnDelConnection,
            this.btnSaveConnectionFile});
            this.cmsConnection.Name = "cmsConnection";
            this.cmsConnection.Size = new System.Drawing.Size(211, 100);
            // 
            // btnDisconnectConnection
            // 
            this.btnDisconnectConnection.Name = "btnDisconnectConnection";
            this.btnDisconnectConnection.Size = new System.Drawing.Size(210, 24);
            this.btnDisconnectConnection.Text = "断开(&D)";
            // 
            // btnConnectConnection
            // 
            this.btnConnectConnection.Name = "btnConnectConnection";
            this.btnConnectConnection.Size = new System.Drawing.Size(210, 24);
            this.btnConnectConnection.Text = "连接";
            // 
            // btnDelConnection
            // 
            this.btnDelConnection.Name = "btnDelConnection";
            this.btnDelConnection.Size = new System.Drawing.Size(210, 24);
            this.btnDelConnection.Text = "删除(&D)";
            // 
            // btnSaveConnectionFile
            // 
            this.btnSaveConnectionFile.Name = "btnSaveConnectionFile";
            this.btnSaveConnectionFile.Size = new System.Drawing.Size(210, 24);
            this.btnSaveConnectionFile.Text = "保存为连接文件(&S)..";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 539);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.tsMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.cmsAllConnections.ResumeLayout(false);
            this.cmsConnection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.TreeView tvQpInstructions;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ImageList ilQpInstructions;
        private System.Windows.Forms.GroupBox gbNodeInfo;
        private System.Windows.Forms.ToolStripDropDownButton btnFile;
        private System.Windows.Forms.ToolStripMenuItem btnAddConnection;
        private System.Windows.Forms.ContextMenuStrip cmsAllConnections;
        private System.Windows.Forms.ToolStripMenuItem cmsBtnAddConnect;
        private System.Windows.Forms.ContextMenuStrip cmsConnection;
        private System.Windows.Forms.ToolStripMenuItem btnDisconnectConnection;
        private System.Windows.Forms.ToolStripMenuItem btnOpenConnectionFile;
        private System.Windows.Forms.ToolStripMenuItem btnExit;
        private System.Windows.Forms.ToolStripMenuItem btnConnectConnection;
        private System.Windows.Forms.ToolStripMenuItem btnDelConnection;
        private System.Windows.Forms.ToolStripMenuItem btnSaveConnectionFile;
    }
}

