using System;

namespace CosmosTime.TimeZone
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class TimeZoneExtensions
    {

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static bool IsUtc(this TimeZoneInfo tz)
        {
            return IanaTimeZone.IsUtc(tz);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static bool HasIanaId(this TimeZoneInfo tz)
        {
            return IanaTimeZone.HasIanaId(tz);
        }
    }
}
