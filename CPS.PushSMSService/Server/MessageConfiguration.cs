using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.MessageSystem
{
    using CPS.Infrastructure.Utils;

    internal class MessageConfiguration : ConfigurationSection
    {
        public static MessageConfiguration GetConfig()
        {
            return GetConfig("./message.config", "MessageConfig");
        }

        public static MessageConfiguration GetConfig(string configFile, string sectionName)
        {
            MessageConfiguration section = (MessageConfiguration)ConfigHelper.GetSection(configFile, sectionName);
            if (section == null)
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            return section;
        }

        [ConfigurationProperty(nameof(AliAccessKeyId), IsRequired =true)]
        public string AliAccessKeyId
        {
            get
            {
                return (string)base["AliAccessKeyId"];
            }
        }

        [ConfigurationProperty(nameof(AliAccessKeySecret), IsRequired =true)]
        public string AliAccessKeySecret
        {
            get
            {
                return (string)base["AliAccessKeySecret"];
            }
        }

        [ConfigurationProperty(nameof(AliRegionId), DefaultValue ="cn-beijing")]
        public string AliRegionId
        {
            get
            {
                return (string)base["AliRegionId"];
            }
        }

        [ConfigurationProperty(nameof(AliSignName), DefaultValue ="充电桩服务")]
        public string AliSignName
        {
            get
            {
                return (string)base["AliSignName"];
            }
        }

        [ConfigurationProperty(nameof(AliTemplateCode), DefaultValue = "SMS_94190001", IsRequired =true)]
        public string AliTemplateCode
        {
            get
            {
                return (string)base["AliTemplateCode"];
            }
        }

        [ConfigurationProperty(nameof(AliTemplateParam), DefaultValue ="code", IsRequired =true)]
        public string AliTemplateParam
        {
            get
            {
                return (string)base["AliTemplateParam"];
            }
        }

        [ConfigurationProperty(nameof(JGPushAppKey), IsRequired =true)]
        public string JGPushAppKey
        {
            get
            {
                return (string)base["JGPushAppKey"];
            }
        }

        [ConfigurationProperty(nameof(JGPushMasterSecret), IsRequired =true)]
        public string JGPushMasterSecret
        {
            get
            {
                return (string)base["JGPushMasterSecret"];
            }
        }
    }
}
