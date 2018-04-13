using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService.Startup
{
    using Infrastructure.Utils;
    using System.Configuration;
    using System.Diagnostics;
    using System.Security.Principal;

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("start launch daemon process...");

            var result = LaunchApp();
            if (result)
                Logger.Info("守护进程启动成功！");
            else
                Logger.Info("守护进程启动失败！");

            Logger.Info("launch daemon process completed...");
        }

        static bool LaunchApp()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ConfigurationManager.AppSettings["startupApp"];
                //startInfo.Arguments = "fromStartup"; // 表示由启动程序启动
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = true;
                startInfo.UseShellExecute = false;

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                    startInfo.Verb = "runas"; // 设置启动动作，确保以管理员身份运行

                process.StartInfo = startInfo;

                return process.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
}
