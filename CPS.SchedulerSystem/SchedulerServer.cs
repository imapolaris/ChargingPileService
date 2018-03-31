using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem
{
    using CPS.Infrastructure.Utils;
    using Quartz;
    using CPS.SchedulerSystem.Jobs;
    using Quartz.Impl;
    using Quartz.Impl.Calendar;
    using Quartz.Spi;
    using Quartz.Plugin.History;

    public class SchedulerServer
    {
        public SchedulerServer()
        {
            
        }

        public void Start()
        {
            JobManager.Instance.GetScheduler("sched1").Start();

            Logger.Info("scheduler system started...");
        }

        public void Stop()
        {
            Logger.Info("scheduler system stopped...");
        }
    }
}
