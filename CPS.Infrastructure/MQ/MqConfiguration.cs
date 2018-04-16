using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.MQ
{
    using System.Configuration;
    using CPS.Infrastructure.Utils;

    public class MqConfiguration : ConfigurationSection
    {
        public static MqConfiguration GetConfig()
        {
            return GetConfig("./mq.config", "MQConfig");
        }

        public static MqConfiguration GetConfig(string configFile, string sectionName)
        {
            var section = (MqConfiguration)ConfigHelper.GetSection(configFile, sectionName);
            if (section == null)
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            return section;
        }

        [ConfigurationProperty(nameof(HostName), DefaultValue="localhost", IsRequired =true)]
        public string HostName
        {
            get
            {
                return (string)base["HostName"];
            }
        }

        [ConfigurationProperty(nameof(VirtualHost), DefaultValue ="local", IsRequired =false)]
        public string VirtualHost
        {
            get
            {
                return (string)base["VirtualHost"];
            }
        }
    }
}
