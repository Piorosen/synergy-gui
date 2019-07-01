using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synergy_WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            notifyIcon1.Icon = System.Drawing.SystemIcons.Warning;
        }

        SynergyManager core;

        private void Form1_Load(object sender, EventArgs e)
        {
            var allp = Process.GetProcessesByName("synergy-core");
            foreach (var p in allp)
            {
                // listBox1.Items.Add($"{p.Id} : Synergy-core 강제 종료");
                p.Kill();
            }

            core = new SynergyManager("./synergy/synergy-core.exe", "./synergy/synergy.sgc");
            core.OnChanged += Core_OnChanged;
            core.Run();
        }

        private void Core_OnChanged(object sender, LogModel e)
        {
            try
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    if (!string.IsNullOrEmpty(e.Day) && !string.IsNullOrEmpty(e.Time))
                    {
                        // listBox1.Items.Add($"{e.Day} : {e.Time}");
                    }
                    // listBox1.Items.Add($"{e.Log}");
                }));
            }
            catch { }
        }

        bool realClose = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!realClose)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000, "창 최소화", "시너지 프로그램을 최소화 하였습니다. 최대화 할려시 더블클릭 및 우클릭 창열기 해주세요.", ToolTipIcon.Info);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            core.Dispose();
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            realClose = true;
            this.Close();
        }

        private void 창열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(3000, "창 오픈", "시너지 프로그램을 열었습니다.", ToolTipIcon.Info);
            this.Show();
            notifyIcon1.Visible = false;
        }
    }
}
