using CosmosTime.TimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Store utc + offset + tz
	/// If storing a clock time in the future and the tz-database changes, the clock time may be wrong since we calculate clock time from utc.
	/// tz-database changes: updated regularly, so for utc time in the future, we can't know for sure what the offset will be in a zone.
	/// I guess if we wanted a struct with focus on clock times, and where the utc\global time would change in the future, if tz-db changes,
	/// then would need eg. ClockZoneTime (clock_time + offset + tz)
	/// </summary>
	public struct ZoneTime : IEquatable<ZoneTime>, IComparable<ZoneTime>, IComparable
	{
		OffsetTime _offset_time;
		TimeZoneInfo _tz;

		public OffsetTime OffsetTime => _offset_time;

		public TimeZoneInfo Zone => _tz;

		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// TODO: why not allow Local?? Does now.
		/// </summary>
		/// <param name="utcOrLocalTime"></param>
		public ZoneTime(DateTime utcOrLocalTime)
		{
			TimeZoneInfo tz = Shared.GetTimeZoneFromKindUtcOrLocal(utcOrLocalTime);
			Init(new OffsetTime(utcOrLocalTime.ToUtcTime(), tz.GetUtcOffset(utcOrLocalTime)), tz);
		}

		public ZoneTime(DateTime anyTime, TimeZoneInfo tz)
		{
			Init(new OffsetTime(anyTime.ToUtcTime(tz), tz.GetUtcOffset(anyTime)), tz);
		}

		/// <summary>
		/// Zone time ticks
		/// </summary>
		public long Ticks => _offset_time.Ticks;

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
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeZoneInfo tz)
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			Init(dt.ToUtcTime(tz).ToOffsetTime(tz.GetUtcOffset(dt)), tz);
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// </summary>
		public ZoneTime(int year, int month, int day, TimeZoneInfo tz, TimeSpan offset)
			: this(year, month, day, 0, 0, 0, 0, tz, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz, TimeSpan offset)
			: this(year, month, day, hour, minute, second, 0, tz, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Zone time
		/// </summary>
		public ZoneTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeZoneInfo tz, TimeSpan offset)
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			Init(dt.ToUtcTime(tz, offset).ToOffsetTime(offset), tz);
		}

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
				throw new ArgumentException($"Invalid time: '{offsetTime}' is invalid in '{GetIanaId()}'");

			(var ok, var msg) = Shared.ValidateOffset(tz, offsetTime.ClockDateTime, offsetTime.Offset);
			if (!ok)
				throw new ArgumentException(msg);

			_offset_time = offsetTime;
		}

		private string GetIanaId()
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

			var offTime = OffsetTime.Now(tz);

			// TODO: skip validation?
			return new ZoneTime(offTime, tz);
		}


		/// <summary>
		/// Can specift offset here to choose correct offset in case of ambigous time.
		/// 
		/// TODO: could have had a callback...so only need to specify offset if ambigous?
		/// </summary>
		public ZoneTime(ClockTime clockTime, TimeZoneInfo tz, TimeSpan offset) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = clockTime.ClockDateTime;
			Init(new OffsetTime(dt.ToUtcTime(tz, offset), offset), tz);
		}

		/// <summary>
		/// Uses default offset (standard time offset) in case of ambigous time.
		/// </summary>
		public ZoneTime(ClockTime clockTime, TimeZoneInfo tz) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = clockTime.ClockDateTime;
			Init(new OffsetTime(dt.ToUtcTime(tz), tz.GetUtcOffset(dt)), tz);
		}


		/// <summary>
		/// Supported directly:
		/// "{time}Z[{tz}]" -> "{time}-00:00"
		/// "{time}+|-{offset}[{tz}]" -> "{time}[{tz}]"
		/// "{time}[{tz}]" -> "{time}[{tz}]"
		/// 
		/// TODO: can support more by supplying callbacks
		/// 
		/// </summary>
		public static bool TryParse(string time, out ZoneTime zoned)
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

			if (!IsoTimeParser.TryParseAsIso(timePart, out DateTimeOffset dto, out TimeZoneKind tzk))
				return false;

			// dto.DateTime: ClockTime, Kind unspecified (even for Utc)

			if (tzk == TimeZoneKind.None)
			{
				// use default offset
				var offset = tz.GetUtcOffset(dto.DateTime);

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?
				zoned = new ZoneTime(new OffsetTime(new UtcTime(dto.DateTime, tz), offset), tz);
			}
			else
			{
				if (!Shared.ValidateOffset(tz, dto.DateTime, dto.Offset).Ok)
					return false;

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?
				zoned = new ZoneTime(new OffsetTime(new UtcTime(dto.DateTime, tz), dto.Offset), tz);
			}

			return true;
		}


//		public static readonly UtcOffsetZoneTime MinValue = DateTimeOffset.MinValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MinValue, 0);
	//	public static readonly UtcOffsetZoneTime MaxValue = DateTimeOffset.MaxValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

//		public UtcTime UtcTime => _utc;

		/// <summary>
		/// Offset from Utc
		/// </summary>


		public override int GetHashCode() => _offset_time.GetHashCode();

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
			return $"{_offset_time.ToString(_tz == TimeZoneInfo.Utc)}[{GetIanaId()}]";
		}

		/// <summary>
		/// Equal if the Utc time is equal.
		/// The offset is ignored, it is only used to make local times.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ZoneTime other) => this._offset_time == other._offset_time;

		public int CompareTo(ZoneTime other) => this._offset_time.CompareTo(other._offset_time);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ZoneTime)obj);
		}

		public static ZoneTime operator +(ZoneTime d, TimeSpan t)
		{
			var adj = d._offset_time.UtcTime + t;
			return new ZoneTime(new OffsetTime(adj, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
		}
		public static ZoneTime operator -(ZoneTime d, TimeSpan t)
		{
			var adj = d._offset_time.UtcTime - t;
			return new ZoneTime(new OffsetTime(adj, d._tz.GetUtcOffset(adj.UtcDateTime)), d._tz);
		}

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



		public static TimeSpan operator -(ZoneTime a, ZoneTime b) => a._offset_time - b._offset_time;

		public static bool operator ==(ZoneTime a, ZoneTime b) => a._offset_time == b._offset_time;
		public static bool operator !=(ZoneTime a, ZoneTime b) => a._offset_time != b._offset_time;
		public static bool operator <(ZoneTime a, ZoneTime b) => a._offset_time < b._offset_time;
		public static bool operator >(ZoneTime a, ZoneTime b) => a._offset_time > b._offset_time;
		public static bool operator <=(ZoneTime a, ZoneTime b) => a._offset_time <= b._offset_time;
		public static bool operator >=(ZoneTime a, ZoneTime b) => a._offset_time >= b._offset_time;

		public ZoneTime AddSeconds(double sec) => this + TimeSpan.FromSeconds(sec);
		public ZoneTime AddMinutes(double min) => this + TimeSpan.FromMinutes(min);
		public ZoneTime AddHours(double h) => this + TimeSpan.FromHours(h);

		// Adding days may not always work, DST will make some days more or less than 24h.
		// You can still add 24 hours, but then it may be clearer that you are not adding days.
		//public ZonedTime AddDays(double days) => this + TimeSpan.FromDays(days);



		/// <summary>
		/// Not sure if having these it a good design. Possibly should require converting to ClockTime, performing operations there, then converting back...
		/// ClockTime would wrap a DateTime kind unspeficied.
		/// maybe could manipulate via using?
		/// 
		/// using (var ct = zoned.AdjustClockTime())
		/// {
		///		
		///		
		/// }
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
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
