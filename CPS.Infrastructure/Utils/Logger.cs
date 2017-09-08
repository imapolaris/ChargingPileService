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
        private readonly ILog _log;

        private static readonly Logger _instance = new Logger();
        public static Logger Instance { get { return _instance; } }

        
        private Logger()
        {
            InitLogger();

            _log = LogManager.GetLogger(typeof(Logger));
        }

        private void InitLogger()
        {
            var path = System.Environment.CurrentDirectory;
            if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                path += System.IO.Path.DirectorySeparatorChar;
            }
            path += "log4net.config";
            var logCfg = new System.IO.FileInfo(path);
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }
    }
}
