using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService
{
    using Topshelf;
    using CPS.Infrastructure.Utils;
    using System.Diagnostics;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            // 检测守护进程是否已启动
            var fullPath = typeof(Program).Assembly.ManifestModule.FullyQualifiedName;
            var processName = Process.GetCurrentProcess().ProcessName;

            var processes = Process.GetProcesses();
            var result = processes.Where(_ => _.ProcessName == processName && _.MainModule.FileName == fullPath);
            if (result != null && result.Count() > 1)
            {
                Logger.Info("守护进程已经启动！");
                Environment.Exit(0);
            }

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
                //x.StartAutomatically();
                x.StartAutomaticallyDelayed();

                x.UseLog4Net("./log4net.config", true);
            });

            Console.Read();
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
