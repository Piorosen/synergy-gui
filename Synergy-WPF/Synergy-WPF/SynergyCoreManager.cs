using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Synergy_WPF
{
    public class SynergyCoreManager : IDisposable
    {
        readonly Process SynergyCore;
        Task synergyloop;
        
        public event EventHandler<MainLogModel> OnChanged;

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

        void loop()
        {
            while (!SynergyCore.StandardError.EndOfStream)
            {
                var err = SynergyCore.StandardError.ReadLine();
                var day = err.Split('T')[0].Trim('[', ']');
                var time = err.Split('T')[1].Trim('[', ']');
                var log = Regex.Split(err, "INFO: ")[1];

                OnChanged?.Invoke(this, new MainLogModel
                {
                    Day = day,
                    Time = time,
                    Log = log
                });
            }
        }


        public void Run()
        {
            synergyloop = Task.Run(() =>
            {
                if (SynergyCore.Start())
                {
                    loop();
                    OnChanged?.Invoke(this, new MainLogModel
                    {
                        Log = "Run..."
                    });
                }
                else
                {
                    OnChanged?.Invoke(this, new MainLogModel
                    {
                        Log = "Fail"
                    });
                }
            });
            synergyloop.Start();
        }

        public void Dispose()
        {
            SynergyCore.Close();
            synergyloop.Dispose();
        }
    }
}
