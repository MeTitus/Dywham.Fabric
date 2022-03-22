using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Providers.IO;

namespace Dywham.Fabric.Providers.Process
{
    public class ProcessProvider : IProcessProvider
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);


        // ReSharper disable once InconsistentNaming
        public IIOProvider IOProvider { get; set; }


        public void SetWindowTitle(int id, string title)
        {
            var process = System.Diagnostics.Process.GetProcessById(id);

            SetWindowText(process.MainWindowHandle, title);
        }

        public IList<System.Diagnostics.Process> GetByName(string name)
        {
            return System.Diagnostics.Process.GetProcessesByName(name);
        }

        public System.Diagnostics.Process GetById(int id)
        {
            return System.Diagnostics.Process.GetProcessById(id);
        }

        public int? Start(string path, StartProcessOptions options)
        {
            return Start(new ProcessStartInfo { FileName = path }, options);
        }

        public int? Start(ProcessStartInfo info, StartProcessOptions options)
        {
            if (options.OnStandardOutputDataReceived != null)
            {
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardError = true;
                info.RedirectStandardOutput = true;
            }

            if (options.SingleInstance)
            {
                var filename = IOProvider.GetFileNameWithoutExtension(info.FileName);
                var processes = GetByName(filename);

                if (processes.Any(x => x.MainModule != null && x.MainModule.FileName.Equals(info.FileName, StringComparison.OrdinalIgnoreCase))) return null;
            }

            var process = System.Diagnostics.Process.Start(info);

            if (process == null) return null;

            if (options.BindToCurrentProcess)
            {
                ChildProcessTracker.AddProcess(process);
            }

            if (options.OnExit != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    options.OnExit(sender, args);
                };
            }

            if (options.WaitUntilAvailable == true)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    while (options.IsAvailable != null && !options.IsAvailable() ||
                           options.IsAvailable == null && process.Handle == IntPtr.Zero)
                    {
                        Thread.Sleep(100);

                        process.Refresh();
                    }
                });

                if (options.MaxTimeToWait.HasValue)
                {
                    task.Wait(options.MaxTimeToWait.Value);
                }
                else
                {
                    task.Wait();
                }
            }

            if (!process.HasExited && options.OnStandardOutputDataReceived != null)
            {
                Task.Run(() =>
                {
                    var line = process.StandardOutput.ReadLine();

                    while (line != null)
                    {
                        options.OnStandardOutputDataReceived.Invoke(line);

                        line = process.StandardOutput.ReadLine();
                    }
                });
            }

            return process.Id;
        }

        public bool Exists(int id)
        {
            return System.Diagnostics.Process.GetProcesses().Any(x => x.Id == id);
        }

        public bool Exists(string name)
        {
            return System.Diagnostics.Process.GetProcesses().Any(x => x.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Kill(int id)
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(id).Kill();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            { }
        }

        public void KillByWindowTitle(string title)
        {
            var processes = System.Diagnostics.Process.GetProcesses();

            foreach (var process in processes)
            {
                if (!process.MainWindowTitle.Equals(title, StringComparison.OrdinalIgnoreCase)) continue;

                try
                {
                    process.Kill();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                { }
            }
        }
    }
}