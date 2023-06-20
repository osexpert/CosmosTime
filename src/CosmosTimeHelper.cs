using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	public static class CosmosTimeHelper
	{

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// not anymore...
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime ToUtcTime(this DateTime utcOrLocalTime)
		{
			return new UtcTime(utcOrLocalTime);
		}


		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// no...
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static ZonedTime ToZonedTime(this DateTime utcOrLocalTime)
		{
			return new ZonedTime(utcOrLocalTime);
		}

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// no...
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static ZonedTime ToZonedTime(this DateTime anyTime, TimeZoneInfo tzIfUnspecified)
		{
			return new ZonedTime(anyTime, tzIfUnspecified);
		}


		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// no...
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static WhatTime ToWhatTime(this DateTime utcOrLocalTime)
		{
			return new WhatTime(utcOrLocalTime);
		}



		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
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
