using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{

	// Ideally we want the internal DateTime kind to always be local here!
	// But what if the system tz is utc?

	// What does DateTime.Now kind return if Utc is local tz?


	// What time? = funny = unspecified point of time


	//	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct WhatTime : IEquatable<WhatTime>, IComparable<WhatTime>, IComparable
	{
		//public const string FixedLengthFormatWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffffff";
		//// this is almost the same as "o" format (roundtrip), except roundtrip uses K (kind) instead of Z (zulu)
		//public const string FixedLengthFormatWithZ = FixedLengthFormatWithoutZ + "Z";

		//public const string VariableLengthFormatWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'FFFFFFF";
		//public const string VariableLengthFormatWithZ = VariableLengthFormatWithoutZ + "Z";

		//public static readonly UtcTime MinValue = new DateTime(0L, DateTimeKind.Utc).ToUtcTime();
		///// <summary>
		///// Seems like a bug in DateTime: 
		///// DateTime.MaxValue.ToUniversalTime().Ticks				  -> 3155378939999999999 // no...not UTC max (its lower)
		///// DateTimeOffset.MaxValue.Ticks							  -> 3155378975999999999 // correct
		///// new DateTime(0x2bca2875f4373fffL, DateTimeKind.Utc).Ticks -> 3155378975999999999 // correct
		///// </summary>
		//public static readonly UtcTime MaxValue = new DateTime(0x2bca2875f4373fffL, DateTimeKind.Utc).ToUtcTime(); // snatched from DateTime

		DateTime _what;

		/// <summary>
		/// Kind: always Local? Or...what if Local == Utc??
		/// </summary>
		public DateTime WhatDateTime => _what;

		public static WhatTime WhatNow => DateTime.Now.ToWhatTime();

		public WhatTime Date => _what.Date.ToWhatTime();

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// TODO: why not allow Local?? Does now.
		/// </summary>
		/// <param name="utcTime"></param>
		public WhatTime(DateTime utcOrLocalTime)
		{
			if (utcOrLocalTime.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("unspecified kind not allowed");

			// Since Kind now is either Utc or Local, ToLocalTime is predictable.
			_what = utcOrLocalTime.ToLocalTime();
		}

		/// <summary>
		/// DateTime can be any Kind, but unspecifiedKind must be set to either Utc or Local and this Kind will be used if anyTime.Kind is unspecified.
		/// </summary>
		/// <param name="anyTime"></param>
		/// <param name="kindIfUnspecified"></param>
		/// <exception cref="ArgumentException"></exception>
		public WhatTime(DateTime anyTime, DateTimeKind kindIfUnspecified)
		{
			if (kindIfUnspecified == DateTimeKind.Unspecified)
				throw new ArgumentException("kindIfUnspecified can not be Unspecified");

			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				anyTime = DateTime.SpecifyKind(anyTime, kindIfUnspecified);
			}

			// Since Kind now is either Utc or Local, ToLocalTime is predictable.
			_what = anyTime.ToLocalTime();
		}

		public WhatTime(DateTime anyTime, TimeZoneInfo tzIfUnspecified)
		{
			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				_what = TimeZoneInfo.ConvertTime(anyTime, tzIfUnspecified, TimeZoneInfo.Local); // TODO: test

				// is _sys.Kind now local?? Well...what if the Local tz is Utc??
//				if (_sys.Kind != DateTimeKind.Local)
	//				throw new Exception("not local?");
			}
			else
			{
				// Since Kind now is either Utc or Local, ToLocalTime is predictable.
				_what = anyTime.ToLocalTime();
			}
		}


		public long Ticks => _what.Ticks;


		public WhatTime(int year, int month, int day) : this()
		{
			// what kind to use? What if current tz is Utc?
			// does this make sense??? I guess both kinds are correct here, but maybe we should favour utc...
			_what = new DateTime(year, month, day, 0, 0, 0, 
				TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local);
		}

		public WhatTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_what = new DateTime(year, month, day, hour, minute, second, 
				TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local);
		}

		public WhatTime Min(WhatTime other)
		{
			if (this._what < other._what)
				return this;
			else
				return other;
		}
		public WhatTime Max(WhatTime other)
		{
			if (this._what > other._what)
				return this;
			else
				return other;
		}

		public static TimeSpan operator -(WhatTime a, WhatTime b) => a._what - b._what;
		public static WhatTime operator -(WhatTime d, TimeSpan t) => (d._what - t).ToWhatTime();
		public static WhatTime operator +(WhatTime d, TimeSpan t) => (d._what + t).ToWhatTime();

		public static bool operator ==(WhatTime a, WhatTime b) => a._what == b._what;
		public static bool operator !=(WhatTime a, WhatTime b) => a._what != b._what;
		public static bool operator <(WhatTime a, WhatTime b) => a._what < b._what;
		public static bool operator >(WhatTime a, WhatTime b) => a._what > b._what;
		public static bool operator <=(WhatTime a, WhatTime b) => a._what <= b._what;
		public static bool operator >=(WhatTime a, WhatTime b) => a._what >= b._what;

		public WhatTime AddSeconds(double sec) => _what.AddSeconds(sec).ToWhatTime();
		public WhatTime AddMinutes(double min) => _what.AddMinutes(min).ToWhatTime();
		public WhatTime AddHours(double h) => _what.AddHours(h).ToWhatTime();
		public WhatTime AddDays(double days) => _what.AddDays(days).ToWhatTime();


		public bool Equals(WhatTime other) => _what.Equals(other._what);

		public override bool Equals(object obj) => obj is WhatTime other && Equals(other);

		public override int GetHashCode() => _what.GetHashCode();

		public int CompareTo(WhatTime other) => _what.CompareTo(other._what);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((WhatTime)obj);
		}

	}


}
