using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CPS.Entities
{
    /// <summary>
    /// 电站
    /// </summary>
    public class Station : EntityBase
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Numbers { get; set; }
        public string Address { get; set; }
    }
}