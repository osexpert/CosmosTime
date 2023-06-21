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


	


	//	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct LocalTime : IEquatable<LocalTime>, IComparable<LocalTime>, IComparable
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

		DateTime _local;

		/// <summary>
		/// Kind: always Local? Or...what if Local == Utc??
		/// </summary>
		public DateTime LocalDateTime => _local;

		public UtcTime ToUtcTime() => _local.ToUtcTime();

		public static LocalTime LocalNow => DateTime.Now.ToLocalTime2();

		public LocalTime Date => _local.Date.ToLocalTime2();

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// TODO: why not allow Local?? Does now.
		/// </summary>
		/// <param name="utcTime"></param>
		public LocalTime(DateTime utcOrLocalTime)
		{
			if (utcOrLocalTime.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("unspecified kind not allowed");

			// Since Kind now is either Utc or Local, ToLocalTime is predictable.
			_local = utcOrLocalTime.ToLocalTime();
		}

		public LocalTime(DateTime anyTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				_local = TimeZoneInfo.ConvertTime(anyTime, tz, TimeZoneInfo.Local); // TODO: test
			}
			else if (anyTime.Kind == DateTimeKind.Local)
			{
				if (tz != TimeZoneInfo.Local)
					throw new ArgumentException("anyTime.Kind is Local with tz is not local");

				_local = anyTime;
			}
			else if (anyTime.Kind == DateTimeKind.Utc)
			{
				if (tz != TimeZoneInfo.Utc)
					throw new ArgumentException("anyTime.Kind is Utz while tz is not utc");

				_local = anyTime.ToLocalTime(); // what if local tz is utc?? Kind will still be Local
			}
			else
			{
				// Since Kind now is either Utc or Local, ToUniversalTime is predictable.
				throw new Exception("impossible");
			}
		}


		public long Ticks => _local.Ticks;


		public LocalTime(int year, int month, int day) : this()
		{
			// what kind to use? What if current tz is Utc?
			// does this make sense??? I guess both kinds are correct here, but maybe we should favour utc...
			_local = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local); 
				//TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local);
		}

		public LocalTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_local = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
			//TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local); // hmm...
		}

		public LocalTime Min(LocalTime other)
		{
			if (this._local < other._local)
				return this;
			else
				return other;
		}
		public LocalTime Max(LocalTime other)
		{
			if (this._local > other._local)
				return this;
			else
				return other;
		}

		public static TimeSpan operator -(LocalTime a, LocalTime b) => a._local - b._local;
		public static LocalTime operator -(LocalTime d, TimeSpan t) => (d._local - t).ToLocalTime2();
		public static LocalTime operator +(LocalTime d, TimeSpan t) => (d._local + t).ToLocalTime2();

		public static bool operator ==(LocalTime a, LocalTime b) => a._local == b._local;
		public static bool operator !=(LocalTime a, LocalTime b) => a._local != b._local;
		public static bool operator <(LocalTime a, LocalTime b) => a._local < b._local;
		public static bool operator >(LocalTime a, LocalTime b) => a._local > b._local;
		public static bool operator <=(LocalTime a, LocalTime b) => a._local <= b._local;
		public static bool operator >=(LocalTime a, LocalTime b) => a._local >= b._local;

		public LocalTime AddSeconds(double sec) => _local.AddSeconds(sec).ToLocalTime2();
		public LocalTime AddMinutes(double min) => _local.AddMinutes(min).ToLocalTime2();
		public LocalTime AddHours(double h) => _local.AddHours(h).ToLocalTime2();
		public LocalTime AddDays(double days) => _local.AddDays(days).ToLocalTime2();


		public bool Equals(LocalTime other) => _local.Equals(other._local);

		public override bool Equals(object obj) => obj is LocalTime other && Equals(other);

		public override int GetHashCode() => _local.GetHashCode();

		public int CompareTo(LocalTime other) => _local.CompareTo(other._local);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((LocalTime)obj);
		}

		public override string ToString() => _local.ToString();
		
	}


}
