using CPS.Infrastructure.Redis;
using CPS.Infrastructure.Utils;
using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ChargingPileService
{
    public class SmsServiceConfig
    {
        private static readonly int VCodeValidityDuration = ConfigHelper.VCodeValidityDuration;
        private bool registered = false;

        private const string SMSContainerKey = "SMSContainer";
        private ManualResetEvent _manualEvent = new ManualResetEvent(true);

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
        }
        private static SmsServiceConfig _instance;

        #endregion 【singleton】

        public bool ValidateVCode(string phoneNumber, string vcode)
        {
            using (var client = RedisManager.GetClient())
            {
                var json = client.HGet(SMSContainerKey, phoneNumber);
                if (string.IsNullOrEmpty(json))
                    return false;
                var entity = JsonHelper.Deserialize<SmsEntity>(json);
                if (entity != null && entity.VCode == vcode)
                    return true;
                return false;
            }
        }

        public void AppendVCode(string phoneNumber, string vcode)
        {
            using (var client = RedisManager.GetClient())
            {
                client.HSet(SMSContainerKey,
                phoneNumber,
                new SmsEntity()
                {
                    PhoneNumber = phoneNumber,
                    VCode = vcode,
                    RegisterDate = DateTime.Now,
                });
            }
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

                        using (var client = RedisManager.GetClient())
                        {
                            var now = DateTime.Now;
                            string[] fs = client.HKeys(SMSContainerKey);
                            if (fs == null || fs.Length <= 0) continue;
                            var collection = client.HMGet(SMSContainerKey, fs);
                            if (collection != null && collection.Length > 0)
                            {
                                List<string> fields = new List<string>();
                                foreach (var item in collection)
                                {
                                    var entity = JsonHelper.Deserialize<SmsEntity>(item);
                                    if ((now - entity.RegisterDate).TotalSeconds >= VCodeValidityDuration)
                                    {
                                        fields.Add(entity.PhoneNumber);
                                    }
                                }
                                if (fields.Count > 0)
                                {
                                    client.HDelAsync(SMSContainerKey, fields.ToArray());
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