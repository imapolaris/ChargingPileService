using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    public class ChargingService : IChargingService
    {
        public object getChargingStatus(string sn)
        {
            var data = new JObject();
            data.Add("status", 1);
            data.Add("electric", 30);
            return data;
        }

        public bool startCharging(string sn)
        {
            return true;
        }

        public bool stopCharging(string sn)
        {
            return true;
        }
    }
}
