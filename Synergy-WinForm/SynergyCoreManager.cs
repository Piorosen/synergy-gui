using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Synergy_WinForm
{
    public class SynergyCoreManager : IDisposable
    {
        readonly Process SynergyCore;
        
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
            while (!Stop)
            {
                try
                {
                    while (!SynergyCore.StandardOutput.EndOfStream)
                    {
                        var err = SynergyCore.StandardOutput.ReadLine();
                        int pos = err.IndexOf('T');
                        if (pos != -1)
                        {
                            var day = err.Substring(pos).Trim('[', ']');

                            var time = err.Substring(0, pos).Substring(err.IndexOf(']')).Trim('[', ']');
                            var log = err.Substring(err.IndexOf(']'));

                            OnChanged?.Invoke(this, new MainLogModel
                            {
                                Day = day,
                                Time = time,
                                Log = log
                            });
                        }
                        else
                        {
                            OnChanged?.Invoke(this, new MainLogModel
                            {
                                Log = err
                            });
                        }
                    }
                    
                }
                catch
                {
                    OnChanged?.Invoke(this, new MainLogModel
                    {
                        Day = "",
                        Time = "",
                        Log = "Core 강제 종료 됨."
                    });
                    return;
                }
                Task.Delay(50);
            }
            SynergyCore.Close();
        }


        public async void Run()
        {
            await Task.Run(() =>
            {
                if (SynergyCore.Start())
                {
                    OnChanged?.Invoke(this, new MainLogModel
                    {
                        Log = "Run..."
                    });
                    loop();
                    
                }
                else
                {
                    OnChanged?.Invoke(this, new MainLogModel
                    {
                        Log = "Fail"
                    });
                }
            });
        }

        public void Dispose()
        {
            Stop = true;
            if (!SynergyCore.HasExited)
            {
                SynergyCore.Kill();
            }
        }
    }
}
