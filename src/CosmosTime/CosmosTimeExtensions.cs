using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	public static class CosmosTimeExtensions
	{

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local.
		/// If Kind.Unspecified, it will throw (must then use ToUtcTime that take a tz)
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime ToUtcTime(this DateTime utcOrLocalTime)
		{
			return new UtcTime(utcOrLocalTime);
		}



		public static UtcTime ToUtcTime(this DateTime anyTime, TimeZoneInfo tz)
		{
			return new UtcTime(anyTime, tz);
		}

		public static UtcTime ToUtcTime(this DateTime anyTime, TimeZoneInfo tz, TimeSpan offset)
		{
			return new UtcTime(anyTime, tz, offset);
		}


		//public static DateOnly GetDateOnly(this DateTime dt)
		//{
		//	return new DateOnly(dt.Year, dt.Month, dt.Day);
		//}

		//public static UtcTime ToUtcTime(this ZonedTime zoned)
		//{
		//	return new UtcTime(zoned.ZonedDateTime, zoned.Zone);
		//}


		/// <summary>
		/// DateTime can be any kind.
		/// If Kind.Unspecified then time is adjusted to supplied tz.
		/// If kind Kind.Utc or Kind.Local, it is simply validated that the tz matches.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static ZonedTime ToUtcZoneTime(this DateTime anyTime, TimeZoneInfo tz)
		//{
		//	return new ZonedTime(anyTime, tz);
		//}

		public static ZoneTime ToZoneTime(this DateTime anyTime, TimeZoneInfo tz)
		{
			return new ZoneTime(anyTime, tz);
		}

		public static OffsetTime ToOffsetTime(this UtcTime utc, TimeSpan offset)
		{
			return new OffsetTime(utc, offset);
		}

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local.
		/// If Kind.Unspecified, it will throw (must then use ToUtzZoneTime that take a tz)
		/// </summary>
		public static ZoneTime ToZoneTime(this DateTime utcOrLocalTime)
		{
			return new ZoneTime(utcOrLocalTime);
		}

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local, else will throw
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static ClockTime ToClockTime(this DateTime utcOrLocalTime)
		//{
		//	return new ClockTime(utcOrLocalTime);
		//}

		public static ClockTime ToClockTime(this ZoneTime zoned)
		{
			return new ClockTime(zoned);
		}


		public static ZoneTime ToZoneTime(this ClockTime ct, TimeZoneInfo tz)
		{
			return new ZoneTime(ct, tz);
		}

		public static ZoneTime ToZoneTime(this ClockTime ct, TimeZoneInfo tz, TimeSpan offset)
		{
			return new ZoneTime(ct, tz, offset);
		}

		//public static UtcZoneTime ToUtcZoneTime(this ClockTime ct, TimeZoneInfo tz, Func<TimeSpan[], TimeSpan> choseOffsetIfAmbigious)
		//{
		//	return new UtcZoneTime(ct, tz, choseOffsetIfAmbigious);
		//}

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local, else will throw
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime? ToUtcTime(this DateTime? utcOrLocalTime)
		{
			if (utcOrLocalTime == null)
				return null;
			return new UtcTime(utcOrLocalTime.Value);
		}

		public static OffsetTime ToOffsetTime(this DateTimeOffset dto)
		{
			return new OffsetTime(dto.UtcDateTime.ToUtcTime(), dto.Offset);
		}

		public static OffsetTime? ToOffsetTime(this DateTimeOffset? dto)
		{
			if (dto == null)
				return null;
			return dto.Value.ToOffsetTime();
		}

		public static DateTime? ToUtcDateTime(this UtcTime? utc)
		{
			if (utc == null)
				return null;
			return utc.Value.UtcDateTime;
		}

		public static IsoWeek GetWeek(this DateTime dt)
		{
			return IsoWeek.GetWeek(dt);
		}

		public static IsoWeek GetWeek(this UtcTime dt)
		{
			return IsoWeek.GetWeek(dt.UtcDateTime);
		}

		public static IsoWeek GetWeek(this OffsetTime dt)
		{
			return IsoWeek.GetWeek(dt.ClockDateTime);
		}

		public static IsoWeek GetWeek(this ZoneTime dt)
		{
			return IsoWeek.GetWeek(dt.OffsetTime.ClockDateTime);
		}

	}
}
