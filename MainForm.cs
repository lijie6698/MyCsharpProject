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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 默认显示视觉检测界面
            ShowVisionControl();
        }

        private void btnVision_Click(object sender, EventArgs e)
        {
            ShowVisionControl();
        }

        private void btnPLC_Click(object sender, EventArgs e)
        {
            ShowPLCControl();
        }

        private void btnDatabase_Click(object sender, EventArgs e)
        {
            ShowDatabaseControl();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            ShowLogControl();
        }

        private void ShowVisionControl()
        {
            panelMain.Controls.Clear();
            VisionControl visionControl = new VisionControl();
            visionControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(visionControl);
        }

        private void ShowPLCControl()
        {
            panelMain.Controls.Clear();
            PLCControl plcControl = new PLCControl();
            plcControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(plcControl);
        }

        private void ShowDatabaseControl()
        {
            panelMain.Controls.Clear();
            DatabaseControl databaseControl = new DatabaseControl();
            databaseControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(databaseControl);
        }

        private void ShowLogControl()
        {
            panelMain.Controls.Clear();
            LogControl logControl = new LogControl();
            logControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(logControl);
        }
    }
}