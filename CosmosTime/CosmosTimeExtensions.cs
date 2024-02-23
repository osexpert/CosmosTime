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
        /// <param name="utcTime"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static ZoneTime ToZoneTime(this UtcTime utcTime, TimeZoneInfo tz)
        {
            return new ZoneTime(utcTime, tz);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="offsetTime"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static ZoneTime ToZoneTime(this OffsetTime offsetTime, TimeZoneInfo tz)
        {
            return new ZoneTime(offsetTime, tz);
        }

        /// <summary>
        /// Convert to time in another zone via Utc.
        /// </summary>
        /// <param name="zoneTime"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static ZoneTime ToZoneTime(this ZoneTime zoneTime, TimeZoneInfo tz)
        {
            return new ZoneTime(zoneTime.ToUtcTime(), tz);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static OffsetTime ToOffsetTime(this UtcTime utcTime, TimeSpan offset)
        {
            return new OffsetTime(utcTime, offset);
        }
#if false
		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local.
		/// If Kind.Unspecified, it will throw (must then use ToUtzZoneTime that take a tz)
		/// </summary>
		//public static ZoneTime ToZoneTime(this DateTime utcOrLocalTime)
		//{
		//	return new ZoneTime(utcOrLocalTime);
		//}

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local, else will throw
		/// </summary>
		/// <returns></returns>
		//public static ClockTime ToClockTime(this DateTime utcOrLocalTime)
		//{
		//	return new ClockTime(utcOrLocalTime);
		//}
#endif
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="zoneTime"></param>
        /// <returns></returns>
        public static ClockTime ToClockTime(this ZoneTime zoneTime)
        {
            return new ClockTime(zoneTime.Ticks);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="zoneTime"></param>
        /// <returns></returns>
        public static UtcTime ToUtcTime(this ZoneTime zoneTime)
        {
            return zoneTime.OffsetTime.UtcTime;
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="clockTime"></param>
        /// <param name="tz"></param>
        /// <returns></returns>
        public static ZoneTime ToZoneTime(this ClockTime clockTime, TimeZoneInfo tz)
        {
            return new ZoneTime(clockTime, tz);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="clockTime"></param>
        /// <param name="tz"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ZoneTime ToZoneTime(this ClockTime clockTime, TimeZoneInfo tz, TimeSpan offset)
        {
            return new ZoneTime(clockTime, tz, offset);
        }

        //public static UtcZoneTime ToUtcZoneTime(this ClockTime ct, TimeZoneInfo tz, Func<TimeSpan[], TimeSpan> choseOffsetIfAmbigious)
        //{
        //	return new UtcZoneTime(ct, tz, choseOffsetIfAmbigious);
        //}

        // <ummaryy>
        // DateTime must be Kind.Utc or Kind.Local, else will throw
        // </ummaryy>
        // <eturns></eturns>
        //public static UtcTime? ToUtcTime(this DateTime? utcOrLocalTime)
        //{
        //	if (utcOrLocalTime == null)
        //		return null;
        //	return UtcTime.FromUtcOrLocalDateTime(utcOrLocalTime.Value);
        //}

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static OffsetTime ToOffsetTime(this DateTimeOffset dto)
        {
            return new OffsetTime(UtcTime.FromUtcDateTime(dto.UtcDateTime), dto.Offset);
        }

        // <summary>
        // TODO
        // </summary>
        // <param name="dto"></param>
        // <returns></returns>
        //public static OffsetTime? ToOffsetTime(this DateTimeOffset? dto)
        //{
        //	if (dto == null)
        //		return null;
        //	return dto.Value.ToOffsetTime();
        //}

        // <summary>
        // TODO
        // </summary>
        // <param name="utc"></param>
        // <returns></returns>
        //public static DateTime? ToUtcDateTime(this UtcTime? utc)
        //{
        //	if (utc == null)
        //		return null;
        //	return utc.Value.UtcDateTime;
        //}

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
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static IsoWeek GetWeek(this UtcTime utcTime)
        {
            return IsoWeek.GetWeek(utcTime.UtcDateTime);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="offsetTime"></param>
        /// <returns></returns>
        public static IsoWeek GetWeek(this OffsetTime offsetTime)
        {
            return IsoWeek.GetWeek(offsetTime.ClockDateTime);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="zoneTime"></param>
        /// <returns></returns>
        public static IsoWeek GetWeek(this ZoneTime zoneTime)
        {
            return IsoWeek.GetWeek(zoneTime.OffsetTime.ClockDateTime);
        }

        // <summary>
        // TODO
        // </summary>
        // <param name="tz"></param>
        // <param name="dt"></param>
        // <param name="chooseOffsetIfAmbigous"></param>
        // <returns></returns>
        // <exception cref="ArgumentNullException"></exception>
        //public static TimeSpan GetUtcOffset(this TimeZoneInfo tz, DateTime dt, Func<TimeSpan[], TimeSpan> chooseOffsetIfAmbigous)
        //{
        //	if (chooseOffsetIfAmbigous == null)
        //		throw new ArgumentNullException(nameof(chooseOffsetIfAmbigous));

        //	if (tz.IsAmbiguousTime(dt))
        //	{
        //		var offsets = tz.GetAmbiguousTimeOffsets(dt);
        //		return chooseOffsetIfAmbigous(offsets);
        //	}
        //	else
        //	{
        //		return tz.GetUtcOffset(dt);
        //	}
        //}

        // <summary>
        // TODO
        // </summary>
        // <param name="tz"></param>
        // <param name="dto"></param>
        // <param name="chooseOffsetIfAmbigous"></param>
        // <returns></returns>
        // <exception cref="ArgumentNullException"></exception>
        //public static TimeSpan GetUtcOffset(this TimeZoneInfo tz, DateTimeOffset dto, Func<TimeSpan[], TimeSpan> chooseOffsetIfAmbigous)
        //{
        //	if (chooseOffsetIfAmbigous == null)
        //		throw new ArgumentNullException(nameof(chooseOffsetIfAmbigous));

        //	if (tz.IsAmbiguousTime(dto))
        //	{
        //		var offsets = tz.GetAmbiguousTimeOffsets(dto);
        //		return chooseOffsetIfAmbigous(offsets);
        //	}
        //	else
        //	{
        //		return tz.GetUtcOffset(dto);
        //	}
        //}

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
