using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    public static class MapExtensions
    {
        public static string ToLatString(this double lat)
        {
            return lat >= 0 ? string.Format("{0}°N", lat) : string.Format("{0}°S", Math.Abs(lat));
        }
        public static string ToLonString(this double lon)
        {
            return lon >= 0 ? string.Format("{0}°W", lon) : string.Format("{0}°E", Math.Abs(lon));
        }

        /// <summary>
        /// Interpolate Longitude untuk map dengan Data menggunakan 0 BT 180 BT 180BB 0 BB 
        /// sedangkan program menggunakan 180BB 0 180BT
        /// contoh: 102 (BT) direpresentasikan dlm -102 diinterpolasi menjadi -78
        /// contoh: 3 BB direpresentasikan dlm 3 diinterpolasi menjadi 177
        /// </summary>
        /// <returns>Longitude interpolated based on program requires</returns>
        public static double InterpolateLon(this double lonDeg)
        {
            return lonDeg < 0 ? -1 * (180 + lonDeg) : 180 - lonDeg;
        }

        /// <summary>
        /// Return interpolated longitude (-180 0 180) to (0E 180E 180W 0W)
        /// </summary>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double DeInterpolateLon(this double lon)
        {
            return lon < 0 ? -1 * (180 + lon) : 180 - lon;
        }
    }
}
