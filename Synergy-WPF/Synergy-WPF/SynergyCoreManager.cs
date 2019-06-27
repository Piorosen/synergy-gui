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

        bool Stop = false;
        void loop()
        {
            while (true)
            {
                if (Stop)
                {
                    return;
                }

                try
                {
                    while (!SynergyCore.StandardError.EndOfStream)
                    {
                        var err = SynergyCore.StandardError.ReadLine();
                        var day = err.Split('T')[0].Trim('[', ']');
                        var time = err.Split('T')[1].Trim('[', ']');
                        var log = err;

                        OnChanged?.Invoke(this, new MainLogModel
                        {
                            Day = day,
                            Time = time,
                            Log = log
                        });
                    }
                    while (!SynergyCore.StandardOutput.EndOfStream)
                    {
                        var err = SynergyCore.StandardError.ReadLine();
                        var day = err.Split('T')[0].Trim('[', ']');
                        var time = err.Split('T')[1].Trim('[', ']');
                        var log = err;

                        OnChanged?.Invoke(this, new MainLogModel
                        {
                            Day = day,
                            Time = time,
                            Log = log
                        });
                    }
                }
                catch
                {
                    return;
                }
                Task.Delay(50);
            }
        }


        public async void Run()
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
            await synergyloop;
        }

        public void Dispose()
        {
            SynergyCore.Close();
            Stop = true;
        }
    }
}
