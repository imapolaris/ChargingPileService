using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.DaemonService
{
    using CPS.Infrastructure.Utils;

    internal class DaemonConfiguration
    {
        public static IList<AppInstance> GetConfig()
        {
            IList<AppInstance> appInsList = GetConfig("appInsGroup");
            return appInsList;
        }

        public static IList<AppInstance> GetConfig(string sectionGroupName)
        {
            var collection = ConfigHelper.GetSectionCollection(@"daemon.config", sectionGroupName);
            if (collection == null)
                return null;

            IList<AppInstance> appInsList = new List<AppInstance>();
            for (int i = 0; i < collection.Count; i++)
            {
                var appIns = (AppInstance)collection[i];
                appInsList.Add(appIns);
            }

            return appInsList;
        }
    }
}
