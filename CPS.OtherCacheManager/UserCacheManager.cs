using CPS.Infrastructure.Cache;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.OtherCacheManager
{
    [Export(typeof(ICacheManager))]
    public class UserCacheManager : ICacheManager
    {
        public async Task LoadCache()
        {
            //Logger.Info("start loading user data into the cache...");
            await Task.Run(() =>
            {

            });
        }
    }
}
