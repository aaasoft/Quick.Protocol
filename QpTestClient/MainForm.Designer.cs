
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
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.tvQpInstructions = new System.Windows.Forms.TreeView();
            this.ilQpInstructions = new System.Windows.Forms.ImageList(this.components);
            this.gbNodeInfo = new System.Windows.Forms.GroupBox();
            this.btnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMain.SuspendLayout();
            this.ssMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
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
            this.ilQpInstructions.Images.SetKeyName(0, "Refresh.png");
            this.ilQpInstructions.Images.SetKeyName(1, "Connection.png");
            this.ilQpInstructions.Images.SetKeyName(2, "Instruction.png");
            this.ilQpInstructions.Images.SetKeyName(3, "Folder.png");
            this.ilQpInstructions.Images.SetKeyName(4, "Notice.png");
            this.ilQpInstructions.Images.SetKeyName(5, "Command.png");
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
            // btnFile
            // 
            this.btnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddConnection});
            this.btnFile.Image = ((System.Drawing.Image)(resources.GetObject("btnFile.Image")));
            this.btnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(71, 24);
            this.btnFile.Text = "文件(&F)";
            // 
            // btnAddConnection
            // 
            this.btnAddConnection.Name = "btnAddConnection";
            this.btnAddConnection.Size = new System.Drawing.Size(185, 26);
            this.btnAddConnection.Text = "添加连接(&A)...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 539);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.tsMain);
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
    }
}

