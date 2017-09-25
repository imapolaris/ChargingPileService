using CPS.Infrastructure.Utils;
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
        private List<SmsEntity> SmsContainer;
        private Timer _timer;
        private bool registered = false;
        private ManualResetEvent _resetEvent = new ManualResetEvent(true);

        public void Register()
        {
            if (!registered)
            {
                registered = !registered;

                SmsContainer = new List<SmsEntity>();
                _timer = new Timer(RunClear, null, VCodeValidityDuration * 1000, VCodeValidityDuration*1000);
            }
        }

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
        private SmsServiceConfig() { }
        private static SmsServiceConfig _instance;

        public bool ValidateVCode(string phoneNumber, string vcode)
        {
            var exists = SmsContainer.Any(_ => _.PhoneNumber == phoneNumber && _.VCode == vcode && !_.Expired);
            if (exists)
            {
                _resetEvent.Reset();

                SmsContainer.ForEach(_ =>
                {
                    if (_.PhoneNumber == phoneNumber)
                    {
                        _.Expired = true;
                    }
                });

                _resetEvent.Set();
            }

            Logger.Instance.Info($"container length: {SmsContainer.Count()}, \nvalidate vcode: {phoneNumber}:{vcode}");

            return exists;
        }

        public void AppendVCode(string phoneNumber, string vcode)
        {
            _resetEvent.Reset();

            SmsContainer.Add(new SmsEntity()
            {
                PhoneNumber = phoneNumber,
                VCode = vcode,
                RegisterDate = DateTime.Now,
            });

            _resetEvent.Set();

            Logger.Instance.Info($"add vcode: {phoneNumber}:{vcode}");
        }

        private void RunClear(object state)
        {
            _resetEvent.Reset();

            var now = DateTime.Now;
            SmsContainer.RemoveAll(_=>
            {
                return _.Expired
                    || (now - _.RegisterDate).TotalSeconds > VCodeValidityDuration;
            });

            _resetEvent.Set();
        }
    }

    public class SmsEntity
    {
        public string PhoneNumber { get; set; }
        public string VCode { get; set; }
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// 是否过期，用来清除验证码列表的标识
        /// </summary>
        public bool Expired { get; set; } = false;
    }
}