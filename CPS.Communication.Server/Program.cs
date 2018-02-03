using CPS.Communication.Service;
using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Server
{
    class Program
    {
        static ChargingService MyService = ChargingService.Instance;

        static void Main(string[] args)
        {
            Init();
            PrintStartInfo();


            lock ("123")
            {
                Console.WriteLine(DateTime.Now);
                Console.WriteLine("thread-server");
            }

            while (true)
            {
                string input = Console.ReadLine();
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

            PrintStopInfo();
        }


        static void Init()
        {
            Console.Title = "充电桩通信服务控制台";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.BufferWidth = Console.LargestWindowWidth / 2;
        }

        static void PrintStartInfo()
        {
            Console.WriteLine("-----------------------开始充电桩通信服务-----------------------");
            Console.WriteLine($"启动时间：{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}");
            Console.WriteLine($"版本：{"Ver 1.0"}");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("服务正在运行 ......(输入exit停止服务)\n");
        }

        static void PrintStopInfo()
        {
            Console.WriteLine("-----------------------结束充电桩通信服务-----------------------");
        }

        private static void Server_ServerStopped(object sender, Service.Events.ServerStoppedEventArgs args)
        {
            Console.WriteLine("Server Stopped!");
        }

        private static void Server_ServerStarted(object sender, Service.Events.ServerStartedEventArgs args)
        {
            Console.WriteLine("Server Started!");
        }

        private static void Server_ClientAccepted(object sender, Service.Events.ClientAcceptedEventArgs args)
        {
            Console.WriteLine($"----客户端 {args.CurClient.ID} 已连接！");
        }

        private static void Server_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            Console.WriteLine("Error:" + args.ErrorMessage);
        }

        /// <summary>
        /// 启动redis
        /// </summary>
        /// <returns></returns>
        private static bool LaunchRedis()
        {
            return true;
        }

        #region ====测试====
        private async static void Reboot()
        {
            var state = await MyService.Reboot("1234567890AbcBCa");
            if (state)
                Console.WriteLine("充电桩重启成功！");
            else
                Console.WriteLine("充电桩重启失败！");
        }

        private async static void SetElecPrice()
        {
            var state = await MyService.SetElecPrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置电价成功！");
            else
                Console.WriteLine("设置电价失败！");
        }

        private async static void SetServicePrice()
        {
            var state = await MyService.SetServicePrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置服务费成功！");
            else
                Console.WriteLine("设置服务费失败！");
        }

        private async static void SetReportInterval()
        {
            var state = await MyService.SetServicePrice("1234567890AbcBCa", 10000, 8000, 6000, 4000);
            if (state)
                Console.WriteLine("设置上报间隔成功！");
            else
                Console.WriteLine("设置上报间隔失败！");
        }

        /// 开始充电
        private async static void StartCharging()
        {
            var result = await MyService.SetCharging("1110000001001001", 1, 0, CPS.Communication.Service.DataPackets.ActionTypeEnum.Startup, 0);
            if (result)
                Console.WriteLine("启动充电成功！");
            else
                Console.WriteLine("启动充电失败！");
        }

        private async static void StopCharging()
        {
            var result = await MyService.SetCharging("1110000001001001", 1, 0, CPS.Communication.Service.DataPackets.ActionTypeEnum.Shutdown, 0);
            if (result)
                Console.WriteLine("停止充电成功！");
            else
                Console.WriteLine("停止充电失败！");
        }
        #endregion ====测试====
    }
}
