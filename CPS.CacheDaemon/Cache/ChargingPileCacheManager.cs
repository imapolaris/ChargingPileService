using CPS.Infrastructure.Cache;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon.Cache
{
    [Export(typeof(ICacheManager))]
    internal class ChargingPileCacheManager : ICacheManager
    {
        public Task LoadCache()
        {
            Logger.Info("start loading charging piles data into the cache...");

            return null;
        }
    }
}
