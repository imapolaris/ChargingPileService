using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Entities
{
    public class ChargingRecord
    {
        public string Id { get; set; }
        /// <summary>
        /// 充电度数
        /// </summary>
        public double Kwhs { get; set; }
        /// <summary>
        /// 充电日期
        /// </summary>
        public string ChargingDate { get; set; }
        /// <summary>
        /// 花费
        /// </summary>
        public double Cost { get; set; }
    }
}