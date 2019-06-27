using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Synergy_WPF
{
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            SetNotifycation();

            DataContext = viewModel = new MainViewModel();
        }

        NotifyIcon notify = new NotifyIcon();
        bool realClose = false;
        void SetNotifycation()
        {
            try
            {
                ContextMenu menu = new ContextMenu();
                notify.ContextMenu = menu;
                notify.Icon = System.Drawing.SystemIcons.Warning;
                notify.Visible = false;
                notify.Text = "Synergy";
                notify.DoubleClick += (s, e) =>
                {
                    this.Show();
                };

                var item = new MenuItem
                {
                    Index = 0,
                    Text = "프로그램 종료",
                };
                item.Click += (s, e) =>
                {
                    realClose = true;
                    this.Close();
                };
                menu.MenuItems.Add(item);
            }
            catch { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!realClose)
            {
                notify.Visible = true;
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
