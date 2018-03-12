using CPS.Infrastructure.Redis;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ChargingPileService
{
    using StackExchange.Redis;

    public class SmsServiceConfig
    {
        private static readonly int VCodeValidityDuration = ConfigHelper.VCodeValidityDuration;
        private bool registered = false;
        
        private ManualResetEvent _manualEvent = new ManualResetEvent(true);
        private ConnectionMultiplexer _redis = null;

        public void Register()
        {
            if (!registered)
            {
                RunClear();

                registered = !registered;
            }
        }

        #region 【singleton】

        public static SmsServiceConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SmsServiceConfig();
                }
                return _instance;
            }
        }
        private SmsServiceConfig()
        {
            _redis = RedisManager.GetClient();
        }
        private static SmsServiceConfig _instance;

        #endregion 【singleton】

        public bool ValidateVCode(string phoneNumber, string vcode)
        {
            var db = _redis.GetDatabase();
            var json = db.HashGet(Constants.SMSContainerKey, phoneNumber);
            if (string.IsNullOrEmpty(json))
                return false;
            var entity = JsonHelper.Deserialize<SmsEntity>(json);
            if (entity != null && entity.VCode == vcode)
                return true;
            return false;
        }

        public void AppendVCode(string phoneNumber, string vcode)
        {
            var db = _redis.GetDatabase();
            db.HashSet(Constants.SMSContainerKey,
                phoneNumber,
                JsonHelper.Serialize(
                    new SmsEntity()
                    {
                        PhoneNumber = phoneNumber,
                        VCode = vcode,
                        RegisterDate = DateTime.Now,
                    }
             ));
        }

        private Thread ThreadRunClear;
        private bool stopRunClear = false;
        public void RunClear()
        {
            ThreadRunClear = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (ThreadRunClear == null
                            || !ThreadRunClear.IsAlive
                            || stopRunClear)
                            break;

                        var db = _redis.GetDatabase();
                        var now = DateTime.Now;
                        var collection = db.HashValues(Constants.SMSContainerKey);
                        if (collection != null && collection.Length > 0)
                        {
                            foreach (var item in collection)
                            {
                                var entity = JsonHelper.Deserialize<SmsEntity>(item);
                                if ((now - entity.RegisterDate).TotalSeconds >= VCodeValidityDuration)
                                {
                                    db.HashDelete(Constants.SMSContainerKey, entity.PhoneNumber);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    Thread.Sleep(500);
                }
            })
            { IsBackground = true };
            ThreadRunClear.Start();
        }
    }

    public class SmsEntity
    {
        public string PhoneNumber { get; set; }
        public string VCode { get; set; }
        public DateTime RegisterDate { get; set; }

        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }
    }
}