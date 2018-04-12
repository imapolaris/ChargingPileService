using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService
{
    using Topshelf;
    using Topshelf.Logging;
    using CPS.Infrastructure.Utils;

    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            HostFactory.Run(x =>
            {
                x.Service<DaemonServer>(s =>
                {
                    s.ConstructUsing(name => new DaemonServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("进程守护服务");
                x.SetDisplayName("CPS.DaemonService");
                x.SetServiceName("CPS.DaemonService");

                x.UseLog4Net("./log4net.config", true);
            });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject);
            if (e.IsTerminating)
            {
                Logger.Warn("通信服务即将终止...");
            }
        }
    }
}
