using CPS.CacheDaemon.Cache;
using CPS.Infrastructure.Cache;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon
{
    internal class RedisCacheDaemon
    {
        [ImportMany]
        private List<ICacheManager> _managers;

        public RedisCacheDaemon()
        {
            var aggCatalog = new AggregateCatalog(
                    //new AssemblyCatalog(this.GetType().Assembly),
                    new DirectoryCatalog("./", "*Cache*")
                );
            var container = new CompositionContainer(aggCatalog);
            container.ComposeParts(this);
        }

        public void Start()
        {
            Logger.Info("cache daemon service start...");

            if (this._managers == null) return;
            this._managers.AsParallel().ForAll(_ => _.Initializer());
        }

        public void Stop()
        {
            Logger.Info("cache daemon service stop...");
        }
    }
}
