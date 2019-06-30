using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Synergy_WinForm
{
    public class SynergyManager : IDisposable
    {
        readonly Process SynergyCore;
        
        public event EventHandler<LogModel> OnChanged;

        public SynergyManager(string path, string config)
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

        async void Loop()
        {
            while (!SynergyCore.HasExited)
            {
                while (!SynergyCore.StandardError.EndOfStream)
                {
                    OnChanged?.Invoke(this, new LogModel
                    {
                        Log = SynergyCore.StandardError.ReadLine()
                    });
                }

                while (!SynergyCore.StandardOutput.EndOfStream)
                {
                    var err = SynergyCore.StandardOutput.ReadLine();
                    int pos = err.IndexOf('T');
                    if (pos != -1)
                    {
                        var day = err.Substring(pos).Trim('[', ']');

                        var time = err.Substring(0, pos).Substring(err.IndexOf(']')).Trim('[', ']');
                        var log = err.Substring(err.IndexOf(']'));

                        OnChanged?.Invoke(this, new LogModel
                        {
                            Day = day,
                            Time = time,
                            Log = log
                        });
                    }
                    else
                    {
                        OnChanged?.Invoke(this, new LogModel
                        {
                            Log = err
                        });
                    }
                }

                await Task.Delay(50);
            }
        }


        public void Run()
        {
            if (SynergyCore.Start())
            {
                OnChanged?.Invoke(this, new LogModel
                {
                    Log = "Run..."
                });
                Loop();
            }
            else
            {
                OnChanged?.Invoke(this, new LogModel
                {
                    Log = "Fail"
                });
            }
        }

        public void Dispose()
        {
            if (!SynergyCore.HasExited)
            {
                SynergyCore.Kill();
            }
        }
    }
}
