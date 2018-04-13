using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService
{
    using Infrastructure.Utils;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    internal class AppInstance : ConfigurationSection, IDisposable
    {
        private const int DefaultMonitorFrequence = 5; // 监测频率默认时间间隔，单位：秒

        [Display(Name ="名称")]
        [ConfigurationProperty(nameof(Name), DefaultValue = "", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
        }

        [Display(Name = "描述")]
        [ConfigurationProperty(nameof(Desc), DefaultValue = "", IsRequired = true)]
        public string Desc
        {
            get
            {
                return (string)base["Desc"];
            }
        }

        [Display(Name ="启动路径")]
        [ConfigurationProperty(nameof(StartupPath), DefaultValue = "", IsRequired = true)]
        public string StartupPath
        {
            get
            {
                return (string)base["StartupPath"];
            }
        }

        [Display(Name= "监测频率，单位：秒")]
        [ConfigurationProperty(nameof(MonitorFrequence), DefaultValue = 5, IsRequired = false)]
        public int MonitorFrequence
        {
            get
            {
                try
                {
                    return (int)base["MonitorFrequence"];
                }
                catch
                {
                    return DefaultMonitorFrequence;
                }
            }
        }
        

        /// <summary>
        /// 是否可以启动
        /// </summary>
        public bool IsCanLaunch
        {
            get
            {
                return !string.IsNullOrEmpty(Name)
                    && !string.IsNullOrEmpty(StartupPath)
                    && File.Exists(StartupPath);
            }
        }

        /// <summary>
        /// 已启动
        /// </summary>
        public bool HasLaunched
        {
            get
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.ProcessName == this.Name && process.MainModule.FileName == this.StartupPath)
                        return true;
                }

                return false;
            }
        }

        Timer _timer = null;
        /// <summary>
        /// 开启监控
        /// </summary>
        public void StartMonitor()
        {
            _timer = new Timer((state) =>
            {
                try
                {
                    if (!HasLaunched)
                    {
                        LaunchApp();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }, null, 0, this.MonitorFrequence * 1000);
        }

        private void LaunchApp()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = this.StartupPath;
            startInfo.Arguments = "fromDeamon"; // 表示由守护进程启动
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = true;
            //startInfo.UseShellExecute = true;
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// 停止监控
        /// </summary>
        public void StopMonitor()
        {
            _timer?.Change(Timeout.Infinite, 0);
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放非托管资源
                }

                _timer?.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
