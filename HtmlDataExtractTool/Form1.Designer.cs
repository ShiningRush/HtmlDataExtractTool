namespace HtmlDataExtractTool
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelectImportDir = new System.Windows.Forms.Button();
            this.txtImportDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectExportDir = new System.Windows.Forms.Button();
            this.txtExportDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectImportDir
            // 
            this.btnSelectImportDir.Location = new System.Drawing.Point(476, 21);
            this.btnSelectImportDir.Name = "btnSelectImportDir";
            this.btnSelectImportDir.Size = new System.Drawing.Size(75, 23);
            this.btnSelectImportDir.TabIndex = 0;
            this.btnSelectImportDir.Text = "浏览";
            this.btnSelectImportDir.UseVisualStyleBackColor = true;
            this.btnSelectImportDir.Click += new System.EventHandler(this.btnSelectImportDir_Click);
            // 
            // txtImportDir
            // 
            this.txtImportDir.Location = new System.Drawing.Point(89, 23);
            this.txtImportDir.Name = "txtImportDir";
            this.txtImportDir.Size = new System.Drawing.Size(381, 21);
            this.txtImportDir.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "取证文件";
            // 
            // btnSelectExportDir
            // 
            this.btnSelectExportDir.Location = new System.Drawing.Point(477, 63);
            this.btnSelectExportDir.Name = "btnSelectExportDir";
            this.btnSelectExportDir.Size = new System.Drawing.Size(75, 23);
            this.btnSelectExportDir.TabIndex = 0;
            this.btnSelectExportDir.Text = "浏览";
            this.btnSelectExportDir.UseVisualStyleBackColor = true;
            this.btnSelectExportDir.Click += new System.EventHandler(this.btnSelectExportDir_Click);
            // 
            // txtExportDir
            // 
            this.txtExportDir.Location = new System.Drawing.Point(90, 65);
            this.txtExportDir.Name = "txtExportDir";
            this.txtExportDir.Size = new System.Drawing.Size(381, 21);
            this.txtExportDir.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "导出路径";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(440, 110);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始搜集";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 145);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtExportDir);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSelectExportDir);
            this.Controls.Add(this.txtImportDir);
            this.Controls.Add(this.btnSelectImportDir);
            this.Name = "Form1";
            this.RightToLeftLayout = true;
            this.Text = "HtmlDataExtractTool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectImportDir;
        private System.Windows.Forms.TextBox txtImportDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectExportDir;
        private System.Windows.Forms.TextBox txtExportDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
    }
}

