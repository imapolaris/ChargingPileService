using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Entities
{
    public class SubscribeRecord
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public string SubscribeDate { get; set; }
        public string SubscribeStatus { get; set; }
    }
}