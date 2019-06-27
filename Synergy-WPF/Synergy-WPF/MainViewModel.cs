using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Synergy_WPF
{
    public class MainViewModel
    {
        public ObservableCollection<MainLogModel> Log { get; private set; } = new ObservableCollection<MainLogModel>();

        SynergyCoreManager core;

        Dispatcher patcher;

        public MainViewModel(Dispatcher d)
        {
            core = new SynergyCoreManager("./synergy-core.exe", "synergy.sgc");
            core.OnChanged += Core_OnChanged;
            core.Run();
        }

        private void Core_OnChanged(object sender, MainLogModel e)
        {
            try
            {
                patcher.Invoke(() =>
                {
                    Log.Add(e);

                });
            }
            catch
            {

            }
        }

        public void Close()
        {
            core.Dispose();
        }
    }
}
