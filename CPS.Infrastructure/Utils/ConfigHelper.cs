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
            configFile.ExeConfigFilename = configFileName;
            return ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }
    }
}
