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
		/// DateTime must be Kind.Utc or Kind.Local.
		/// If Kind.Unspecified, it will throw (must then use ToZonedTime that take a tz)
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static ZonedTime ToZonedTime(this DateTime utcOrLocalTime)
		//{
		//	return new ZonedTime(utcOrLocalTime);
		//}

		/// <summary>
		/// DateTime can be any kind.
		/// If Kind.Unspecified then time is adjusted to supplied tz.
		/// If kind Kind.Utc or Kind.Local, it is simply validated that the tz matches.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static ZonedTime ToZonedTime(this DateTime anyTime, TimeZoneInfo tz)
		//{
		//	return new ZonedTime(anyTime, tz);
		//}

		//public static ZonedTime ToZonedTime(this UtcTime utc, TimeZoneInfo tz)
		//{
		//	return new ZonedTime(utc, tz);
		//}

		public static OffsetTime ToOffsetTime(this UtcTime utc, TimeSpan offset)
		{
			return new OffsetTime(utc, offset);
		}

		public static ZonedTime ToZonedTime(this DateTime utcOrLocalTime)
		{
			return new ZonedTime(utcOrLocalTime);
		}

		//public static int TotalWholeMinutes(this TimeSpan timeSpan)
		//{
		//	//return Shared.GetWholeMinutes(timeSpan);
		//	var mins = timeSpan.TotalMinutes;
		//	var res = (int)mins;
		//	if (res != mins)
		//		throw new Exception("not whole minutes (has fractions)");
		//	return res;
		//}

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local, else will throw
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static LocalTime ToLocalTime2(this DateTime utcOrLocalTime)
		//{
		//	return new LocalTime(utcOrLocalTime);
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

		/// <summary>
		/// Will convert from local to Utc. Should ideally only be used on a client.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static UtcTime LocalToUtcTime(this DateTime dt)
		//{
		//	return new UtcTime(dt.ToUniversalTime());
		//}

		/// <summary>
		/// Will convert from local to Utc. Should ideally only be used on a client.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		//public static UtcTime? LocalToUtcTime(this DateTime? dt)
		//{
		//	if (dt == null)
		//		return null;
		//	return new UtcTime(dt.Value.ToUniversalTime());
		//}

		public static OffsetTime ToOffsetTime(this DateTimeOffset dto)
		{
			return new OffsetTime(dto);
		}

		public static OffsetTime? ToOffsetTime(this DateTimeOffset? dto)
		{
			if (dto == null)
				return null;
			return new OffsetTime(dto.Value);
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
			return IsoWeek.GetWeek(dt.UnspecifiedDateTime);
		}

		public static IsoWeek GetWeek(this ZonedTime dt)
		{
			return IsoWeek.GetWeek(dt.OffsetTime.UnspecifiedDateTime);
		}

	}
}
