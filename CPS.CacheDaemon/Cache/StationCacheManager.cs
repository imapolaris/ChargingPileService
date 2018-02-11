using CPS.Infrastructure.Cache;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon.Cache
{
    [Export(typeof(ICacheManager))]
    internal class StationCacheManager : ICacheManager
    {
        SystemDbContext systemDbContext = new SystemDbContext();

        public async Task LoadCache()
        {
            Logger.Info("start loading station data into the cache...");
            var result = await Task.Run(() =>
            {


                return true;
            });

            if (result)
                Logger.Info("loading station data succeed.");
            else
                Logger.Info("loading station data failed.");
        }
    }
}
