using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.CacheDaemon.Cache
{
    using Infrastructure.Cache;
    using Infrastructure.Utils;
    using System.Timers;

    internal class CacheManagerBase : ICacheManager
    {
        protected Timer _timer = null;
        protected int DefaultInspectInterval = 10 * 60 * 1000; // 10分钟

        public CacheManagerBase()
        {
            _timer = new Timer();
            _timer.Interval = DefaultInspectInterval;
        }

        public virtual async Task Initializer()
        {
            await Task.Run(() => { });
        }

        public virtual void Inspector()
        {
            Logger.Info(this.GetType().ToString() + " start inspect...");
        }
    }
}
