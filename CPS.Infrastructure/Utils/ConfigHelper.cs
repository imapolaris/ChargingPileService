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
            var configuration = GetConfiguration(assembly);

            var val = configuration.AppSettings.Settings[key];
            if (val == null)
                throw new KeyNotFoundException("没有找到key对应的值");

            return val.Value;
        }

        public static ConfigurationSection GetSection(Assembly assembly, string section)
        {
            var configuration = GetConfiguration(assembly);

            return configuration.GetSection(section);
        }

        static Configuration GetConfiguration(Assembly assembly)
        {
            var fileName = assembly.ManifestModule.FullyQualifiedName + ".config";
            if (!File.Exists(fileName))
                throw new ArgumentNullException("没有找到程序集配置文件");

            var configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = fileName;
            return ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }
    }
}
