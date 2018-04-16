using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.MessageSystem
{
    using Topshelf;
    using CPS.Infrastructure.Utils;

    class Program
    {
        static void Main(string[] args)
        {
            // 捕获全局异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            HostFactory.Run(x =>
            {
                x.Service<MessageServer>(s =>
                {
                    s.ConstructUsing(name => new MessageServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("充电桩消息服务");
                x.SetDisplayName("CPS.MessageSystem");
                x.SetServiceName("CPS.MessageSystem");

                x.UseLog4Net("./log4net.config", true);
            });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject);
            if (e.IsTerminating)
            {
                Logger.Warn("消息服务即将终止...");
            }
        }
    }
}
