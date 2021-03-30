
namespace QpTestClient
{
    partial class LoginForm
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
            this.gbConnectMethod = new System.Windows.Forms.GroupBox();
            this.tcConnectMethod = new System.Windows.Forms.TabControl();
            this.tpTcp = new System.Windows.Forms.TabPage();
            this.nudTcpPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTcpHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tpPipeline = new System.Windows.Forms.TabPage();
            this.tpSerialPort = new System.Windows.Forms.TabPage();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cbCompress = new System.Windows.Forms.CheckBox();
            this.cbEncrypt = new System.Windows.Forms.CheckBox();
            this.gbConnectMethod.SuspendLayout();
            this.tcConnectMethod.SuspendLayout();
            this.tpTcp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTcpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // gbConnectMethod
            // 
            this.gbConnectMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbConnectMethod.Controls.Add(this.tcConnectMethod);
            this.gbConnectMethod.Location = new System.Drawing.Point(12, 12);
            this.gbConnectMethod.Name = "gbConnectMethod";
            this.gbConnectMethod.Size = new System.Drawing.Size(502, 247);
            this.gbConnectMethod.TabIndex = 0;
            this.gbConnectMethod.TabStop = false;
            this.gbConnectMethod.Text = "连接方式";
            // 
            // tcConnectMethod
            // 
            this.tcConnectMethod.Controls.Add(this.tpTcp);
            this.tcConnectMethod.Controls.Add(this.tpPipeline);
            this.tcConnectMethod.Controls.Add(this.tpSerialPort);
            this.tcConnectMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcConnectMethod.Location = new System.Drawing.Point(3, 23);
            this.tcConnectMethod.Name = "tcConnectMethod";
            this.tcConnectMethod.SelectedIndex = 0;
            this.tcConnectMethod.Size = new System.Drawing.Size(496, 221);
            this.tcConnectMethod.TabIndex = 2;
            // 
            // tpTcp
            // 
            this.tpTcp.Controls.Add(this.nudTcpPort);
            this.tpTcp.Controls.Add(this.label3);
            this.tpTcp.Controls.Add(this.txtTcpHost);
            this.tpTcp.Controls.Add(this.label2);
            this.tpTcp.Location = new System.Drawing.Point(4, 29);
            this.tpTcp.Name = "tpTcp";
            this.tpTcp.Padding = new System.Windows.Forms.Padding(3);
            this.tpTcp.Size = new System.Drawing.Size(488, 188);
            this.tpTcp.TabIndex = 0;
            this.tpTcp.Text = "TCP";
            this.tpTcp.UseVisualStyleBackColor = true;
            // 
            // nudTcpPort
            // 
            this.nudTcpPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudTcpPort.Location = new System.Drawing.Point(142, 88);
            this.nudTcpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudTcpPort.Name = "nudTcpPort";
            this.nudTcpPort.Size = new System.Drawing.Size(219, 27);
            this.nudTcpPort.TabIndex = 3;
            this.nudTcpPort.Value = new decimal(new int[] {
            3011,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "端口:";
            // 
            // txtTcpHost
            // 
            this.txtTcpHost.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTcpHost.Location = new System.Drawing.Point(142, 55);
            this.txtTcpHost.Name = "txtTcpHost";
            this.txtTcpHost.Size = new System.Drawing.Size(219, 27);
            this.txtTcpHost.TabIndex = 1;
            this.txtTcpHost.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "主机:";
            // 
            // tpPipeline
            // 
            this.tpPipeline.Location = new System.Drawing.Point(4, 29);
            this.tpPipeline.Name = "tpPipeline";
            this.tpPipeline.Padding = new System.Windows.Forms.Padding(3);
            this.tpPipeline.Size = new System.Drawing.Size(488, 188);
            this.tpPipeline.TabIndex = 1;
            this.tpPipeline.Text = "命名管道";
            this.tpPipeline.UseVisualStyleBackColor = true;
            // 
            // tpSerialPort
            // 
            this.tpSerialPort.Location = new System.Drawing.Point(4, 29);
            this.tpSerialPort.Name = "tpSerialPort";
            this.tpSerialPort.Padding = new System.Windows.Forms.Padding(3);
            this.tpSerialPort.Size = new System.Drawing.Size(488, 188);
            this.tpSerialPort.TabIndex = 2;
            this.tpSerialPort.Text = "串口";
            this.tpSerialPort.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(413, 324);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 268);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "密码：";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(19, 291);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(488, 27);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "HelloQP";
            // 
            // cbCompress
            // 
            this.cbCompress.AutoSize = true;
            this.cbCompress.Location = new System.Drawing.Point(19, 324);
            this.cbCompress.Name = "cbCompress";
            this.cbCompress.Size = new System.Drawing.Size(61, 24);
            this.cbCompress.TabIndex = 2;
            this.cbCompress.Text = "压缩";
            this.cbCompress.UseVisualStyleBackColor = true;
            // 
            // cbEncrypt
            // 
            this.cbEncrypt.AutoSize = true;
            this.cbEncrypt.Location = new System.Drawing.Point(86, 324);
            this.cbEncrypt.Name = "cbEncrypt";
            this.cbEncrypt.Size = new System.Drawing.Size(61, 24);
            this.cbEncrypt.TabIndex = 2;
            this.cbEncrypt.Text = "加密";
            this.cbEncrypt.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 361);
            this.Controls.Add(this.cbEncrypt);
            this.Controls.Add(this.cbCompress);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbConnectMethod);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录 ";
            this.gbConnectMethod.ResumeLayout(false);
            this.tcConnectMethod.ResumeLayout(false);
            this.tpTcp.ResumeLayout(false);
            this.tpTcp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTcpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConnectMethod;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TabControl tcConnectMethod;
        private System.Windows.Forms.TabPage tpTcp;
        private System.Windows.Forms.TabPage tpPipeline;
        private System.Windows.Forms.TabPage tpSerialPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox cbCompress;
        private System.Windows.Forms.CheckBox cbEncrypt;
        private System.Windows.Forms.TextBox txtTcpHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudTcpPort;
        private System.Windows.Forms.Label label3;
    }
}