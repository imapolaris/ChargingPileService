using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public class ConfigHelper
    {
        #region 【属性】

        public static string AliAccessKeyId
        {
            get
            {
                return ConfigurationManager.AppSettings["AliAccessKeyId"];
            }
        }

        public static string AliAccessKeySecret
        {
            get
            {
                return ConfigurationManager.AppSettings["AliAccessKeySecret"];
            }
        }

        public static string AliRegionId
        {
            get
            {
                return ConfigurationManager.AppSettings["AliRegionId"];
            }
        }

        public static string AliSignName
        {
            get
            {
                return ConfigurationManager.AppSettings["AliSignName"];
            }
        }

        public static string AliTemplateCode
        {
            get
            {
                return ConfigurationManager.AppSettings["AliTemplateCode"];
            }
        }

        public static string AliTemplateParam
        {
            get
            {
                return ConfigurationManager.AppSettings["AliTemplateParam"];
            }
        }

        public static int VCodeValidityDuration
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["VCodeValidityDuration"]);
            }
        }

        public static double NearbyDistance
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings["NearbyDistance"]);
            }
        }

        public static string ProductDesc
        {
            get
            {
                return ConfigurationManager.AppSettings["ProductDesc"];
            }
        }

        public static string ServerIP
        {
            get
            {
                return ConfigurationManager.AppSettings["serverIP"];
            }
        }

        public const int DefaultPort = 2222;
        public static int ServerPort
        {
            get
            {
                var port = ConfigurationManager.AppSettings["serverPort"];
                if (string.IsNullOrEmpty(port))
                    return DefaultPort;
                else
                {
                    int p = DefaultPort;
                    int.TryParse(port, out p);
                    return p;
                }

            }
        }

        public static string Message_From_Http_Channel
        {
            get
            {
                return ConfigurationManager.AppSettings["message_from_http_channel"];
            }
        }

        public static string Message_From_Tcp_Channel
        {
            get
            {
                return ConfigurationManager.AppSettings["message_from_tcp_channel"];
            }
        }

        public static string StationContainerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StationContainerKey"];
            }
        }

        public static string ChargingPileContainerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ChargingPileContainerKey"];
            }
        }

        #endregion 【属性】

        #region 【接口】

        public static string GetValue(Assembly assembly, string key)
        {
            var configuration = GetConfiguration(GetConfigFileName(assembly));

            var val = configuration.AppSettings.Settings[key];
            if (val == null)
                throw new KeyNotFoundException("没有找到key对应的值");

            return val.Value;
        }

        public static ConfigurationSection GetSection(Assembly assembly, string section)
        {
            var configuration = GetConfiguration(GetConfigFileName(assembly));
            return configuration.GetSection(section);
        }

        public static ConfigurationSection GetSection(string configFile, string section)
        {
            var configuration = GetConfiguration(configFile);
            return configuration.GetSection(section);
        }

        static string GetConfigFileName(Assembly assembly)
        {
            var fileName = assembly.ManifestModule.FullyQualifiedName + ".config";
            if (!File.Exists(fileName))
                throw new ArgumentNullException("没有找到配置文件...");

            return fileName;
        }

        static Configuration GetConfiguration(string configFileName)
        {
            var configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            return ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }

        #endregion 【接口】
    }
}
