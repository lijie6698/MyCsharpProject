using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeepTrainingTest
{
    public partial class LogControl : UserControl
    {
        public LogControl()
        {
            InitializeComponent();
            // 初始化日志控件相关组件
            InitLogComponents();
        }

        private void InitLogComponents()
        {
            // 这里可以添加日志控件初始化代码
            Label lblInfo = new Label();
            lblInfo.Text = "日志记录控件";
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(20, 20);
            this.Controls.Add(lblInfo);
            
            // 添加一个多行文本框用于显示日志
            TextBox txtLog = new TextBox();
            txtLog.Multiline = true;
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Location = new Point(20, 50);
            txtLog.Size = new Size(750, 500);
            txtLog.BackColor = Color.White;
            this.Controls.Add(txtLog);
        }

        // 日志记录相关方法
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            // 清空日志
            // 在实际实现中，这里会清除日志文本框的内容
            MessageBox.Show("清空日志");
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            // 保存日志到文件
            MessageBox.Show("保存日志");
        }

        #region 组件设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
        }
        #endregion
    }
}