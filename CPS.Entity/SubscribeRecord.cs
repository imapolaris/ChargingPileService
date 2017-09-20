using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CPS.Entities
{
    public class SubscribeRecord : EntityBase
    {
        public string SerialNumber { get; set; }
        public string SubscribeDate { get; set; }
        public string SubscribeStatus { get; set; }
    }
}