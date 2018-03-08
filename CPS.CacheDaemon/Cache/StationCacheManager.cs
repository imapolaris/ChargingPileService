using CPS.Infrastructure.Cache;
using CPS.Infrastructure.Utils;
using Soaring.WebMonter.Contract.Cache;
using Soaring.WebMonter.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon.Cache
{
    using CSRedis;
    using Infrastructure.Redis;
    using System.Timers;

    [Export(typeof(ICacheManager))]
    internal class StationCacheManager : CacheManagerBase
    {
        private static readonly string StationContainer = ConfigHelper.StationContainerKey;
        
        public StationCacheManager() : base()
        {
            _timer.Elapsed += _timer_Elapsed;
        }

        public override async Task Initializer()
        {
            Logger.Info("start loading station data into the cache...");

            var result = await Task.Run(() =>
            {
                try
                {
                    SystemDbContext SysDbContext = new SystemDbContext();
                    var data = SysDbContext.Stations.Select(_ => new StationCache
                    {
                        Id = _.Id,
                        Name = _.Name,
                        Address = _.Address,
                        Longitude = _.Lon,
                        Latitude = _.Lat,
                        ElecPrice = 0,
                        DetailId = _.DetailId,
                        Distance = 0,
                        ACAll = SysDbContext.ChargingPiles.Count(cp => cp.StationId == _.Id && cp.Cpcategory == 0),
                        ACIdle = 0,
                        DCAll = SysDbContext.ChargingPiles.Count(cp => cp.StationId == _.Id && cp.Cpcategory == 1),
                        DCIdle = 0,
                    }).ToDictionary(_ => _.Id, _ => JsonHelper.Serialize(_));

                    using (var client = RedisManager.GetClient())
                    {
                        client.HMSet(StationContainer, data);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return false;
                }

                return true;
            });

            if (result)
            {
                Logger.Info("succeed to load station data.");
            }
            else
            {
                Logger.Info("fail to load station data into cache.");
            }

            // start to insepect.
            Inspector();
        }

        public override void Inspector()
        {
            base.Inspector();

            _timer?.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Info($"start inspect station data at {DateTime.Now}");
            using (var client = RedisManager.GetClient())
            {
                var fields = client.HKeys(StationContainer);

                if (fields == null || fields.Count() <= 0)
                    return;

                var SysDbContext = new SystemDbContext();
                var data = SysDbContext.Stations.Where(_ => !fields.Contains(_.Id));
                if (data == null || data.Count() <= 0) return;
                var newd = data.Select(_ => new StationCache()
                {
                    Id = _.Id,
                    Name = _.Name,
                    Address = _.Address,
                    Longitude = _.Lon,
                    Latitude = _.Lat,
                    ElecPrice = 0,
                    DetailId = _.DetailId,
                    Distance = 0,
                    ACAll = SysDbContext.ChargingPiles.Count(cp => cp.StationId == _.Id && cp.Cpcategory == 0),
                    ACIdle = 0,
                    DCAll = SysDbContext.ChargingPiles.Count(cp => cp.StationId == _.Id && cp.Cpcategory == 1),
                    DCIdle = 0,
                }).ToDictionary(_ => _.Id, _ => JsonHelper.Serialize(_));

                client.HMSet(StationContainer, newd);
            }
            Logger.Info("inspect station data completed...");
        }
    }
}
