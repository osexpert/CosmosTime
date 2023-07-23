﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// TODO
	/// </summary>
	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct UtcTime : IEquatable<UtcTime>, IComparable<UtcTime>, IComparable
	{

		/// <summary>
		/// TODO
		/// </summary>
		public static readonly UtcTime MinValue = UtcTime.FromUtcDateTime(new DateTime(0L, DateTimeKind.Utc));

		/// <summary>
		/// Seems like a bug in DateTime: 
		/// DateTime.MaxValue.ToUniversalTime().Ticks				  -> 3155378939999999999 // no...not UTC max (its lower)
		/// DateTimeOffset.MaxValue.Ticks							  -> 3155378975999999999 // correct
		/// new DateTime(0x2bca2875f4373fffL, DateTimeKind.Utc).Ticks -> 3155378975999999999 // correct
		/// </summary>
		public static readonly UtcTime MaxValue = UtcTime.FromUtcDateTime(new DateTime(0x2bca2875f4373fffL, DateTimeKind.Utc)); // snatched from DateTime

		DateTime _utc;

		/// <summary>
		/// Kind is always Utc
		/// Naming: DateTimeOffset has UtcDateTime and LocalDateTime, so follow same naming {kind}DateTime
		/// </summary>
		public DateTime UtcDateTime => _utc;

		/// <summary>
		/// Now in Utc
		/// </summary>
		public static UtcTime Now => UtcTime.FromUtcDateTime(DateTime.UtcNow);

		/// <summary>
		/// TimeOfDay in Utc
		/// </summary>
		public TimeOnly TimeOfDay => TimeOnly.FromDateTime(_utc);

		/// <summary>
		/// Date in Utc
		/// </summary>
		public DateOnly Date => DateOnly.FromDateTime(_utc);


		// <summary>
		// Date part in Utc
		// </summary>
		//public UtcTime DatePart => _utc.Date.ToUtcTime();

		/// <summary>
		/// Fixed length (28 chars) Iso format in Utc.
		/// Example: 2020-01-20T12:13:14.0000000Z
		/// </summary>
		/// <returns></returns>
		public string ToCosmosDb()
		{
			return _utc.ToString(Constants.FixedLengthIsoFormatWithZ, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Invariant culture
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ToString(string format)
		{
			return _utc.ToString(format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Iso format with variable length milliseconds in utc (Z)
		/// Examples: 
		/// 2020-01-20T12:13:14.1234Z
		/// 2020-01-20T12:13:14.123Z
		/// 2020-01-20T12:13:14.123456Z
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _utc.ToString(Constants.VariableLengthMicrosIsoFormatWithZ, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// DateTime must be Kind Utc, else will throw
		/// </summary>
		public static UtcTime FromUtcDateTime(DateTime utcTime)
		{
			if (utcTime.Kind != DateTimeKind.Utc)
				throw new ArgumentException("kind must be utc");

			// Since Kind now is either Utc or Local, ToUniversalTime is predictable.
			return new UtcTime { _utc = utcTime };
		}

		/// <summary>
		/// DateTime must be Local, else will throw
		/// </summary>
		public static UtcTime FromLocalDateTime(DateTime localTime)
		{
			if (localTime.Kind != DateTimeKind.Local)
				throw new ArgumentException("kind must be local");

			// Since Kind now is either Utc or Local, ToUniversalTime is predictable.
			return new UtcTime { _utc = localTime.ToUniversalTime() };
		}

#if false
		/// <summary>
		/// If anyTime.Kind is Local, then tz must be TimeZoneInfo.Local
		/// If anyTime.Kind is Utc, then tz must be TimeZoneInfo.Utc
		/// If anyTime.Kind is Unspecified, then tz can be anything and anyTime is converted from this zone into Utc.
		/// </summary>
		public static UtcTime FromAnyDateTime(DateTime anyTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException();

			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				// ConvertTimeToUtc will verify the time is valid in the zone
				// For ambigous time, will chose standard time offset
				return new UtcTime { _utc = TimeZoneInfo.ConvertTimeToUtc(anyTime, tz) };
			}
			else if (anyTime.Kind == DateTimeKind.Local)
			{
				if (tz != TimeZoneInfo.Local)
					throw new ArgumentException("When anyTime.Kind is Local, tz must be TimeZoneInfo.Local");

				return new UtcTime { _utc = anyTime.ToUniversalTime() };
			}
			else if (anyTime.Kind == DateTimeKind.Utc)
			{
				if (tz != TimeZoneInfo.Utc)
					throw new ArgumentException("When anyTime.Kind is Utc, tz must be TimeZoneInfo.Utc");

				return new UtcTime { _utc = anyTime };
			}
			else
			{
				throw new Exception("impossible");
			}
		}


		/// <summary>
		/// If anyTime.Kind is Local, then tz must be TimeZoneInfo.Local
		/// If anyTime.Kind is Utc, then tz must be TimeZoneInfo.Utc
		/// If anyTime.Kind is Unspecified, then tz can be anything
		/// 
		/// The only reason to use this ctor with offset is that you have an ambigous time and want to choose the offset manually (or you simply know it up front)
		/// </summary>
		public static UtcTime FromAnyDateTime(DateTime anyTime, TimeZoneInfo tz, TimeSpan offset)
		{
			if (tz == null)
				throw new ArgumentNullException();

			(var ok, var msg) = Shared.ValidateOffset(tz, anyTime, offset);
			if (!ok)
				throw new ArgumentException(msg);

			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				// ConvertTimeToUtc will verify the time is valid in the zone
				//_utc = TimeZoneInfo.ConvertTimeToUtc(anyTime, tz); // TODO: test

				return new UtcTime { _utc = DateTime.SpecifyKind(anyTime - offset, DateTimeKind.Utc) };

			}
			else if (anyTime.Kind == DateTimeKind.Local)
			{
				if (tz != TimeZoneInfo.Local)
					throw new ArgumentException("When anyTime.Kind is Local, tz must be TimeZoneInfo.Local");

				return new UtcTime { _utc = anyTime.ToUniversalTime() };
			}
			else if (anyTime.Kind == DateTimeKind.Utc)
			{
				if (tz != TimeZoneInfo.Utc)
					throw new ArgumentException("When anyTime.Kind is Utc, tz must be TimeZoneInfo.Utc");

				return new UtcTime { _utc = anyTime };
			}
			else
			{
				throw new Exception("impossible");
			}
		}
#endif

		/// <summary>
		/// Offset is only used if Kind is Unspecified
		/// </summary>
		public static UtcTime FromUnspecifiedDateTime(DateTime unspecifiedTime, TimeSpan offset)
		{
			if (unspecifiedTime.Kind != DateTimeKind.Unspecified)
				throw new ArgumentException("Kind must be unspecified");

			Shared.ValidateOffset(offset);

			return new UtcTime { _utc = DateTime.SpecifyKind(unspecifiedTime - offset, DateTimeKind.Utc) };
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="unspecifiedTime"></param>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static UtcTime FromUnspecifiedDateTime(DateTime unspecifiedTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException();

			if (unspecifiedTime.Kind != DateTimeKind.Unspecified)
				throw new ArgumentException("Kind must be unspecified");

			
			// ConvertTimeToUtc will verify the time is valid in the zone
			// For ambigous time, will chose standard time offset
			return new UtcTime { _utc = TimeZoneInfo.ConvertTimeToUtc(unspecifiedTime, tz) };
		}


		/// <summary>
		/// Ticks in utc
		/// </summary>
		public long Ticks => _utc.Ticks;

	//	public ZonedTime ToLocalZoneTime() => new ZonedTime(this, TimeZoneInfo.Local);

		//public ZonedTime ToUtcZoneTime(TimeZoneInfo tz) => new ZonedTime(this, tz);

		/// <summary>
		/// year, month, day, etc. is time in Utc
		/// </summary>
		public UtcTime(int year, int month, int day) : this()
		{
			_utc = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
		}

		/// <summary>
		/// year, month, day, etc. is time in Utc
		/// </summary>
		public UtcTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_utc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
		}

		/// <summary>
		/// year, month, day, etc. is time in Utc
		/// </summary>
		public UtcTime(int year, int month, int day, int hour, int minute, int second, int millis) : this()
		{
			_utc = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Utc);
		}

		//public UtcTime Min(UtcTime other)
		//{
		//	if (this._utc < other._utc)
		//		return this;
		//	else
		//		return other;
		//}
		//public UtcTime Max(UtcTime other)
		//{
		//	if (this._utc > other._utc)
		//		return this;
		//	else
		//		return other;
		//}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static TimeSpan operator -(UtcTime a, UtcTime b) => a._utc - b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="d"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static UtcTime operator +(UtcTime d, TimeSpan t) => UtcTime.FromUtcDateTime(d._utc + t);
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="d"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static UtcTime operator -(UtcTime d, TimeSpan t) => UtcTime.FromUtcDateTime(d._utc - t);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(UtcTime a, UtcTime b) => a._utc == b._utc;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(UtcTime a, UtcTime b) => a._utc != b._utc;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <(UtcTime a, UtcTime b) => a._utc < b._utc;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >(UtcTime a, UtcTime b) => a._utc > b._utc;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <=(UtcTime a, UtcTime b) => a._utc <= b._utc;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >=(UtcTime a, UtcTime b) => a._utc >= b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="sec"></param>
		/// <returns></returns>
		public UtcTime AddSeconds(double sec) => UtcTime.FromUtcDateTime(_utc.AddSeconds(sec));
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="min"></param>
		/// <returns></returns>
		public UtcTime AddMinutes(double min) => UtcTime.FromUtcDateTime(_utc.AddMinutes(min));
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
		public UtcTime AddHours(double h) => UtcTime.FromUtcDateTime(_utc.AddHours(h));
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="days"></param>
		/// <returns></returns>
		public UtcTime AddDays(double days) => UtcTime.FromUtcDateTime(_utc.AddDays(days));

		/// <summary>
		/// TODO
		/// kind of both is utc 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(UtcTime other) => _utc.Equals(other._utc);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) => obj is UtcTime other && Equals(other);

		/// <inheritdoc/>
		public override int GetHashCode() => _utc.GetHashCode();

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(UtcTime other) => _utc.CompareTo(other._utc);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((UtcTime)obj);
		}

		/// <summary>
		/// Parse Utc fixed length format with 28 chars and ending with Z.
		/// Example: 2020-01-20T12:13:14.0000000Z
		/// </summary>
		public static UtcTime ParseCosmosDb(string utc)
		{
			// yyyy-MM-ddTHH:mm:ss.fffffffZ
			if (utc.Length != 28)
				throw new FormatException("not 28 chars");

			// does verify the length, but do it outselfs anyways to be sure
			var dt = DateTime.ParseExact(utc, Constants.FixedLengthIsoFormatWithZ, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind /* needed? yes, else kind is wrong*/);
			return UtcTime.FromUtcDateTime(dt);
		}

		/// <summary>
		/// Only allows {utc}Z or {local}[+-]{offset}
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static UtcTime Parse(string str)
		{
			if (TryParse(str, out var ut))
				return ut;
			throw new FormatException("Format must be {utc}Z or {local}[+-]{offset}");
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="str"></param>
		/// <param name="getOffsetIfNone"></param>
		/// <returns></returns>
		/// <exception cref="FormatException"></exception>
		public static UtcTime Parse(string str, Func<DateTimeOffset, TimeSpan> getOffsetIfNone)
		{
			if (TryParse(str, out var ut, getOffsetIfNone))
				return ut;
			throw new FormatException("Format must be {utc}Z or {local}[+-]{offset}");
		}



		/// <summary>
		/// Parse any Iso time in utc or local[+-]offset. Example:
		/// <para>2020-01-01Z</para>
		/// <para>2020-01-01T12:12:12Z</para>
		/// <para>2020-01-01T12:12:12.123Z</para>
		/// <para>2020-01-01T12:12:12.123+00:30</para>
		/// 
		/// Time without zone can not be parsed. Example:
		/// <para>2020-01-01T12:12:12.123</para>
		/// </summary>
		/// <param name="str"></param>
		/// <param name="utc"></param>
		/// <returns></returns>
		public static bool TryParse(string str, out UtcTime utc)
		{
			utc = default;

			if (IsoTimeParser.TryParseAsIso(str, out DateTimeOffset dto, out var offsetKind) && offsetKind != OffsetKind.None)
			{
				utc = UtcTime.FromUtcDateTime(dto.UtcDateTime);
				return true;
			}

			return false;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="str"></param>
		/// <param name="utc"></param>
		/// <param name="getOffsetIfNone"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static bool TryParse(string str, out UtcTime utc, Func<DateTimeOffset, TimeSpan> getOffsetIfNone)
		{
			if (getOffsetIfNone == null)
				throw new ArgumentNullException(nameof(getOffsetIfNone));

			utc = default;

			if (IsoTimeParser.TryParseAsIso(str, out DateTimeOffset dto, out var offsetKind))
			{
				if (offsetKind == OffsetKind.None)
				{
					var offset = getOffsetIfNone(dto);
					utc = UtcTime.FromUnspecifiedDateTime(dto.DateTime, offset);
					return true;
				}
				else
				{
					utc = UtcTime.FromUtcDateTime(dto.UtcDateTime);
					return true;
				}
			}

			return false;
		}


	}


}
