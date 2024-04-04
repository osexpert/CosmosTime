using System;
using System.Globalization;

namespace CosmosTime
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class CosmosTimeExtensions
    {

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static OffsetTime ToOffsetTime(this DateTimeOffset dto)
        {
            return OffsetTime.FromUtcDateTime(dto.UtcDateTime, dto.Offset);
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IsoWeek GetWeek(this DateTime dt)
        {
            return IsoWeek.GetWeek(dt);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="timeOnly"></param>
        /// <returns></returns>
        public static string ToIsoString(this TimeOnly timeOnly)
        {
            return timeOnly.ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dateOnly"></param>
        /// <returns></returns>
        public static string ToIsoString(this DateOnly dateOnly)
        {
            return dateOnly.ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static DateTime SpecifyKind(this DateTime dt, DateTimeKind kind)
            => DateTime.SpecifyKind(dt, kind);

    }
}
