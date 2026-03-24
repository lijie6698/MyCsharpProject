namespace DeepTrainingTest
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </private
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnDatabase = new System.Windows.Forms.Button();
            this.btnPLC = new System.Windows.Forms.Button();
            this.btnVision = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Location = new System.Drawing.Point(150, 12);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(800, 600);
            this.panelMain.TabIndex = 0;
            // 
            // btnLog
            // 
            this.btnLog.Location = new System.Drawing.Point(12, 200);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(120, 40);
            this.btnLog.TabIndex = 4;
            this.btnLog.Text = "日志记录";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // btnDatabase
            // 
            this.btnDatabase.Location = new System.Drawing.Point(12, 150);
            this.btnDatabase.Name = "btnDatabase";
            this.btnDatabase.Size = new System.Drawing.Size(120, 40);
            this.btnDatabase.TabIndex = 3;
            this.btnDatabase.Text = "数据库";
            this.btnDatabase.UseVisualStyleBackColor = true;
            this.btnDatabase.Click += new System.EventHandler(this.btnDatabase_Click);
            // 
            // btnPLC
            // 
            this.btnPLC.Location = new System.Drawing.Point(12, 100);
            this.btnPLC.Name = "btnPLC";
            this.btnPLC.Size = new System.Drawing.Size(120, 40);
            this.btnPLC.TabIndex = 2;
            this.btnPLC.Text = "PLC连接";
            this.btnPLC.UseVisualStyleBackColor = true;
            this.btnPLC.Click += new System.EventHandler(this.btnPLC_Click);
            // 
            // btnVision
            // 
            this.btnVision.Location = new System.Drawing.Point(12, 50);
            this.btnVision.Name = "btnVision";
            this.btnVision.Size = new System.Drawing.Size(120, 40);
            this.btnVision.TabIndex = 1;
            this.btnVision.Text = "视觉检测";
            this.btnVision.UseVisualStyleBackColor = true;
            this.btnVision.Click += new System.EventHandler(this.btnVision_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 624);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.btnDatabase);
            this.Controls.Add(this.btnPLC);
            this.Controls.Add(this.btnVision);
            this.Controls.Add(this.panelMain);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "主界面";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnDatabase;
        private System.Windows.Forms.Button btnPLC;
        private System.Windows.Forms.Button btnVision;
    }
}