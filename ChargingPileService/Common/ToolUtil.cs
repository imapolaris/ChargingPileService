using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Common
{
    using CPS.Infrastructure.Utils;
    using Soaring.WebMonter.Contract.Manager;
    using Soaring.WebMonter.DB;

    public class ToolUtil
    {
        private static SystemDbContext SysDbContext = new SystemDbContext();

        private static readonly object locker = new object();
        public static long CreateTransactionSerialNumber()
        {
            lock (locker)
            {
                long transSn = 0;
                long initSn = 10000001;
                var configs = SysDbContext.Sys_SettingConfigs.Where(_ => _.ItemName == Constants.TransactionSerialNumberKey).FirstOrDefault();
                if (configs == null)
                {
                    SysDbContext.Sys_SettingConfigs.Add(new Sys_SettingConfig()
                    {
                        SettingType = Constants.CPServiceKey,
                        ItemName = Constants.TransactionSerialNumberKey,
                        ItemValue = initSn.ToString(),
                    });
                    transSn = initSn;
                }
                else
                {
                    long sn = long.Parse(configs.ItemValue);
                    sn += 1;
                    transSn = sn;
                    configs.ItemValue = sn.ToString();
                }

                int result = SysDbContext.SaveChanges();
                if (result > 0)
                    return transSn;
                else
                    throw new ArgumentException();
            }
        }
    }
}