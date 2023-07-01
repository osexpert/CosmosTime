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

		/// <summary>
		/// DateTime must be Kind.Utc or Kind.Local.
		/// If Kind.Unspecified, it will throw (must then use ToZonedTime that take a tz)
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static ZonedTime ToZonedTime(this DateTime utcOrLocalTime)
		{
			return new ZonedTime(utcOrLocalTime);
		}

		/// <summary>
		/// DateTime can be any kind.
		/// If Kind.Unspecified then time is adjusted to supplied tz.
		/// If kind Kind.Utc or Kind.Local, it is simply validated that the tz matches.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static ZonedTime ToZonedTime(this DateTime anyTime, TimeZoneInfo tz)
		{
			return new ZonedTime(anyTime, tz);
		}


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

		public static UtcOffsetTime ToUtcOffsetTime(this DateTimeOffset dto)
		{
			return new UtcOffsetTime(dto);
		}

		public static UtcOffsetTime? ToUtcOffsetTime(this DateTimeOffset? dto)
		{
			if (dto == null)
				return null;
			return new UtcOffsetTime(dto.Value);
		}

		public static DateTime? ToUtcDateTime(this UtcTime? utc)
		{
			if (utc == null)
				return null;
			return utc.Value.UtcDateTime;
		}
	}
}
