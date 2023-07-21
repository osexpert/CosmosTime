
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{

	/// <summary>
	/// The time as you see on the clock:-)
	/// No info about time zone or anything.
	/// No context.
	/// Backed by a DateTime of Unspecified kind.
	/// </summary>

	[TypeConverter(typeof(ClockTimeTypeConverter))]
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



		public static ClockTime LocalNow => Now(TimeZoneInfo.Local);

		public static ClockTime UtcNow => Now(TimeZoneInfo.Utc);

		public static ClockTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			if (tz == TimeZoneInfo.Local)
				return ToClockTime_MakeUnspecified(DateTime.Now);
			else if (tz == TimeZoneInfo.Utc)
				return ToClockTime_MakeUnspecified(DateTime.UtcNow);
			else // convert to time in the zone
			{
				var utcNow = DateTime.UtcNow;
				var dtInTz = TimeZoneInfo.ConvertTime(utcNow, tz);
				// if tz is Utc, kind may be utc?
				return ToClockTime_MakeUnspecified(dtInTz);
			}
		}




		public TimeOnly TimeOfDay => TimeOnly.FromDateTime(_clock_time);

		public DateOnly Date => DateOnly.FromDateTime(_clock_time);

		//public static ClockTime Truncate(ClockTime ct, TimeSpan timeSpan)
		//{
		//	if (timeSpan == TimeSpan.Zero) return ct; // Or could throw an ArgumentException
		//	if (ct == ClockTime.MinValue || ct == ClockTime.MaxValue) return ct; // do not modify "guard" values
		//	return ct.AddTicks(-(ct.Ticks % timeSpan.Ticks));
		//}

		public ClockTime AddTicks(long t) => ToClockTime(_clock_time.AddTicks(t));

		private static ClockTime ToClockTime_MakeUnspecified(DateTime t) => ToClockTime(DateTime.SpecifyKind(t, DateTimeKind.Unspecified));

		//public ClockTime(ZoneTime zoned)
		//{
		//	_clock_time = zoned.OffsetTime.ClockDateTime;
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
			_clock_time = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);
		}

		public ClockTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
		}

		public ClockTime(int year, int month, int day, int hour, int minute, int second, int millis) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
		}

		public ClockTime(DateTime anyTime)
		{
			_clock_time = DateTime.SpecifyKind(anyTime, DateTimeKind.Unspecified);
		}

		public ClockTime(long ticks)
		{
			_clock_time = new DateTime(ticks, DateTimeKind.Unspecified);
		}

		//public ClockTime Min(ClockTime other)
		//{
		//	if (this._clock_time < other._clock_time)
		//		return this;
		//	else
		//		return other;
		//}
		//public ClockTime Max(ClockTime other)
		//{
		//	if (this._clock_time > other._clock_time)
		//		return this;
		//	else
		//		return other;
		//}

		public static TimeSpan operator -(ClockTime a, ClockTime b) => a._clock_time - b._clock_time;

		public static ClockTime operator +(ClockTime d, TimeSpan t) => ToClockTime(d._clock_time + t);
		public static ClockTime operator -(ClockTime d, TimeSpan t) => ToClockTime(d._clock_time - t);

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

		public static bool TryParse(string str, out ClockTime clockTime)
		{
			clockTime = default;

			if (IsoTimeParser.TryParseAsIso(str, out DateTimeOffset dto, out var tzk))
			{
				clockTime = ToClockTime(dto.DateTime);
				return true;
			}

			return false;
		}

		public static ClockTime Parse(string str)
		{
			if (TryParse(str, out var ct))
				return ct;
			else
				throw new FormatException();
		}
	}

}
