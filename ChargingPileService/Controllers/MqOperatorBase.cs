using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Controllers
{
    using CSRedis;
    using CPS.Infrastructure.Redis;
    using CPS.Infrastructure.Utils;

    public class MqOperatorBase : OperatorBase
    {
        private static readonly string Call_Channel = ConfigHelper.Message_From_Http_Channel;
        protected SessionServiceConfig SessionService = SessionServiceConfig.Instance;

        public MqOperatorBase()
        {
            
        }

        public virtual void Call(string msg)
        {
            using (var client = RedisManager.GetClient())
            {
                client.Publish(Call_Channel, msg);
            }
        }

        public virtual void CallAsync(string msg)
        {
            using (var client = RedisManager.GetClient())
            {
                client.PublishAsync(Call_Channel, msg);
            }
        }
    }
}