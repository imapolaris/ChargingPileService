using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.SchedulerSystem
{
    using CPS.Infrastructure.Utils;
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            // 捕获全局异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            HostFactory.Run(x =>
            {
                x.Service<SchedulerServer>(s =>
                {
                    s.ConstructUsing(name => new SchedulerServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("充电桩调度服务");
                x.SetDisplayName("CPS.SchedulerSystem");
                x.SetServiceName("CPS.SchedulerSystem");

                x.UseLog4Net("./log4net.config", true);
            });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject);
            if (e.IsTerminating)
            {
                Logger.Warn("调度服务即将终止...");
            }
        }
    }
}
