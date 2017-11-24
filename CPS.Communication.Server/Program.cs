using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();

            Service.Server server = new Service.Server();
            server.Listen(2222);
            server.ErrorOccurred += Server_ErrorOccurred;
            server.ClientAccepted += Server_ClientAccepted;
            server.ServerStarted += Server_ServerStarted;
            server.ServerStopped += Server_ServerStopped;

            PrintStartInfo();

            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower().Equals("exit"))
                    break;
            }

            PrintStopInfo();
        }

        static void Init()
        {
            Console.Title = "充电桩通信服务控制台";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
        }

        static void PrintStartInfo()
        {
            Console.WriteLine("-----------------------开始充电桩通信服务-----------------------");
            Console.WriteLine($"启动时间：{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}");
            Console.WriteLine($"版本：{"Ver 1.0"}");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("服务启动成功 ......(输入exit停止服务)\n");
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
            Console.WriteLine("Client Connected！");
        }

        private static void Server_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            Console.WriteLine("Error:" + args.ErrorMessage);
        }
    }
}
