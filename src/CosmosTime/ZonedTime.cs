using CosmosTime.TimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CosmosTime
{
	public struct ZonedTime : IEquatable<ZonedTime>, IComparable<ZonedTime>, IComparable
	{
		OffsetTime _offset_time;
		TimeZoneInfo _tz;

		public OffsetTime OffsetTime => _offset_time;
		public TimeZoneInfo Zone => _tz;



		/// <summary>
		/// DateTime must be Kind.Utc, else will throw
		/// TODO: why not allow Local?? Does now.
		/// </summary>
		/// <param name="utcTime"></param>
		public ZonedTime(DateTime utcOrLocalTime)
		{
			if (utcOrLocalTime.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("unspecified kind not allowed");

			// Since Kind now is either Utc or Local, its easy
			if (utcOrLocalTime.Kind == DateTimeKind.Local)
			{
				// can Local tz be Utc? Yes. Should we in this case change Kind of _zoned to Utc? Maybe...
				var tz = TimeZoneInfo.Local;
				Init(new OffsetTime(utcOrLocalTime.ToUtcTime(), tz.GetUtcOffset(utcOrLocalTime)), tz);
			}
			else if (utcOrLocalTime.Kind == DateTimeKind.Utc)
			{
				var tz = TimeZoneInfo.Utc;
				Init(new OffsetTime(utcOrLocalTime.ToUtcTime(), tz.GetUtcOffset(utcOrLocalTime)), tz);
			}
			else
				throw new Exception("impossible, still unspec");
		}


		public long Ticks => _offset_time.Ticks;



		public ZonedTime(OffsetTime time, TimeZoneInfo tz)
		{
			Init(time, tz);
		}

		private void Init(OffsetTime offset_time, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));

			if (tz.IsInvalidTime(offset_time.UnspecifiedDateTime))
				throw new ArgumentException("invalid time (time does not exist in tz)");

			// trigger validation that IanaId exists
			var ianaIdDummy = IanaTimeZone.GetIanaId(tz);

			(var ok, var msg) = Shared.ValidateOffset(tz, offset_time.UnspecifiedDateTime, offset_time.OffsetMinutes);
			if (!ok)
				throw new ArgumentException(msg);

			_offset_time = offset_time;
			_tz = tz;
		}

		/// <summary>
		/// Will capture Utc time + local offset to utc
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// Same as Now(TimeZoneInfo.Local)
		/// </summary>
		public static ZonedTime LocalNow => Now(TimeZoneInfo.Local);

		/// <summary>
		/// An UtcOffsetTime with utc time without offset (no time zone info)
		/// Same as Now(TimeZoneInfo.Utc)
		/// </summary>
		public static ZonedTime UtcNow => Now(TimeZoneInfo.Utc);

		/// <summary>
		/// Get Now in a zone (time will always be utc, but the offset captured will depend on the tz, and if ambigous, one will be chosen for you (standard time))
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static ZonedTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			//			var zoned = ZonedTime.Now(tz);
			//		var off = tz.GetUtcOffset(zoned.ZonedDateTime);

			var offTime = OffsetTime.Now(tz);

			// TODO: skip validation?
			return new ZonedTime(offTime, tz);
		}


		/// <summary>
		/// Can specift offset here to choose correct offset in case of ambigous time.
		/// 
		/// TODO: could have had a callback...so only need to specify offset if ambigous?
		/// </summary>
		public ZonedTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeZoneInfo tz, TimeSpan offset) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			Init(new OffsetTime(dt.ToUtcTime(tz, offset), offset), tz);
		}

		public ZonedTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz)
			: this(year, month, day, hour, minute, second, 0, tz)
		{
		}

		public ZonedTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz, TimeSpan offset)
			: this(year, month, day, hour, minute, second, 0, tz, offset)
		{
		}

		/// <summary>
		/// Uses default offset (standard time offset) in case of ambigous time.
		/// </summary>
		public ZonedTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeZoneInfo tz) : this()
		{
			if (tz == null)
				throw new ArgumentNullException(nameof(tz));
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
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
		public static bool TryParse(string time, out ZonedTime zoned)
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
				var offsetMinutes = Shared.GetWholeMinutes(tz.GetUtcOffset(dto.DateTime));

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?
				zoned = new ZonedTime(new OffsetTime(new UtcTime(dto.DateTime, tz), offsetMinutes), tz);
			}
			else
			{
				if (!Shared.ValidateOffset(tz, dto.DateTime, Shared.GetWholeMinutes(dto.Offset)).Ok)
					return false;

				// TODO: ctor also validate offset. Optimize?
				// TODO: ctor also validate iana. Optimize?
				zoned = new ZonedTime(new OffsetTime(new UtcTime(dto.DateTime, tz), Shared.GetWholeMinutes(dto.Offset)), tz);
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

		public override bool Equals(object obj) => obj is ZonedTime other && Equals(other);


		//private DateTime ClockDateTime_KindUtc => _utc.UtcDateTime.AddMinutes(_offsetMins);// _utc.AddMinutes(_offsetMins);
//		private DateTime ClockDateTime_KindUnspecified => DateTime.SpecifyKind(_offset_time.UtcTime.UtcDateTime.AddMinutes(_offset_time.OffsetMinutes), DateTimeKind.Unspecified);// _utc.AddMinutes(_offsetMins);

		/// <summary>
		/// Variable length local[+-]offset
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{_offset_time.ToString(_tz == TimeZoneInfo.Utc)}[{IanaTimeZone.GetIanaId(_tz)}]";
		}

		/// <summary>
		/// Equal if the Utc time is equal.
		/// The offset is ignored, it is only used to make local times.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ZonedTime other) => this._offset_time == other._offset_time;

		public int CompareTo(ZonedTime other) => this._offset_time.CompareTo(other._offset_time);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ZonedTime)obj);
		}

		public static ZonedTime operator +(ZonedTime d, TimeSpan t) => d.AddOrRemove(t, true);
		public static ZonedTime operator -(ZonedTime d, TimeSpan t) => d.AddOrRemove(t, false);

		private ZonedTime AddOrRemove(TimeSpan adjustment, bool add)
		{
			if (add)
			{
				var adj = this._offset_time.UtcTime + adjustment;
				return new ZonedTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
			}
			else
			{
				var adj = this._offset_time.UtcTime - adjustment;
				return new ZonedTime(new OffsetTime(adj, _tz.GetUtcOffset(adj.UtcDateTime)), _tz);
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



		public static TimeSpan operator -(ZonedTime a, ZonedTime b) => a._offset_time - b._offset_time;

		public static bool operator ==(ZonedTime a, ZonedTime b) => a._offset_time == b._offset_time;
		public static bool operator !=(ZonedTime a, ZonedTime b) => a._offset_time != b._offset_time;
		public static bool operator <(ZonedTime a, ZonedTime b) => a._offset_time < b._offset_time;
		public static bool operator >(ZonedTime a, ZonedTime b) => a._offset_time > b._offset_time;
		public static bool operator <=(ZonedTime a, ZonedTime b) => a._offset_time <= b._offset_time;
		public static bool operator >=(ZonedTime a, ZonedTime b) => a._offset_time >= b._offset_time;
	}
}
