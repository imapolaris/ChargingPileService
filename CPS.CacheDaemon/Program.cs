using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon
{
    using Infrastructure.Utils;
    using System.Threading;
    using Topshelf;

    class Program
    {
        private static readonly string InstanceName = @"CacheDaemon_{02FF336F-4329-4AF3-AD56-D6AE505A2217}";

        static void Main(string[] args)
        {
            // 捕获全局异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            bool createdNew;
            Mutex _mutext = new Mutex(true, InstanceName, out createdNew);
            if (!createdNew)
            {
                Console.WriteLine("One daemon instance is running...");
                Environment.Exit(-1);
            }

            HostFactory.Run(x => {
                x.Service<RedisCacheDaemon>(sc => {
                    sc.ConstructUsing(() => new RedisCacheDaemon());

                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                });

                x.StartManually();
                x.RunAsLocalSystem();

                x.SetDescription("cps--cache daemon service");
                x.SetDisplayName("CPS.CacheDaemon");
                x.SetInstanceName("CPS.CacheDaemon");

                x.UseLog4Net("./log4net.config", watchFile: true);

                x.SetHelpTextPrefix("topshelf--");
            });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject);
            if (e.IsTerminating)
            {
                Logger.Warn("缓存系统即将终止运行...");
            }
        }
    }
}
