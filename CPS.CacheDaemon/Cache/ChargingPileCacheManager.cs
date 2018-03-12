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
    using StackExchange.Redis;
    using Infrastructure.Utils;

    [Export(typeof(ICacheManager))]
    internal class ChargingPileCacheManager : CacheManagerBase
    {
        private static readonly string ChargingPileContainer = Constants.ChargingPileContainerKey;

        public ChargingPileCacheManager() : base()
        {
            _timer.Elapsed += _timer_Elapsed;
        }
        
        public override async Task Initializer()
        {
            Logger.Info("start loading charging piles data into the cache...");

            var result = await Task.Run(() =>
            {
                try
                {
                    var SysDbContext = new SystemDbContext();
                    var data = SysDbContext.ChargingPiles.Select(_ => new ChargingPileCache
                    {
                        Id = _.Id,
                        Name = _.DisName,
                        Category = _.Cpcategory,
                        StationId = _.StationId,
                        SubscribeStatus = (int)_.Status,
                        SN = _.SerialNumber,
                        SOC = _.SOC,
                    }).ToDictionary(_ => _.SN, _=>JsonHelper.Serialize(_));

                    var db = _redis.GetDatabase();
                    List<HashEntry> list = new List<HashEntry>();
                    foreach (var item in data)
                    {
                        HashEntry he = new HashEntry(item.Key, item.Value);
                        list.Add(he);
                    }
                    db.HashSet(ChargingPileContainer, list.ToArray());
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
                Logger.Info("succeed to load charing pile data into the cache.");
            }
            else
            {
                Logger.Info("fail to load charging pile data into the cache.");
            }

            // start inspect.
            Inspector();
        }

        public override void Inspector()
        {
            base.Inspector();

            _timer.Start();
        }


        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Info($"start inspect charging pile data at {DateTime.Now}");

            var db = _redis.GetDatabase();
            var fields = db.HashKeys(ChargingPileContainer).ToStringArray();

            if (fields == null || fields.Count() <= 0)
                return;

            var SysDbContext = new SystemDbContext();
            var data = SysDbContext.ChargingPiles.Where(_ => !fields.Contains(_.Id));
            if (data == null || data.Count() <= 0) return;
            var newd = data.Select(_ => new ChargingPileCache()
            {
                Id = _.Id,
                Name = _.DisName,
                Category = _.Cpcategory,
                StationId = _.StationId,
                SubscribeStatus = (int)_.Status,
                SN = _.SerialNumber,
                SOC = _.SOC,
            }).ToDictionary(_ => _.SN, _ => JsonHelper.Serialize(_));

            List<HashEntry> list = new List<HashEntry>();
            foreach (var item in newd)
            {
                list.Add(new HashEntry(item.Key, item.Value));
            }
            db.HashSet(ChargingPileContainer, list.ToArray());
            Logger.Info("inspect charging pile data completed...");
        }
    }
}
