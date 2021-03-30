
namespace QpTestClient
{
    partial class ConnectForm
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbConnectType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pgOptions = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(417, 11);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbConnectType
            // 
            this.cbConnectType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbConnectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConnectType.FormattingEnabled = true;
            this.cbConnectType.Items.AddRange(new object[] {
            "TCP",
            "命名管道",
            "串口"});
            this.cbConnectType.Location = new System.Drawing.Point(101, 12);
            this.cbConnectType.Name = "cbConnectType";
            this.cbConnectType.Size = new System.Drawing.Size(310, 28);
            this.cbConnectType.TabIndex = 2;
            this.cbConnectType.SelectedIndexChanged += new System.EventHandler(this.cbConnectType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "连接方式:";
            // 
            // pgOptions
            // 
            this.pgOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgOptions.Location = new System.Drawing.Point(12, 46);
            this.pgOptions.Name = "pgOptions";
            this.pgOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgOptions.Size = new System.Drawing.Size(499, 444);
            this.pgOptions.TabIndex = 4;
            this.pgOptions.ToolbarVisible = false;
            // 
            // ConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 502);
            this.Controls.Add(this.pgOptions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbConnectType);
            this.Controls.Add(this.btnConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加连接";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cbConnectType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PropertyGrid pgOptions;
    }
}