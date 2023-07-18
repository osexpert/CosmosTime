using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{

	// What does DateTime.Now kind return if Utc is local tz?
	//	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct ClockTime : IEquatable<ClockTime>, IComparable<ClockTime>, IComparable
	{

		public static readonly ClockTime MinValue = ToClockTime(new DateTime(0L, DateTimeKind.Unspecified));
		public static readonly ClockTime MaxValue = ToClockTime(new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Unspecified));

		DateTime _clock_time;

		/// <summary>
		/// Kind: always Unpecified
		/// </summary>
		public DateTime ClockDateTime => _clock_time;

		//public UtcTime ToUtcTime() => _clock_time.ToUtcTime();

		//public static ClockTime Now => DateTime.Now.ToClockTime();

		public ClockTime DatePart => ToClockTime(_clock_time.Date);

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// TODO: why not allow Local?? Does now.
		/// 
		/// TODO: remove?
		/// </summary>
		/// <param name="utcTime"></param>
		//public ClockTime(DateTime utcOrLocalTime)
		//{
		//	if (utcOrLocalTime.Kind == DateTimeKind.Unspecified)
		//		throw new ArgumentException("unspecified kind not allowed");

		//	// Since Kind now is either Utc or Local, ToLocalTime is predictable.
		//	_clock_time = DateTime.SpecifyKind(utcOrLocalTime.ToLocalTime(), DateTimeKind.Unspecified);
		//}

		public ClockTime(UtcZoneTime zoned)
		{
			_clock_time = zoned.OffsetTime.ClockDateTime;
		}

		/// <summary>
		/// /// TODO: remove?
		/// </summary>
		//public ClockTime(DateTime anyTime, TimeZoneInfo tz)
		//{
		//	if (tz == null)
		//		throw new ArgumentNullException("tz");

		//	if (anyTime.Kind == DateTimeKind.Unspecified)
		//	{
		//		// does this work and make sense??
		//		_clock_time = DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(anyTime, tz, TimeZoneInfo.Local), DateTimeKind.Unspecified);
		//	}
		//	else if (anyTime.Kind == DateTimeKind.Local)
		//	{
		//		if (tz != TimeZoneInfo.Local)
		//			throw new ArgumentException("anyTime.Kind is Local with tz is not local");

		//		_clock_time = DateTime.SpecifyKind(anyTime, DateTimeKind.Unspecified);
		//	}
		//	else if (anyTime.Kind == DateTimeKind.Utc)
		//	{
		//		if (tz != TimeZoneInfo.Utc)
		//			throw new ArgumentException("anyTime.Kind is Utc while tz is not utc");

		//		_clock_time = DateTime.SpecifyKind(anyTime.ToLocalTime(), DateTimeKind.Unspecified); // what if local tz is utc?? Kind will still be Local
		//	}
		//	else
		//	{
		//		throw new Exception("impossible");
		//	}
		//}

		private static ClockTime ToClockTime(DateTime unspecTime)
		{
			if (unspecTime.Kind != DateTimeKind.Unspecified)
				throw new ArgumentException("Unspecified kind required");
			return new ClockTime { _clock_time = unspecTime };
		}

		public long Ticks => _clock_time.Ticks;


		public ClockTime(int year, int month, int day) : this()
		{
			// what kind to use? What if current tz is Utc?
			// does this make sense??? I guess both kinds are correct here, but maybe we should favour utc...
			_clock_time = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified); 
				//TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local);
		}

		public ClockTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
			//TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local); // hmm...
		}

		public ClockTime(int year, int month, int day, int hour, int minute, int second, int millis) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			//TimeZoneInfo.Local == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Local); // hmm...
		}

		//public ClockTime Min(ClockTime other)
		//{
		//	if (this._local < other._local)
		//		return this;
		//	else
		//		return other;
		//}
		//public ClockTime Max(ClockTime other)
		//{
		//	if (this._local > other._local)
		//		return this;
		//	else
		//		return other;
		//}

		public static TimeSpan operator -(ClockTime a, ClockTime b) => a._clock_time - b._clock_time;
		public static ClockTime operator -(ClockTime d, TimeSpan t) => ToClockTime(d._clock_time - t);
		public static ClockTime operator +(ClockTime d, TimeSpan t) => ToClockTime(d._clock_time + t);

		public static bool operator ==(ClockTime a, ClockTime b) => a._clock_time == b._clock_time;
		public static bool operator !=(ClockTime a, ClockTime b) => a._clock_time != b._clock_time;
		public static bool operator <(ClockTime a, ClockTime b) => a._clock_time < b._clock_time;
		public static bool operator >(ClockTime a, ClockTime b) => a._clock_time > b._clock_time;
		public static bool operator <=(ClockTime a, ClockTime b) => a._clock_time <= b._clock_time;
		public static bool operator >=(ClockTime a, ClockTime b) => a._clock_time >= b._clock_time;

		public ClockTime AddSeconds(double sec) => ToClockTime(_clock_time.AddSeconds(sec));
		public ClockTime AddMinutes(double min) => ToClockTime(_clock_time.AddMinutes(min));
		public ClockTime AddHours(double h) => ToClockTime(_clock_time.AddHours(h));
		public ClockTime AddDays(double days) => ToClockTime(_clock_time.AddDays(days));


		public bool Equals(ClockTime other) => _clock_time.Equals(other._clock_time);

		public override bool Equals(object obj) => obj is ClockTime other && Equals(other);

		public override int GetHashCode() => _clock_time.GetHashCode();

		public int CompareTo(ClockTime other) => _clock_time.CompareTo(other._clock_time);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ClockTime)obj);
		}

		public override string ToString() => _clock_time.ToString(Constants.VariableLengthMicrosIsoFormatWithoutZ, CultureInfo.InvariantCulture);
		
	}


}
