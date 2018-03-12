using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public class Constants
    {
        public const string StationContainerKey = "StationContainer";
        public const string ChargingPileContainerKey = "ChargingPileContainer";

        public const string SessionContainerKey = "SessionContainer";
        public const string SMSContainerKey = "SMSContainer";

        public const string ChargingRealtimeDataContainerKey = "ChargingRealtimeDataContainer";

        // 标示字典表中记录所属系统
        public const string CPServiceKey = "CPService";
        // 字典表中交易流水号Key值
        public const string TransactionSerialNumberKey = "TransactionSerialNumber";
    }
}
