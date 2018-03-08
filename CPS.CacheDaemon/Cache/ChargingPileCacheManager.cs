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

    [Export(typeof(ICacheManager))]
    internal class ChargingPileCacheManager : CacheManagerBase
    {
        private static readonly string ChargingPileContainer = ConfigHelper.ChargingPileContainerKey;

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

                    using (var client = RedisManager.GetClient())
                    {
                        client.HMSet(ChargingPileContainer, data);
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
            using (var client = RedisManager.GetClient())
            {
                var fields = client.HKeys(ChargingPileContainer);

                if (fields == null || fields.Count() <= 0)
                    return;

                var SysDbContext = new SystemDbContext();
                var data = SysDbContext.ChargingPiles.Where(_ => !fields.Contains(_.Id));
                if (data == null || data.Count() <= 0) return;
                var newd = data.Select(_=> new ChargingPileCache()
                {
                    Id = _.Id,
                    Name = _.DisName,
                    Category = _.Cpcategory,
                    StationId = _.StationId,
                    SubscribeStatus = (int)_.Status,
                    SN = _.SerialNumber,
                    SOC = _.SOC,
                }).ToDictionary(_ => _.SN, _ => JsonHelper.Serialize(_));

                client.HMSet(ChargingPileContainer, newd);
            }
            Logger.Info("inspect charging pile data completed...");
        }
    }
}
