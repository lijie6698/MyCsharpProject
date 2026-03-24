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
        private System.Windows.Forms.TextBox txtLogDisplay;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.Button btnSaveLogs;
        private System.Windows.Forms.Label lblLogLevelFilter;
        private System.Windows.Forms.ComboBox cbLogLevelFilter;
        private System.Windows.Forms.Label lblLogCount;
        private System.Windows.Forms.CheckBox chkAutoScroll;

        public LogControl()
        {
            InitializeComponent();
            // 初始化日志控件相关组件
            InitLogComponents();
            
            // 订阅日志事件
            Logger.LogAdded += Logger_LogAdded;
            Logger.LogCleared += Logger_LogCleared;
        }

        private void InitLogComponents()
        {
            // 日志级别过滤标签
            lblLogLevelFilter = new Label();
            lblLogLevelFilter.Text = "日志级别:";
            lblLogLevelFilter.Location = new Point(20, 20);
            lblLogLevelFilter.AutoSize = true;
            this.Controls.Add(lblLogLevelFilter);

            // 日志级别过滤下拉框
            cbLogLevelFilter = new ComboBox();
            cbLogLevelFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLogLevelFilter.Items.AddRange(Enum.GetNames(typeof(LogLevel)));
            cbLogLevelFilter.Items.Add("全部");
            cbLogLevelFilter.SelectedIndex = cbLogLevelFilter.Items.Count - 1; // 默认选中"全部"
            cbLogLevelFilter.Location = new Point(100, 17);
            cbLogLevelFilter.Size = new Size(100, 21);
            cbLogLevelFilter.SelectedIndexChanged += CbLogLevelFilter_SelectedIndexChanged;
            this.Controls.Add(cbLogLevelFilter);

            // 日志数量标签
            lblLogCount = new Label();
            lblLogCount.Text = "日志数量: 0";
            lblLogCount.Location = new Point(220, 20);
            lblLogCount.AutoSize = true;
            this.Controls.Add(lblLogCount);

            // 自动滚动复选框
            chkAutoScroll = new CheckBox();
            chkAutoScroll.Text = "自动滚动";
            chkAutoScroll.Checked = true;
            chkAutoScroll.Location = new Point(350, 18);
            chkAutoScroll.AutoSize = true;
            this.Controls.Add(chkAutoScroll);

            // 日志显示文本框
            txtLogDisplay = new TextBox();
            txtLogDisplay.Multiline = true;
            txtLogDisplay.ReadOnly = true;
            txtLogDisplay.ScrollBars = ScrollBars.Vertical;
            txtLogDisplay.Location = new Point(20, 50);
            txtLogDisplay.Size = new Size(750, 480);
            txtLogDisplay.Font = new Font("Consolas", 9F, FontStyle.Regular);
            txtLogDisplay.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.Add(txtLogDisplay);

            // 清空日志按钮
            btnClearLogs = new Button();
            btnClearLogs.Text = "清空日志";
            btnClearLogs.Location = new Point(600, 540);
            btnClearLogs.Size = new Size(80, 30);
            btnClearLogs.Click += BtnClearLogs_Click;
            this.Controls.Add(btnClearLogs);

            // 保存日志按钮
            btnSaveLogs = new Button();
            btnSaveLogs.Text = "保存日志";
            btnSaveLogs.Location = new Point(690, 540);
            btnSaveLogs.Size = new Size(80, 30);
            btnSaveLogs.Click += BtnSaveLogs_Click;
            this.Controls.Add(btnSaveLogs);

            // 初始显示现有日志
            UpdateLogDisplay();
        }

        private void Logger_LogAdded(object sender, LogAddedEventArgs e)
        {
            // 如果需要跨线程访问UI，这里需要Invoke
            if (txtLogDisplay.InvokeRequired)
            {
                txtLogDisplay.Invoke(new Action(() => AppendLog(e.LogEntry)));
            }
            else
            {
                AppendLog(e.LogEntry);
            }
        }

        private void Logger_LogCleared(object sender, EventArgs e)
        {
            if (txtLogDisplay.InvokeRequired)
            {
                txtLogDisplay.Invoke(new Action(() => ClearLogDisplay()));
            }
            else
            {
                ClearLogDisplay();
            }
        }

        private void AppendLog(LogEntry entry)
        {
            // 应用过滤器
            if (!ShouldDisplayLog(entry))
                return;

            // 添加日志到显示框
            txtLogDisplay.AppendText(entry.ToString() + Environment.NewLine);
            
            // 更新日志计数
            UpdateLogCount();
            
            // 自动滚动到底部
            if (chkAutoScroll.Checked)
            {
                txtLogDisplay.SelectionStart = txtLogDisplay.TextLength;
                txtLogDisplay.ScrollToCaret();
            }
        }

        private bool ShouldDisplayLog(LogEntry entry)
        {
            string selectedFilter = cbLogLevelFilter.SelectedItem.ToString();
            if (selectedFilter == "全部")
                return true;
            
            return entry.Level.ToString() == selectedFilter;
        }

        private void UpdateLogCount()
        {
            int count = Logger.Logs.Count;
            // 应用过滤器后的实际显示数量
            string selectedFilter = cbLogLevelFilter.SelectedItem.ToString();
            if (selectedFilter != "全部")
            {
                LogLevel filterLevel = (LogLevel)Enum.Parse(typeof(LogLevel), selectedFilter);
                count = Logger.Logs.Count(l => l.Level == filterLevel);
            }
            
            lblLogCount.Text = $"日志数量: {count}";
        }

        private void UpdateLogDisplay()
        {
            txtLogDisplay.Clear();
            foreach (var entry in Logger.Logs)
            {
                if (ShouldDisplayLog(entry))
                {
                    txtLogDisplay.AppendText(entry.ToString() + Environment.NewLine);
                }
            }
            
            // 自动滚动到底部
            if (chkAutoScroll.Checked && txtLogDisplay.TextLength > 0)
            {
                txtLogDisplay.SelectionStart = txtLogDisplay.TextLength;
                txtLogDisplay.ScrollToCaret();
            }
            
            UpdateLogCount();
        }

        private void CbLogLevelFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLogDisplay();
        }

        private void BtnClearLogs_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清空所有日志吗？", "确认清空", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Logger.Clear();
            }
        }

        private void BtnSaveLogs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "文本文件|*.txt|所有文件|*.*";
                saveDialog.FileName = $"应用日志_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filteredLogs = string.Join(Environment.NewLine, 
                            Logger.Logs.Where(ShouldDisplayLog).Select(l => l.ToString()));
                        
                        System.IO.File.WriteAllText(saveDialog.FileName, filteredLogs);
                        MessageBox.Show("日志已成功保存！", "保存成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存日志时出错: {ex.Message}", "保存失败", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearLogDisplay()
        {
            txtLogDisplay.Clear();
            UpdateLogCount();
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
            this.PerformLayout();
        }
        #endregion
    }
}