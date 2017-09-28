using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CPS.Infrastructure.Utils;

namespace ChargingPileService.Tests
{
    [TestClass]
    public class MapTest
    {
        private const double myLng = 116.250022;
        private const double myLat = 40.08803;

        [TestMethod]
        public void CalcDistance()
        {
            double lng1 = 116.40739630000007;
            double lat1 = 39.90419989999999;
            double lng2 = 117.12009499999999;
            double lat2 = 36.6512;
            var distance1 = MapHelper.GetDistance(lng1, lat1, lng2, lat2);
            var distance2 = MapHelper.GetDistance(lng2, lat2, lng1, lat1);

            Assert.AreEqual(distance1, distance2, 0);
        }
    }
}
