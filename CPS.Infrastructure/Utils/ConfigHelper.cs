using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    }
}
