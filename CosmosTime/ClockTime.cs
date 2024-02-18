
using CosmosTime.TimeZone;
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
		/// <summary>
		/// TODO
		/// </summary>
		public static readonly ClockTime MinValue = ToClockTime(new DateTime(0L, DateTimeKind.Unspecified));
		/// <summary>
		/// TODO
		/// </summary>
		public static readonly ClockTime MaxValue = ToClockTime(new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Unspecified));

		DateTime _clock_time;

		/// <summary>
		/// Kind: always Unpecified
		/// </summary>
		public DateTime ClockDateTime => _clock_time;

		//public UtcTime ToUtcTime() => _clock_time.ToUtcTime();


		/// <summary>
		/// TODO
		/// </summary>
		public static ClockTime LocalNow => Now(TimeZoneInfo.Local);

		/// <summary>
		/// TODO
		/// </summary>
		public static ClockTime UtcNow => Now(TimeZoneInfo.Utc);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static ClockTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			if (tz == TimeZoneInfo.Local)
				return ToClockTime_MakeUnspecified(DateTime.Now);
			else if (tz.IsUtc())
				return ToClockTime_MakeUnspecified(DateTime.UtcNow);
			else // convert to time in the zone
			{
				var utcNow = DateTime.UtcNow;
				var dtInTz = TimeZoneInfo.ConvertTime(utcNow, tz);
				// if tz is Utc, kind may be utc?
				return ToClockTime_MakeUnspecified(dtInTz);
			}
		}




		/// <summary>
		/// TimeOfDay
		/// </summary>
		public TimeOnly TimeOfDay => TimeOnly.FromDateTime(_clock_time);

		/// <summary>
		/// Date
		/// </summary>
		public DateOnly Date => DateOnly.FromDateTime(_clock_time);

		//public static ClockTime Truncate(ClockTime ct, TimeSpan timeSpan)
		//{
		//	if (timeSpan == TimeSpan.Zero) return ct; // Or could throw an ArgumentException
		//	if (ct == ClockTime.MinValue || ct == ClockTime.MaxValue) return ct; // do not modify "guard" values
		//	return ct.AddTicks(-(ct.Ticks % timeSpan.Ticks));
		//}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		public ClockTime AddTicks(long ticks) => ToClockTime(_clock_time.AddTicks(ticks));

		private static ClockTime ToClockTime_MakeUnspecified(DateTime t) => ToClockTime(DateTime.SpecifyKind(t, DateTimeKind.Unspecified));

		//public ClockTime(ZoneTime zoned)
		//{
		//	_clock_time = zoned.OffsetTime.ClockDateTime;
		//}

		private static ClockTime ToClockTime(DateTime unspecTime)
		{
			if (unspecTime.Kind != DateTimeKind.Unspecified)
				throw new ArgumentException("DateTimeKind.Unspecified is required");
			return new ClockTime { _clock_time = unspecTime };
		}

		/// <summary>
		/// Ticks in Clock time
		/// </summary>
		public long Ticks => _clock_time.Ticks;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		public ClockTime(int year, int month, int day) : this()
		{
			_clock_time = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		public ClockTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <param name="millisecond"></param>
		public ClockTime(int year, int month, int day, int hour, int minute, int second, int millisecond) : this()
		{
			_clock_time = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="anyTime"></param>
		public ClockTime(DateTime anyTime)
		{
			_clock_time = DateTime.SpecifyKind(anyTime, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="ticks"></param>
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

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static TimeSpan operator -(ClockTime t1, ClockTime t2) => t1._clock_time - t2._clock_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <param name="ts"></param>
		/// <returns></returns>
		public static ClockTime operator +(ClockTime t, TimeSpan ts) => ToClockTime(t._clock_time + ts);
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <param name="ts"></param>
		/// <returns></returns>
		public static ClockTime operator -(ClockTime t, TimeSpan ts) => ToClockTime(t._clock_time - ts);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator ==(ClockTime t1, ClockTime t2) => t1._clock_time == t2._clock_time;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator !=(ClockTime t1, ClockTime t2) => t1._clock_time != t2._clock_time;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator <(ClockTime t1, ClockTime t2) => t1._clock_time < t2._clock_time;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator >(ClockTime t1, ClockTime t2) => t1._clock_time > t2._clock_time;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator <=(ClockTime t1, ClockTime t2) => t1._clock_time <= t2._clock_time;
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator >=(ClockTime t1, ClockTime t2) => t1._clock_time >= t2._clock_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public ClockTime AddSeconds(double seconds) => ToClockTime(_clock_time.AddSeconds(seconds));
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public ClockTime AddMinutes(double minutes) => ToClockTime(_clock_time.AddMinutes(minutes));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public ClockTime AddHours(double hours) => ToClockTime(_clock_time.AddHours(hours));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="days"></param>
		/// <returns></returns>
		public ClockTime AddDays(double days) => ToClockTime(_clock_time.AddDays(days));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ClockTime other) => _clock_time.Equals(other._clock_time);

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is ClockTime other && Equals(other);

		/// <inheritdoc/>
		public override int GetHashCode() => _clock_time.GetHashCode();

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ClockTime other) => _clock_time.CompareTo(other._clock_time);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ClockTime)obj);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public override string ToString() => _clock_time.ToString(Constants.VariableLengthMicrosIsoFormatWithoutZ, CultureInfo.InvariantCulture);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="str"></param>
        /// <param name="clockTime"></param>
        /// <param name="disallowOffset">If true, require that offset is absent. If false (default), offset (if any) is silently ignored.</param>
        /// <returns></returns>
        public static bool TryParse(string str, out ClockTime clockTime, bool disallowOffset = false)
		{
			clockTime = default;

			if (IsoTimeParser.TryParseAsIso(str, out DateTimeOffset dto, out var offsetKind))
			{
                if (!disallowOffset || offsetKind == OffsetKind.None)
				{
					clockTime = ToClockTime(dto.DateTime);
					return true;
				}
			}

			return false;
		}

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="str"></param>
        /// <param name="disallowOffset">If true, require that offset is absent. If false (default), offset (if any) is silently ignored.</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static ClockTime Parse(string str, bool disallowOffset = false)
		{
			if (TryParse(str, out var ct, disallowOffset))
				return ct;
			else
				throw new FormatException();
		}
	}

}
