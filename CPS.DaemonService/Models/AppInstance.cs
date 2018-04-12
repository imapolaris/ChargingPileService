using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService.Models
{
    internal class AppInstance
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string StartupPath { get; set; }
        // 监测频率，单位：秒
        public int MonitorFrequence { get; set; }
    }
}
