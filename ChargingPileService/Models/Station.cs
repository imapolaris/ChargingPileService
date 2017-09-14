using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Models
{
    public class Station
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Numbers { get; set; }
        public string Address { get; set; }
    }
}