using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.Common
{    
    public static class CalculateCoordinatesDistance
    {
        #region Calculate Distance From Latitude & Longitude
        private const double EarthRadius = 6371.0;

        public static double CalculateDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            var Latitude = ToRadians((double)(latitude2 - latitude1));
            var Longitude = ToRadians((double)(longitude2 - longitude1));
            var haversine = Math.Sin(Latitude / 2) * Math.Sin(Latitude / 2) +
                    Math.Cos(ToRadians((double)latitude1)) * Math.Cos(ToRadians((double)latitude2)) *
                    Math.Sin(Longitude / 2) * Math.Sin(Longitude / 2);
            var intermediate = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

            var distance = EarthRadius * intermediate;

            return distance;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        #endregion
    }
}
