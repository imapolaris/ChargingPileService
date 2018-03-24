using CPS.Communication.Service;
using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CPS.Communication.Service
{
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            // 捕获全局异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            HostFactory.Run(x =>
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

    public class CommunicationServer : IDisposable
    {
        public CommunicationServer()
        {}

        public void Start()
        {
            InitEnv();
            PrintStartInfo();

            StartupServer();
        }

        public void Stop()
        {
            PrintStopInfo();
            Dispose();
        }

        #region 【启动TCP服务】

        internal Server MyServer { get; set; }
        private static readonly string ServerIP = ConfigHelper.ServerIP;
        private static readonly int ServerPort = ConfigHelper.ServerPort;

        private void StartupServer()
        {
            MyServer = new Server();
            MyServer.ErrorOccurred += Server_ErrorOccurred;
            MyServer.ClientAccepted += Server_ClientAccepted;
            MyServer.ServerStarted += Server_ServerStarted;
            MyServer.ServerStopped += Server_ServerStopped;
            MyServer.Listen(ServerPort);
        }

        private static void Server_ServerStopped(object sender, Service.Events.ServerStoppedEventArgs args)
        {
            Logger.Info("Server Stopped!");
        }

        private static void Server_ServerStarted(object sender, Service.Events.ServerStartedEventArgs args)
        {
            Logger.Info("Server Started!");
        }

        private static void Server_ClientAccepted(object sender, Service.Events.ClientAcceptedEventArgs args)
        {
            Logger.Info($"----客户端 {args.CurClient.ID} 已连接！");
        }

        private static void Server_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            Logger.Info("Error:" + args.ErrorMessage);
        }

        #endregion 【启动TCP服务】

        #region 【初始化控制台环境】

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
            Logger.Info("\n-----------------------结束充电桩通信服务-----------------------");
        }

        #endregion

        #region 【IDisposable Support】

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                MyServer?.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion 【IDisposable Support】
    }
}
