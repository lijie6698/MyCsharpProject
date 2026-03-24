using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace DeepTrainingTest
{
    public partial class VisionControl : UserControl
    {
        public VisionControl()
        {
            InitializeComponent();
            // 初始化视觉检测相关组件
            InitVisionComponents();
        }

        private void InitVisionComponents()
        {
            // 这里可以添加视觉检测初始化代码
            // 例如：创建Halcon窗口控件，加载模型等
            Label lblInfo = new Label();
            lblInfo.Text = "视觉检测控件";
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(20, 20);
            this.Controls.Add(lblInfo);
        }

        // 视觉检测相关方法
        private void btnDetect_Click(object sender, EventArgs e)
        {
            // 实现视觉检测逻辑
            MessageBox.Show("执行视觉检测");
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
            // VisionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "VisionControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
        }
        #endregion
    }
}