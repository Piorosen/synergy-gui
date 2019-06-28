using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        SynergyCoreManager core;

        private void Form1_Load(object sender, EventArgs e)
        {
            core = new SynergyCoreManager("./synergy-core.exe", "synergy.sgc");
            core.OnChanged += Core_OnChanged;
            core.Run();
        }

        private void Core_OnChanged(object sender, MainLogModel e)
        {
            try
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    listBox1.Items.Add($"{e.Day} : {e.Time}");
                    listBox1.Items.Add($"{e.Log}");
                }));
            }
            catch
            {

            }
        }

        bool realClose = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!realClose)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000, "닫힘", "우헤헿", ToolTipIcon.Info);
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
            notifyIcon1.ShowBalloonTip(3000, "열림", "우헤헿", ToolTipIcon.Info);
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(3000, "열림", "우헤헿", ToolTipIcon.Info);
            this.Show();
            notifyIcon1.Visible = false;
        }
    }
}
