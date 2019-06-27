using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy_WPF
{
    public class MainViewModel
    {
        public ObservableCollection<MainLogModel> Log { get; private set; } = new ObservableCollection<MainLogModel>();

        public MainViewModel()
        {

        }

    }
}
