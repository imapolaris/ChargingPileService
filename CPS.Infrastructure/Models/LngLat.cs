using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Models
{
    public class LngLat
    {
        private static readonly double NearbyDistance = ConfigHelper.NearbyDistance;

        private double longitude;
        private double latitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public bool IsNearby(LngLat lnglat)
        {
            return Between2Coords(lnglat) <= NearbyDistance;
        }

        public double Between2Coords(LngLat lnglat)
        {
            var distance = MapHelper.GetDistance(this.Longitude, this.Latitude, lnglat.Longitude, lnglat.Latitude);
            return distance / 1000; // 单位千米
        }
    }
}
