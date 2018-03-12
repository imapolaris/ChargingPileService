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
    using Infrastructure.Redis;
    using System.Timers;
    using StackExchange.Redis;
    using Infrastructure.Utils;

    [Export(typeof(ICacheManager))]
    internal class StationCacheManager : CacheManagerBase
    {
        private static readonly string StationContainer = Constants.StationContainerKey;
        
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

                    var db = _redis.GetDatabase();
                    List<HashEntry> list = new List<HashEntry>();
                    foreach (var item in data)
                    {
                        list.Add(new HashEntry(item.Key, item.Value));
                    }
                    db.HashSet(StationContainer, list.ToArray());
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

            var db = _redis.GetDatabase();
            var fields = db.HashKeys(StationContainer).ToStringArray();

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

            List<HashEntry> list = new List<HashEntry>();
            foreach (var item in newd)
            {
                list.Add(new HashEntry(item.Key, item.Value));
            }
            db.HashSet(StationContainer, list.ToArray());
            Logger.Info("inspect station data completed...");
        }
    }
}
