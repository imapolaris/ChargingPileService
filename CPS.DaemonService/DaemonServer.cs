using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.DaemonService
{
    public class DaemonServer
    {
        internal IList<AppInstance> appInsList { get; set; } = new List<AppInstance>();

        public DaemonServer()
        {
            appInsList = DaemonConfiguration.GetConfig();
        }

        public void Start()
        {
            appInsList.AsParallel().ForAll(_ => _.StartMonitor());
        }

        public void Stop()
        {
            appInsList?.AsParallel().ForAll(_ => _.StopMonitor());
        }
    }
}
