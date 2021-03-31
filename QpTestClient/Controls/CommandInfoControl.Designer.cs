﻿
namespace QpTestClient.Controls
{
    partial class CommandInfoControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpBasic = new System.Windows.Forms.TabPage();
            this.txtBasic = new System.Windows.Forms.TextBox();
            this.tpRequestSchema = new System.Windows.Forms.TabPage();
            this.txtRequestSchema = new System.Windows.Forms.TextBox();
            this.tpRequestSchemaSample = new System.Windows.Forms.TabPage();
            this.txtRequestSchemaSample = new System.Windows.Forms.TextBox();
            this.tpResponseSchema = new System.Windows.Forms.TabPage();
            this.txtResponseSchema = new System.Windows.Forms.TextBox();
            this.tpResponseSchemaSample = new System.Windows.Forms.TabPage();
            this.txtResponseSchemaSample = new System.Windows.Forms.TextBox();
            this.tpTest = new System.Windows.Forms.TabPage();
            this.scTest = new System.Windows.Forms.SplitContainer();
            this.txtTestRequest = new System.Windows.Forms.TextBox();
            this.txtTestResponse = new System.Windows.Forms.TextBox();
            this.tsTest = new System.Windows.Forms.ToolStrip();
            this.btnExecuteTest = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tpBasic.SuspendLayout();
            this.tpRequestSchema.SuspendLayout();
            this.tpRequestSchemaSample.SuspendLayout();
            this.tpResponseSchema.SuspendLayout();
            this.tpResponseSchemaSample.SuspendLayout();
            this.tpTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTest)).BeginInit();
            this.scTest.Panel1.SuspendLayout();
            this.scTest.Panel2.SuspendLayout();
            this.scTest.SuspendLayout();
            this.tsTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpBasic);
            this.tabControl1.Controls.Add(this.tpRequestSchema);
            this.tabControl1.Controls.Add(this.tpRequestSchemaSample);
            this.tabControl1.Controls.Add(this.tpResponseSchema);
            this.tabControl1.Controls.Add(this.tpResponseSchemaSample);
            this.tabControl1.Controls.Add(this.tpTest);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(432, 437);
            this.tabControl1.TabIndex = 1;
            // 
            // tpBasic
            // 
            this.tpBasic.Controls.Add(this.txtBasic);
            this.tpBasic.Location = new System.Drawing.Point(4, 29);
            this.tpBasic.Name = "tpBasic";
            this.tpBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tpBasic.Size = new System.Drawing.Size(424, 404);
            this.tpBasic.TabIndex = 0;
            this.tpBasic.Text = "基本";
            this.tpBasic.UseVisualStyleBackColor = true;
            // 
            // txtBasic
            // 
            this.txtBasic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBasic.Location = new System.Drawing.Point(3, 3);
            this.txtBasic.Multiline = true;
            this.txtBasic.Name = "txtBasic";
            this.txtBasic.ReadOnly = true;
            this.txtBasic.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBasic.Size = new System.Drawing.Size(418, 398);
            this.txtBasic.TabIndex = 1;
            // 
            // tpRequestSchema
            // 
            this.tpRequestSchema.Controls.Add(this.txtRequestSchema);
            this.tpRequestSchema.Location = new System.Drawing.Point(4, 29);
            this.tpRequestSchema.Name = "tpRequestSchema";
            this.tpRequestSchema.Padding = new System.Windows.Forms.Padding(3);
            this.tpRequestSchema.Size = new System.Drawing.Size(424, 404);
            this.tpRequestSchema.TabIndex = 1;
            this.tpRequestSchema.Text = "请求定义";
            this.tpRequestSchema.UseVisualStyleBackColor = true;
            // 
            // txtRequestSchema
            // 
            this.txtRequestSchema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequestSchema.Location = new System.Drawing.Point(3, 3);
            this.txtRequestSchema.Multiline = true;
            this.txtRequestSchema.Name = "txtRequestSchema";
            this.txtRequestSchema.ReadOnly = true;
            this.txtRequestSchema.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRequestSchema.Size = new System.Drawing.Size(418, 398);
            this.txtRequestSchema.TabIndex = 0;
            // 
            // tpRequestSchemaSample
            // 
            this.tpRequestSchemaSample.Controls.Add(this.txtRequestSchemaSample);
            this.tpRequestSchemaSample.Location = new System.Drawing.Point(4, 29);
            this.tpRequestSchemaSample.Name = "tpRequestSchemaSample";
            this.tpRequestSchemaSample.Padding = new System.Windows.Forms.Padding(3);
            this.tpRequestSchemaSample.Size = new System.Drawing.Size(424, 404);
            this.tpRequestSchemaSample.TabIndex = 2;
            this.tpRequestSchemaSample.Text = "请求示例";
            this.tpRequestSchemaSample.UseVisualStyleBackColor = true;
            // 
            // txtRequestSchemaSample
            // 
            this.txtRequestSchemaSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequestSchemaSample.Location = new System.Drawing.Point(3, 3);
            this.txtRequestSchemaSample.Multiline = true;
            this.txtRequestSchemaSample.Name = "txtRequestSchemaSample";
            this.txtRequestSchemaSample.ReadOnly = true;
            this.txtRequestSchemaSample.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRequestSchemaSample.Size = new System.Drawing.Size(418, 398);
            this.txtRequestSchemaSample.TabIndex = 1;
            // 
            // tpResponseSchema
            // 
            this.tpResponseSchema.Controls.Add(this.txtResponseSchema);
            this.tpResponseSchema.Location = new System.Drawing.Point(4, 29);
            this.tpResponseSchema.Name = "tpResponseSchema";
            this.tpResponseSchema.Padding = new System.Windows.Forms.Padding(3);
            this.tpResponseSchema.Size = new System.Drawing.Size(424, 404);
            this.tpResponseSchema.TabIndex = 3;
            this.tpResponseSchema.Text = "响应定义";
            this.tpResponseSchema.UseVisualStyleBackColor = true;
            // 
            // txtResponseSchema
            // 
            this.txtResponseSchema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResponseSchema.Location = new System.Drawing.Point(3, 3);
            this.txtResponseSchema.Multiline = true;
            this.txtResponseSchema.Name = "txtResponseSchema";
            this.txtResponseSchema.ReadOnly = true;
            this.txtResponseSchema.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResponseSchema.Size = new System.Drawing.Size(418, 398);
            this.txtResponseSchema.TabIndex = 1;
            // 
            // tpResponseSchemaSample
            // 
            this.tpResponseSchemaSample.Controls.Add(this.txtResponseSchemaSample);
            this.tpResponseSchemaSample.Location = new System.Drawing.Point(4, 29);
            this.tpResponseSchemaSample.Name = "tpResponseSchemaSample";
            this.tpResponseSchemaSample.Padding = new System.Windows.Forms.Padding(3);
            this.tpResponseSchemaSample.Size = new System.Drawing.Size(424, 404);
            this.tpResponseSchemaSample.TabIndex = 4;
            this.tpResponseSchemaSample.Text = "响应示例";
            this.tpResponseSchemaSample.UseVisualStyleBackColor = true;
            // 
            // txtResponseSchemaSample
            // 
            this.txtResponseSchemaSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResponseSchemaSample.Location = new System.Drawing.Point(3, 3);
            this.txtResponseSchemaSample.Multiline = true;
            this.txtResponseSchemaSample.Name = "txtResponseSchemaSample";
            this.txtResponseSchemaSample.ReadOnly = true;
            this.txtResponseSchemaSample.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResponseSchemaSample.Size = new System.Drawing.Size(418, 398);
            this.txtResponseSchemaSample.TabIndex = 1;
            // 
            // tpTest
            // 
            this.tpTest.Controls.Add(this.scTest);
            this.tpTest.Controls.Add(this.tsTest);
            this.tpTest.Location = new System.Drawing.Point(4, 29);
            this.tpTest.Name = "tpTest";
            this.tpTest.Padding = new System.Windows.Forms.Padding(3);
            this.tpTest.Size = new System.Drawing.Size(424, 404);
            this.tpTest.TabIndex = 5;
            this.tpTest.Text = "测试";
            this.tpTest.UseVisualStyleBackColor = true;
            // 
            // scTest
            // 
            this.scTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTest.Location = new System.Drawing.Point(3, 30);
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
            this.scTest.Size = new System.Drawing.Size(418, 371);
            this.scTest.SplitterDistance = 180;
            this.scTest.TabIndex = 1;
            // 
            // txtTestRequest
            // 
            this.txtTestRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTestRequest.Location = new System.Drawing.Point(0, 0);
            this.txtTestRequest.Multiline = true;
            this.txtTestRequest.Name = "txtTestRequest";
            this.txtTestRequest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestRequest.Size = new System.Drawing.Size(418, 180);
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
            this.txtTestResponse.Size = new System.Drawing.Size(418, 187);
            this.txtTestResponse.TabIndex = 2;
            // 
            // tsTest
            // 
            this.tsTest.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExecuteTest});
            this.tsTest.Location = new System.Drawing.Point(3, 3);
            this.tsTest.Name = "tsTest";
            this.tsTest.Size = new System.Drawing.Size(418, 27);
            this.tsTest.TabIndex = 0;
            this.tsTest.Text = "toolStrip1";
            // 
            // btnExecuteTest
            // 
            this.btnExecuteTest.Image = global::QpTestClient.Properties.Resources._009_HighPriority_32x32_72;
            this.btnExecuteTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExecuteTest.Name = "btnExecuteTest";
            this.btnExecuteTest.Size = new System.Drawing.Size(63, 24);
            this.btnExecuteTest.Text = "执行";
            // 
            // CommandInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "CommandInfoControl";
            this.Size = new System.Drawing.Size(432, 437);
            this.tabControl1.ResumeLayout(false);
            this.tpBasic.ResumeLayout(false);
            this.tpBasic.PerformLayout();
            this.tpRequestSchema.ResumeLayout(false);
            this.tpRequestSchema.PerformLayout();
            this.tpRequestSchemaSample.ResumeLayout(false);
            this.tpRequestSchemaSample.PerformLayout();
            this.tpResponseSchema.ResumeLayout(false);
            this.tpResponseSchema.PerformLayout();
            this.tpResponseSchemaSample.ResumeLayout(false);
            this.tpResponseSchemaSample.PerformLayout();
            this.tpTest.ResumeLayout(false);
            this.tpTest.PerformLayout();
            this.scTest.Panel1.ResumeLayout(false);
            this.scTest.Panel1.PerformLayout();
            this.scTest.Panel2.ResumeLayout(false);
            this.scTest.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTest)).EndInit();
            this.scTest.ResumeLayout(false);
            this.tsTest.ResumeLayout(false);
            this.tsTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpBasic;
        private System.Windows.Forms.TextBox txtBasic;
        private System.Windows.Forms.TabPage tpRequestSchema;
        private System.Windows.Forms.TextBox txtRequestSchema;
        private System.Windows.Forms.TabPage tpRequestSchemaSample;
        private System.Windows.Forms.TextBox txtRequestSchemaSample;
        private System.Windows.Forms.TabPage tpResponseSchema;
        private System.Windows.Forms.TabPage tpResponseSchemaSample;
        private System.Windows.Forms.TextBox txtResponseSchema;
        private System.Windows.Forms.TextBox txtResponseSchemaSample;
        private System.Windows.Forms.TabPage tpTest;
        private System.Windows.Forms.ToolStrip tsTest;
        private System.Windows.Forms.ToolStripButton btnExecuteTest;
        private System.Windows.Forms.SplitContainer scTest;
        private System.Windows.Forms.TextBox txtTestRequest;
        private System.Windows.Forms.TextBox txtTestResponse;
    }
}
