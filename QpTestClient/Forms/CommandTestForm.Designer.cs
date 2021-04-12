
namespace QpTestClient.Forms
{
    partial class CommandTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandTestForm));
            this.tsTest = new System.Windows.Forms.ToolStrip();
            this.btnExecuteTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblCommandRequestTypeName = new System.Windows.Forms.ToolStripLabel();
            this.txtCommandRequestTypeName = new System.Windows.Forms.ToolStripTextBox();
            this.scTest = new System.Windows.Forms.SplitContainer();
            this.txtTestRequest = new System.Windows.Forms.TextBox();
            this.txtTestResponse = new System.Windows.Forms.TextBox();
            this.tsTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTest)).BeginInit();
            this.scTest.Panel1.SuspendLayout();
            this.scTest.Panel2.SuspendLayout();
            this.scTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsTest
            // 
            this.tsTest.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExecuteTest,
            this.toolStripSeparator1,
            this.lblCommandRequestTypeName,
            this.txtCommandRequestTypeName});
            this.tsTest.Location = new System.Drawing.Point(0, 0);
            this.tsTest.Name = "tsTest";
            this.tsTest.Size = new System.Drawing.Size(817, 27);
            this.tsTest.TabIndex = 1;
            this.tsTest.Text = "toolStrip1";
            // 
            // btnExecuteTest
            // 
            this.btnExecuteTest.Image = ((System.Drawing.Image)(resources.GetObject("btnExecuteTest.Image")));
            this.btnExecuteTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExecuteTest.Name = "btnExecuteTest";
            this.btnExecuteTest.Size = new System.Drawing.Size(63, 24);
            this.btnExecuteTest.Text = "执行";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // lblCommandRequestTypeName
            // 
            this.lblCommandRequestTypeName.Name = "lblCommandRequestTypeName";
            this.lblCommandRequestTypeName.Size = new System.Drawing.Size(73, 24);
            this.lblCommandRequestTypeName.Text = "请求类型:";
            // 
            // txtCommandRequestTypeName
            // 
            this.txtCommandRequestTypeName.Name = "txtCommandRequestTypeName";
            this.txtCommandRequestTypeName.ReadOnly = true;
            this.txtCommandRequestTypeName.Size = new System.Drawing.Size(580, 27);
            // 
            // scTest
            // 
            this.scTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTest.Location = new System.Drawing.Point(0, 27);
            this.scTest.Name = "scTest";
            this.scTest.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTest.Panel1
            // 
            this.scTest.Panel1.Controls.Add(this.txtTestRequest);
            // 
            // scTest.Panel2
            // 
            this.scTest.Panel2.Controls.Add(this.txtTestResponse);
            this.scTest.Size = new System.Drawing.Size(817, 692);
            this.scTest.SplitterDistance = 335;
            this.scTest.TabIndex = 2;
            // 
            // txtTestRequest
            // 
            this.txtTestRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTestRequest.Location = new System.Drawing.Point(0, 0);
            this.txtTestRequest.Multiline = true;
            this.txtTestRequest.Name = "txtTestRequest";
            this.txtTestRequest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestRequest.Size = new System.Drawing.Size(817, 335);
            this.txtTestRequest.TabIndex = 2;
            // 
            // txtTestResponse
            // 
            this.txtTestResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTestResponse.Location = new System.Drawing.Point(0, 0);
            this.txtTestResponse.Multiline = true;
            this.txtTestResponse.Name = "txtTestResponse";
            this.txtTestResponse.ReadOnly = true;
            this.txtTestResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestResponse.Size = new System.Drawing.Size(817, 353);
            this.txtTestResponse.TabIndex = 2;
            // 
            // CommandTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 719);
            this.Controls.Add(this.scTest);
            this.Controls.Add(this.tsTest);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CommandTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "命令测试";
            this.Load += new System.EventHandler(this.CommandTestForm_Load);
            this.tsTest.ResumeLayout(false);
            this.tsTest.PerformLayout();
            this.scTest.Panel1.ResumeLayout(false);
            this.scTest.Panel1.PerformLayout();
            this.scTest.Panel2.ResumeLayout(false);
            this.scTest.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTest)).EndInit();
            this.scTest.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsTest;
        private System.Windows.Forms.ToolStripButton btnExecuteTest;
        private System.Windows.Forms.SplitContainer scTest;
        private System.Windows.Forms.TextBox txtTestRequest;
        private System.Windows.Forms.TextBox txtTestResponse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lblCommandRequestTypeName;
        private System.Windows.Forms.ToolStripTextBox txtCommandRequestTypeName;
    }
}