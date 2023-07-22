using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// TODO
	/// </summary>
	public static class Shared
	{
		//internal static (bool Ok, string Msg) ValidateOffset(ZonedTime zoned, short offsetMinutes)
		//{
		//	if (offsetMinutes < -840 || offsetMinutes > 840)
		//		return (false, "offset must be max [+-] 14 hours");

		//	// FIXME: is there an easier\more effective way to validate this?
		//	if (zoned.Zone.IsAmbiguousTime(zoned.ZonedDateTime))
		//	{
		//		var validOffsets = zoned.Zone.GetAmbiguousTimeOffsets(zoned.ZonedDateTime);

		//		if (!validOffsets.Any(o => o.TotalMinutes == offsetMinutes))
		//			return (false, "Offset is not valid in zone (none of the ambiguous offsets)");
		//	}
		//	else if (zoned.Zone.GetUtcOffset(zoned.ZonedDateTime).TotalMinutes != offsetMinutes)
		//	{
		//		return (false, "Offset is not valid in zone");
		//	}

		//	return (true, null);
		//}

		internal static (bool Ok, string Msg) ValidateOffset(TimeZoneInfo tz, DateTime zonedDateTime, TimeSpan offset)
		{
			// zonedDateTime should never be Local kind
			// zonedDateTime should be Kind Utc if tz is Utc

			var offsetMinutes = Shared.GetWholeMinutes(offset);

			if (offsetMinutes < -840 || offsetMinutes > 840)
				return (false, "offset must be max [+-] 14 hours");

			// FIXME: is there an easier\more effective way to validate this?
			if (tz.IsAmbiguousTime(zonedDateTime))
			{
				var validOffsets = tz.GetAmbiguousTimeOffsets(zonedDateTime);

				if (!validOffsets.Any(o => o.TotalMinutes == offsetMinutes))
					return (false, "Offset is not valid in zone (none of the ambiguous offsets)");
			}
			else if (tz.GetUtcOffset(zonedDateTime).TotalMinutes != offsetMinutes)
			{
				return (false, "Offset is not valid in zone");
			}

			return (true, null);
		}

		//internal static short GetWholeMinutes(double mins)
		//{
		//	var res = (short)mins;
		//	if (res != mins)
		//		throw new Exception("fractions lost in offset");
		//	return res;
		//}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static short GetWholeMinutes(TimeSpan timeSpan)
		{
			var mins = timeSpan.TotalMinutes;
			var res = (short)mins;
			if (res != mins)
				throw new Exception("fractions lost in offset");
			return res;
		}

		internal static TimeSpan[] GetUtcOffsets(TimeZoneInfo tz, DateTime time)
		{
			if (tz.IsAmbiguousTime(time))
			{
				var offsets = tz.GetAmbiguousTimeOffsets(time);
				return offsets;
			}
			else
			{
				return new[] { tz.GetUtcOffset(time) };
			}
		}

		//internal static TimeZoneInfo GetTimeZoneFromKindUtcOrLocal(DateTime utcOrLocalTime)
		//{
		//	if (utcOrLocalTime.Kind == DateTimeKind.Unspecified)
		//		throw new ArgumentException("unspecified kind not allowed");

		//	// Since Kind now is either Utc or Local, its easy
		//	if (utcOrLocalTime.Kind == DateTimeKind.Local)
		//	{
		//		// can Local tz be Utc? Yes. Should we in this case change Kind of _zoned to Utc? Maybe...
		//		return TimeZoneInfo.Local;
		//	}
		//	else if (utcOrLocalTime.Kind == DateTimeKind.Utc)
		//	{
		//		return TimeZoneInfo.Utc;
		//	}
		//	else
		//		throw new Exception("impossible, still unspec");
		//}
	}
}
