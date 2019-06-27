using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy_WPF
{
    public class SynergyCoreManager : IDisposable
    {
        readonly Process SynergyCore;

        public SynergyCoreManager(string path, string config)
        {
            SynergyCore = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = $"--server --config {config}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
        }


        public void Run()
        {
            SynergyCore.Start();
        }

        public void Dispose()
        {
            SynergyCore.Close();
        }
    }
}
