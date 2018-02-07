using CPS.Communication.Service;
using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Topshelf;

namespace CPS.Communication.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<CommunicationServer>(s =>
                {
                    s.ConstructUsing(name => new CommunicationServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("充电桩通信服务");
                x.SetDisplayName("CPS.Communication.Server");
                x.SetServiceName("CPS.Communication.Server");

                x.UseLog4Net("./log4net.config", true);
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
            Logger.Info($"启动结果：{exitCode}");
        }
    }

    public class CommunicationServer : IDisposable
    {
        readonly ChargingService MyService;// = ChargingService.Instance;

        public CommunicationServer() { }

        public void Start()
        {
            InitEnv();
            PrintStartInfo();

            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    break;
                if (input.ToLower().Equals("exit"))
                    break;
                else if (input.ToLower().Equals("detail"))
                    Console.WriteLine(MyService.MyServer);
                else if (input.ToLower().Equals("clear"))
                {
                    Console.Clear();
                    PrintStartInfo();
                }
                else if (input.ToLower().Equals("reboot_c"))
                {
                    Reboot();
                }
                else if (input.ToLower().Equals("setep_c"))
                {
                    SetElecPrice();
                }
                else if (input.ToLower().Equals("setsp_c"))
                {
                    SetServicePrice();
                }
                else if (input.ToLower().Equals("setrI_c"))
                {

                }
                else if (input.ToLower().Equals("startc"))
                {
                    StartCharging();
                }
                else if (input.ToLower().Equals("stopc"))
                {
                    StopCharging();
                }
            }
        }

        public void Stop()
        {
            PrintStopInfo();
            Dispose();
        }

        private void InitEnv()
        {
            Console.Title = "充电桩通信服务控制台";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.BufferWidth = Console.LargestWindowWidth / 2;
        }

        private void PrintStartInfo()
        {
            Console.Clear();
            Console.WriteLine("-----------------------开始充电桩通信服务-----------------------");
            Console.WriteLine($"启动时间：{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}");
            Console.WriteLine($"版本：{"Ver 1.0"}");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("服务正在运行 ......(输入 Control+C 停止服务)\n");
        }

        private void PrintStopInfo()
        {
            Console.WriteLine("\n-----------------------结束充电桩通信服务-----------------------");
        }

        private void Server_ServerStopped(object sender, Service.Events.ServerStoppedEventArgs args)
        {
            Console.WriteLine("Server Stopped!");
        }

        private void Server_ServerStarted(object sender, Service.Events.ServerStartedEventArgs args)
        {
            Console.WriteLine("Server Started!");
        }

        private void Server_ClientAccepted(object sender, Service.Events.ClientAcceptedEventArgs args)
        {
            Console.WriteLine($"----客户端 {args.CurClient.ID} 已连接！");
        }

        private void Server_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            Console.WriteLine("Error:" + args.ErrorMessage);
        }

        /// <summary>
        /// 启动redis
        /// </summary>
        /// <returns></returns>
        private bool LaunchRedis()
        {
            return true;
        }

        #region ====测试====
        private async void Reboot()
        {
            var state = await MyService.Reboot("1234567890AbcBCa");
            if (state)
                Console.WriteLine("充电桩重启成功！");
            else
                Console.WriteLine("充电桩重启失败！");
        }

        private async void SetElecPrice()
        {
            var state = await MyService.SetElecPrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置电价成功！");
            else
                Console.WriteLine("设置电价失败！");
        }

        private async void SetServicePrice()
        {
            var state = await MyService.SetServicePrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置服务费成功！");
            else
                Console.WriteLine("设置服务费失败！");
        }

        private async void SetReportInterval()
        {
            var state = await MyService.SetServicePrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置上报间隔成功！");
            else
                Console.WriteLine("设置上报间隔失败！");
        }

        /// 开始充电
        private async void StartCharging()
        {
            var result = await MyService.SetCharging("1110000001001001", 1, 0, CPS.Communication.Service.DataPackets.ActionTypeEnum.Startup, 0);
            if (result)
                Console.WriteLine("启动充电成功！");
            else
                Console.WriteLine("启动充电失败！");
        }

        private async void StopCharging()
        {
            var result = await MyService.SetCharging("1110000001001001", 1, 0, CPS.Communication.Service.DataPackets.ActionTypeEnum.Shutdown, 0);
            if (result)
                Console.WriteLine("停止充电成功！");
            else
                Console.WriteLine("停止充电失败！");
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                MyService?.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion ====测试====
    }
}
