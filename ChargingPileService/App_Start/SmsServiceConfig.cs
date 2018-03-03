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
        private Timer _timer;
        private bool registered = false;

        private const string SMSContainerKey = "SMSContainer";
        private readonly RedisClient _client = null;
        private ManualResetEvent _manualEvent = new ManualResetEvent(true);

        public void Register()
        {
            if (!registered)
            {
                registered = !registered;

                //_timer = new Timer(RunClear, null, VCodeValidityDuration * 1000, 500);
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
            _client = RedisManager.GetClient();
        }
        private static SmsServiceConfig _instance;

        #endregion 【singleton】

        public bool ValidateVCode(string phoneNumber, string vcode)
        {
            var json = _client.HGet(SMSContainerKey, phoneNumber);
            if (string.IsNullOrEmpty(json))
                return false;
            var entity = JsonHelper.Deserialize<SmsEntity>(json);
            if (entity != null && entity.VCode == vcode)
                return true;
            return false;
        }

        public void AppendVCode(string phoneNumber, string vcode)
        {
            _client.HSet(SMSContainerKey,
                phoneNumber,
                new SmsEntity()
                {
                    PhoneNumber = phoneNumber,
                    VCode = vcode,
                    RegisterDate = DateTime.Now,
                });
        }

        public void RunClear(object state)
        {
            _manualEvent.Reset();

            var now = DateTime.Now;
            var fs = _client.HKeys(SMSContainerKey);
            if (fs == null || fs.Length <= 0) return;
            var collection = _client.HMGet(SMSContainerKey, fs);
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
                    _client.HDel(SMSContainerKey, fields.ToArray());
                }
            }

            _manualEvent.Set();
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