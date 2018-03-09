using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Controllers
{
    using CPS.Infrastructure.Redis;
    using CPS.Infrastructure.Utils;
    using StackExchange.Redis;

    public class MqOperatorBase : OperatorBase
    {
        private static readonly string Call_Channel = ConfigHelper.Message_From_Http_Channel;
        protected SessionServiceConfig SessionService = SessionServiceConfig.Instance;
        private ConnectionMultiplexer _redis = null;

        public MqOperatorBase()
        {
            _redis = RedisManager.GetClient();
        }

        public virtual void Call(string msg)
        {
            var sub = _redis.GetSubscriber();
            sub.Publish(Call_Channel, msg);
        }

        public virtual void CallAsync(string msg)
        {
            var sub = _redis.GetSubscriber();
            sub.PublishAsync(Call_Channel, msg);
        }
    }
}