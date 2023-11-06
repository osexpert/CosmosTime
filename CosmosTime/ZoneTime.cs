using CosmosTime.TimeZone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Store utc + offset + tz
	/// 
	/// If storing a clock time in the future and the tz-database changes, the clock time may be wrong since we calculate clock time from utc.
	/// tz-database changes: updated regularly, so for utc time in the future, we can't know for sure what the offset will be in a zone.
	/// I guess if we wanted a struct with focus on clock times, and where the utc\global time would change in the future, if tz-db changes,
	/// then would need eg. ClockZoneTime (clock_time + offset + tz)
	/// </summary>
	[TypeConverter(typeof(ZoneTimeTypeConverter))]
	public struct ZoneTime : IEquatable<ZoneTime>, IComparable<ZoneTime>, IComparable
	{
		OffsetTime _offset_time;
		TimeZoneInfo _tz;

		/// <summary>
		/// TODO
		/// </summary>
		public OffsetTime OffsetTime => _offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		public TimeZoneInfo Zone => _tz;

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// </summary>
		public static ZoneTime FromUtcDateTime(DateTime utcTime)
		{
			TimeZoneInfo tz = TimeZoneInfo.Utc;
			return new ZoneTime(new OffsetTime(UtcTime.FromUtcDateTime(utcTime), tz.GetUtcOffset(utcTime)), tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="localTime"></param>
		/// <returns></returns>
		public static ZoneTime FromLocalDateTime(DateTime localTime)
		{
			TimeZoneInfo tz = TimeZoneInfo.Local;
			return new ZoneTime(new OffsetTime(UtcTime.FromLocalDateTime(localTime), tz.GetUtcOffset(localTime)), tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="unspecifiedTime"></param>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static ZoneTime FromUnspecifiedDateTime(DateTime unspecifiedTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var offset = tz.GetUtcOffset(unspecifiedTime);
			return new ZoneTime(new OffsetTime(UtcTime.FromUnspecifiedDateTime(unspecifiedTime, offset), offset), tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="unspecifiedTime"></param>
		/// <param name="tz"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static ZoneTime FromUnspecifiedDateTime(DateTime unspecifiedTime, TimeZoneInfo tz, TimeSpan offset)
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			return new ZoneTime(new OffsetTime(UtcTime.FromUnspecifiedDateTime(unspecifiedTime, offset), offset), tz);
		}

		/// <summary>
		/// Ticks in Clock time
		/// </summary>
		public long Ticks => _offset_time.Ticks;

		/// <summary>
		/// TimeOfDay in Clock time
		/// </summary>
		public TimeOnly TimeOfDay => _offset_time.TimeOfDay;

		/// <summary>
		/// Date in Clock time
		/// </summary>
		public DateOnly Date => _offset_time.Date;


		/// <summary>
		/// year, month, day, etc. in Zone time
		/// </summary>
		public ZoneTime(int year, int month, int day, TimeZoneInfo tz)
			: this(year, month, day, 0, 0, 0, 0, tz)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz)
			: this(year, month, day, hour, minute, second, 0, tz)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeZoneInfo tz)
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
			var offset = tz.GetUtcOffset(dt);
			Init(UtcTime.FromUnspecifiedDateTime(dt, offset).ToOffsetTime(offset), tz);
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// Both tz and offset? Yes, in case you want to choose offset if ambigous time (or you simply know it up front)
		/// </summary>
		public ZoneTime(int year, int month, int day, TimeZoneInfo tz, TimeSpan offset)
			: this(year, month, day, 0, 0, 0, 0, tz, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// Both tz and offset? Yes, in case you want to choose offset if ambigous time (or you simply know it up front)
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz, TimeSpan offset)
			: this(year, month, day, hour, minute, second, 0, tz, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// Both tz and offset? Yes, in case you want to choose offset if ambigous time (or you simply know it up front)
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeZoneInfo tz, TimeSpan offset)
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
			Init(UtcTime.FromUnspecifiedDateTime(dt, offset).ToOffsetTime(offset), tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="time"></param>
		/// <param name="tz"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ZoneTime(UtcTime time, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			Init(time.ToOffsetTime(tz.GetUtcOffset(time.UtcDateTime)), tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="time"></param>
		/// <param name="tz"></param>
		public ZoneTime(OffsetTime time, TimeZoneInfo tz)
		{
			Init(time, tz);
		}

		private void Init(OffsetTime offsetTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			_tz = tz;

			// trigger validation that IanaId exists
			//var ianaIdDummy = IanaTimeZone.GetIanaId(tz);

			if (tz.IsInvalidTime(offsetTime.ClockDateTime))
				throw new ArgumentException($"Invalid time: '{offsetTime}' is invalid in '{GetIanaTzId()}'");

			(var ok, var msg) = Shared.ValidateOffset(tz, offsetTime.ClockDateTime, offsetTime.Offset);
			if (!ok)
				throw new ArgumentException(msg);

			_offset_time = offsetTime;
		}

		private string GetIanaTzId()
		{
			if (IanaTimeZone.TryGetIanaId(_tz, out var ianaId))
				return ianaId;
			else // crazy hack?
				return $"Windows/{_tz.Id}";
		}

		/// <summary>
		/// Will capture Utc time + local offset to utc
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// Same as Now(TimeZoneInfo.Local)
		/// </summary>
		public static ZoneTime LocalNow => Now(TimeZoneInfo.Local);

		/// <summary>
		/// An UtcOffsetTime with utc time without offset (no time zone info)
		/// Same as Now(TimeZoneInfo.Utc)
		/// </summary>
		public static ZoneTime UtcNow => Now(TimeZoneInfo.Utc);

		/// <summary>
		/// Get Now in a zone (time will always be utc, but the offset captured will depend on the tz, and if ambigous, one will be chosen for you (standard time))
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static ZoneTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			//			var zoned = ZonedTime.Now(tz);
			//		var off = tz.GetUtcOffset(zoned.ZonedDateTime);

			var offsetTime = OffsetTime.Now(tz);

			// TODO: skip validation?
			return new ZoneTime(offsetTime, tz);
		}


		/// <summary>
		/// Can specify offset here to choose correct offset in case of ambigous time.
		/// 
		/// TODO: could have had a callback...so only need to specify offset if ambigous?
		/// </summary>
		public ZoneTime(ClockTime clockTime, TimeZoneInfo tz, TimeSpan offset) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = clockTime.ClockDateTime;
			Init(new OffsetTime(UtcTime.FromUnspecifiedDateTime(dt, offset), offset), tz);
		}

		/// <summary>
		/// Uses default offset (standard time offset) in case of ambigous time.
		/// </summary>
		public ZoneTime(ClockTime clockTime, TimeZoneInfo tz) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = clockTime.ClockDateTime;
			var offset = tz.GetUtcOffset(dt);
			Init(new OffsetTime(UtcTime.FromUnspecifiedDateTime(dt, offset), offset), tz);
		}


		/// <summary>
		/// Supported directly:
		/// <para>{time}Z[{tz}] -> {time}-00:00</para>
		/// <para>{time}+|-{offset}[{tz}]" -> {time}[{tz}]</para>
		/// <para>{time}[{tz}] -> {time}[{tz}]</para>
		/// 
		/// TODO: can support more by supplying callbacks
		/// TODO: what if we do not want to choose in chooseOffsetIfAmbigous? Now we need to throw? Could we return a touple (TimeSpan, bool)? Or TimeSpan? (nullable?)
		/// </summary>
		public static bool TryParse(string time, out ZoneTime zoned, Func<TimeSpan[], TimeSpan> chooseOffsetIfAmbigous = null)
		{
			zoned = default;

			if (time.Last() != ']')
				return false;

			var i = time.IndexOf('[');
			if (i == -1)
				return false;

			var timePart = time.Substring(0, i);
			var tzPart = time.Substring(i + 1, time.Length - i - 2);

			// TODO: add option to allow specify tz via callback?
			if (!IanaTimeZone.TryGetTimeZoneInfo(tzPart, out TimeZoneInfo tz))
				return false;

			if (!IsoTimeParser.TryParseAsIso(timePart, out DateTimeOffset dto, out OffsetKind offsetKind))
				return false;

			// dto.DateTime: ClockTime, Kind unspecified (even for Utc)

			if (offsetKind == OffsetKind.None)
			{
				TimeSpan offset;
				if (chooseOffsetIfAmbigous == null)
				{
					// use default tz offset
					offset = tz.GetUtcOffset(dto);
				}
				else
				{
					offset = tz.GetUtcOffset(dto, chooseOffsetIfAmbigous);
				}

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?


				zoned = new ZoneTime(new OffsetTime(UtcTime.FromUnspecifiedDateTime(dto.DateTime, offset), offset), tz);
			}
			else
			{
				if (!Shared.ValidateOffset(tz, dto.DateTime, dto.Offset).Ok)
					return false;

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?
				zoned = new ZoneTime(new OffsetTime(UtcTime.FromUnspecifiedDateTime(dto.DateTime, dto.Offset), dto.Offset), tz);
			}

			return true;
		}


		//		public static readonly UtcOffsetZoneTime MinValue = DateTimeOffset.MinValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MinValue, 0);
		//	public static readonly UtcOffsetZoneTime MaxValue = DateTimeOffset.MaxValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

		//		public UtcTime UtcTime => _utc;



		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => _offset_time.GetHashCode();

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) => obj is ZoneTime other && Equals(other);


		//private DateTime ClockDateTime_KindUtc => _utc.UtcDateTime.AddMinutes(_offsetMins);// _utc.AddMinutes(_offsetMins);
		//		private DateTime ClockDateTime_KindUnspecified => DateTime.SpecifyKind(_offset_time.UtcTime.UtcDateTime.AddMinutes(_offset_time.OffsetMinutes), DateTimeKind.Unspecified);// _utc.AddMinutes(_offsetMins);

		/// <summary>
		/// Iso format: 
		/// {time}+|-{offset}[{iana}]
		/// {utc_time}Z[{iana}]
		/// Examples:
		/// 2020-01-20T04:05:06.007+01:00[Europe/Berlin]
		/// 2020-01-20T04:05:06.007Z[Etc/UTC]
		/// Time is variable length (milliseconds)
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{_offset_time.ToString(_tz.IsUtc())}[{GetIanaTzId()}]";
		}

		/// <summary>
		/// Equal if the Utc time is equal.
		/// The offset is ignored, it is only used to make local times.
		/// The tz is ignored, Utc is valid on its own
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ZoneTime other) => this._offset_time.Equals(other._offset_time);


		/// <summary>
		/// Also check that offset and zone is the same
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool EqualsExact(ZoneTime other)
		{
			return _offset_time.EqualsExact(other._offset_time) && _tz == other._tz;
		}


		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ZoneTime other) => this._offset_time.CompareTo(other._offset_time);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ZoneTime)obj);
		}

		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public ZoneTime AddUtc(TimeSpan t)
		{
			var adj = _offset_time.UtcTime + t;
			//return new ZoneTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			return adj.ToZoneTime(_tz);
		}
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public ZoneTime AddClock(TimeSpan t)
		{
			var adj = _offset_time.ClockDateTime + t;
			//return new ZoneTime(new OffsetTime(adj.to, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
			return ZoneTime.FromUnspecifiedDateTime(adj, _tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public ZoneTime SubtractUtc(TimeSpan t)
		{
			var adj = _offset_time.UtcTime - t;
			//return new ZoneTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			return adj.ToZoneTime(_tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public ZoneTime SubtractClock(TimeSpan t)
		{
			var adj = _offset_time.ClockDateTime - t;
			//return new ZoneTime(new OffsetTime(adj.to, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
			return ZoneTime.FromUnspecifiedDateTime(adj, _tz);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public TimeSpan SubtractUtc(ZoneTime t)
		{
			//var adj = _offset_time.UtcTime - t;
			//return new ZoneTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			//return adj.ToZoneTime(_tz);
			return _offset_time - t._offset_time;
		}
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public TimeSpan SubtractClock(ZoneTime t)
		{
			//var adj = _offset_time.ClockDateTime - t;
			//return new ZoneTime(new OffsetTime(adj.to, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
			//return adj.ToZoneTime(_tz);
			return _offset_time.ClockDateTime - t._offset_time.ClockDateTime;
		}

		//public static ZoneTime operator +(ZoneTime d, TimeSpan t)
		//{
		//	var adj = d._offset_time.UtcTime + t;
		//	return new ZoneTime(new OffsetTime(adj, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
		//}
		//public static ZoneTime operator -(ZoneTime d, TimeSpan t)
		//{
		//	var adj = d._offset_time.UtcTime - t;
		//	return new ZoneTime(new OffsetTime(adj, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
		//}

#if false
		private ZoneTime Adjust(TimeSpan adjustment, bool add)
		{
			if (add)
			{
				var adj = this._offset_time.UtcTime + adjustment;
				return new ZoneTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			}
			else
			{
				var adj = this._offset_time.UtcTime - adjustment;
				return new ZoneTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			}

			// WHAT use could this have???
#if false
			var offsetBefore = _offset_time.Offset;
			var newOffsetTime = add ? _offset_time + adjustment : _offset_time - adjustment;
			var offsets = Shared.GetUtcOffsets(_tz, newOffsetTime.IsoDateTime);
			var offsetAfter = add ? offsets.First() : offsets.Last();
			var offsetDiff = offsetAfter - offsetBefore;

			if (offsetDiff != TimeSpan.Zero)
			{
				var utcTime = (newOffsetTime.IsoDateTime + offsetDiff).ToUtcTime(_tz, offsetAfter);
				return new ZonedTime(new OffsetTime(utcTime, offsetAfter), _tz);
			}
			else
			{
				return new ZonedTime(newOffsetTime, _tz);
			}
#endif
		}

#endif

		//public static TimeSpan operator -(ZoneTime a, ZoneTime b) => a._offset_time - b._offset_time;

		// Equality and ordering is always in Utc

		/// <summary>
		/// Equality and ordering is always in Utc
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator ==(ZoneTime t1, ZoneTime t2) => t1._offset_time == t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator !=(ZoneTime t1, ZoneTime t2) => t1._offset_time != t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator <(ZoneTime t1, ZoneTime t2) => t1._offset_time < t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator >(ZoneTime t1, ZoneTime t2) => t1._offset_time > t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator <=(ZoneTime t1, ZoneTime t2) => t1._offset_time <= t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static bool operator >=(ZoneTime t1, ZoneTime t2) => t1._offset_time >= t2._offset_time;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		public ZoneTime AddUtcTicks(long ticks) => AddUtc(TimeSpan.FromTicks(ticks));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public ZoneTime AddUtcSeconds(double seconds) => AddUtc(TimeSpan.FromSeconds(seconds));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public ZoneTime AddUtcMinutes(double minutes) => AddUtc(TimeSpan.FromMinutes(minutes));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public ZoneTime AddUtcHours(double hours) => AddUtc(TimeSpan.FromHours(hours));
		// Adding days may not always work, DST will make some days more or less than 24h.
		// You can still add 24 hours, but then it may be clearer that you are not adding days.
		//		public ZoneTime AddUtcDays(double days) => AddUtc(TimeSpan.FromDays(days));


		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		public ZoneTime AddClockTicks(long ticks) => AddClock(TimeSpan.FromTicks(ticks));


		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public ZoneTime AddClockSeconds(double seconds) => AddClock(TimeSpan.FromSeconds(seconds));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public ZoneTime AddClockMinutes(double minutes) => AddClock(TimeSpan.FromMinutes(minutes));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public ZoneTime AddClockHours(double hours) => AddClock(TimeSpan.FromHours(hours));

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="days"></param>
		/// <returns></returns>
		public ZoneTime AddClockDays(double days) => AddClock(TimeSpan.FromDays(days));

		/// <summary>
		/// Parse fixed length utc time
		/// </summary>
		/// <param name="utcTime"></param>
		/// <param name="offset"></param>
		/// <param name="ianaId"></param>
		/// <returns></returns>
		public static ZoneTime ParseCosmosDb(string utcTime, TimeSpan offset, string ianaId)
		{
			return new(OffsetTime.ParseCosmosDb(utcTime, offset), IanaTimeZone.GetTimeZoneInfo(ianaId));
		}




		// <summary>
		// Not sure if having these it a good design. Possibly should require converting to ClockTime, performing operations there, then converting back...
		// ClockTime would wrap a DateTime kind unspeficied.
		// maybe could manipulate via using?
		// 
		// using (var ct = zoned.AdjustClockTime())
		// {
		//		
		//		
		// }
		// </summary>
		// <param name="h"></param>
		// <returns></returns>
		//public ZonedTime AddClockHours(double h) => AddClock(TimeSpan.FromHours(h));


		//public ZonedTime AddClockDays(double days) => AddClock(TimeSpan.FromDays(days));

		///// <summary>
		///// Could it be a mode?
		///// </summary>
		//private ZonedTime AddClock(TimeSpan t)
		//{
		//	var adj = _offset_time.ClockDateTime + t;
		//	return adj.ToUtcZoneTime(_tz);
		//}


		//public ZonedTime AdjustClockTime(Func<ClockTime, ClockTime> ct)
		//{
		//	ct()
		//}
	}

}
