using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;

namespace CPS.Infrastructure.Utils
{
    public class Logger
    {
        private static readonly ILog _log;

        private static readonly Logger _instance = new Logger();
        public static Logger Instance { get { return _instance; } }

        static Logger()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("./log4net.config"));
            _log = LogManager.GetLogger(typeof(Logger));
        }

        public static void Debug(object message)
        {
            _log.Debug(message);
        }

        public static void Info(object message)
        {
            _log.Info(message);
        }

        public static void Warn(object message)
        {
            _log.Warn(message);
        }

        public static void Error(object message)
        {
            _log.Error(message);
        }

        public static void Fatal(object message)
        {
            _log.Fatal(message);
        }
    }
}
