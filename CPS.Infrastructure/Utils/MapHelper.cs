using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Utils
{
    public class MapHelper
    {
        private static readonly double EARTHRADIUS = 6370996.81;

        public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            try
            {
                if (!IsValidCoord(lng1, lat1) || !IsValidCoord(lng2, lat2))
                    throw new ArgumentException($"非法坐标:{lng1},{lat1};{lng2},{lat2}");

                lng1 = GetLoop(lng1, -180, 180);
                lat1 = GetRange(lat1, -74, 74);
                lng2 = GetLoop(lng2, -180, 180);
                lat2 = GetRange(lat2, -74, 74);

                double x1, y1, x2, y2;
                x1 = DegreeToRad(lng1);
                y1 = DegreeToRad(lat1);
                x2 = DegreeToRad(lng2);
                y2 = DegreeToRad(lat2);

                return EARTHRADIUS * Math.Acos((Math.Sin(y1) * Math.Sin(y2) + Math.Cos(y1) * Math.Cos(y2) * Math.Cos(x2 - x1)));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                return double.MaxValue;
            }
        }

        private static double GetLoop(double v, double a, double b)
        {
            while (v > b)
            {
                v -= b - a;
            }
            while (v < a)
            {
                v += b - a;
            }
            return v;
        }

        private static double GetRange(double v, double a, double b)
        {
            v = Math.Max(v, a);
            v = Math.Min(v, b);

            return v;
        }

        private static double DegreeToRad(double degree)
        {
            return Math.PI * degree / 180;
        }

        public static bool IsValidCoord(double lng, double lat)
        {
            if (lng < -180 || lng > 180 || lat < -90 || lat > 90 )
                return false;
            return true;
        }
    }
}
