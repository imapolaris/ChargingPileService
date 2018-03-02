﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    using System.IO;
    using log4net;
    using log4net.Config;

    public class Logger
    {
        private static readonly ILog _log;

        private static readonly Logger _instance = new Logger();
        public static Logger Instance { get { return _instance; } }

        static Logger()
        {
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
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
