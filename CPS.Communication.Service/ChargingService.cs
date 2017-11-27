using CPS.Communication.Service.DataPackets;
using CPS.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    public class ChargingService : IChargingPileService
    {
        CPS_Entities EntityContext = new CPS_Entities();

        private ChargingService()
        {
            ThreadPool.SetMaxThreads(100, 500);
        }

        private static ChargingService _instance;
        static ChargingService()
        {
            if (_instance == null)
                _instance = new ChargingService();
        }
        public static ChargingService Instance { get { return _instance; } }

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

        public bool LoginIn(object sender, EventArgs args)
        {
            string sn = "";
            string username = "";
            string pwd = "";
            var result = EntityContext.CPS_ChargingPile.Any(_ => _.SerialNumber == sn && _.Username == username && _.Pwd == pwd);

            return true;
        }
    }
}
