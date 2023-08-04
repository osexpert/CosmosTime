using CosmosTime.TimeZone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Store utc + offset
	/// </summary>
	[TypeConverter(typeof(OffsetTimeTypeConverter))]
	public struct OffsetTime : IEquatable<OffsetTime>, IComparable<OffsetTime>, IComparable
	{
		UtcTime _utc;
		short _offsetMinutes;

		/// <summary>
		/// Will capture Utc time + local offset to utc
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// Same as Now(TimeZoneInfo.Local)
		/// </summary>
		public static OffsetTime LocalNow => Now(TimeZoneInfo.Local);

		/// <summary>
		/// An UtcOffsetTime with utc time without offset (no time zone info)
		/// Same as Now(TimeZoneInfo.Utc)
		/// </summary>
		public static OffsetTime UtcNow => Now(TimeZoneInfo.Utc);

		/// <summary>
		/// Get Now in a zone (time will always be utc, but the offset captured will depend on the tz)
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static OffsetTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			if (tz == TimeZoneInfo.Local)
				return DateTimeOffset.Now.ToOffsetTime();
			else if (tz.IsUtc())
				return DateTimeOffset.UtcNow.ToOffsetTime();
			else // convert to time in the zone
			{
				var utcNow = DateTime.UtcNow;
				var dtInTz = TimeZoneInfo.ConvertTime(utcNow, tz);
				var offset = (dtInTz - utcNow); // same as tz.GetUtcOffset(dtInTz), I think.
				return OffsetTime.FromUtcDateTime(utcNow, offset);
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="utcTime"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static OffsetTime FromUtcDateTime(DateTime utcTime, TimeSpan offset)
		{
			var ot = new OffsetTime();
			ot.Init(UtcTime.FromUtcDateTime(utcTime), offset);
			return ot;
		}
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="localTime"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static OffsetTime FromLocalDateTime(DateTime localTime, TimeSpan offset)
		{
			var ot = new OffsetTime();
			ot.Init(UtcTime.FromLocalDateTime(localTime), offset);
			return ot;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="unspecifiedTime"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static OffsetTime FromUnspecifiedDateTime(DateTime unspecifiedTime, TimeSpan offset)
		{
			var ot = new OffsetTime();
			ot.Init(UtcTime.FromUnspecifiedDateTime(unspecifiedTime, offset), offset);
			return ot;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static readonly OffsetTime MinValue = DateTimeOffset.MinValue.ToOffsetTime();// new OffsetTime(UtcTime.MinValue, 0);
		/// <summary>
		/// TODO
		/// </summary>
		public static readonly OffsetTime MaxValue = DateTimeOffset.MaxValue.ToOffsetTime();// new OffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

		/// <summary>
		/// TODO
		/// </summary>
		public UtcTime UtcTime => _utc;

		/// <summary>
		/// Offset from Utc
		/// </summary>
		public TimeSpan Offset => TimeSpan.FromMinutes(_offsetMinutes);

		/// <summary>
		/// Ticks in clock time
		/// If you need Tick in Utc, use UtcTime.Ticks
		/// </summary>
		public long Ticks => ClockDateTime_KindUnspecified.Ticks;

		//		public long UtcTicks => _utc.Ticks;

		/// <summary>
		/// clock time's TimeOfDay
		/// </summary>
		public TimeOnly TimeOfDay => TimeOnly.FromDateTime(ClockDateTime_KindUnspecified);

		/// <summary>
		///  clock time's Date
		/// </summary>
		public DateOnly Date => DateOnly.FromDateTime(ClockDateTime_KindUnspecified);

		/// <summary>
		/// Add ticks
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		public OffsetTime AddTicks(long ticks) => new OffsetTime(Ticks + ticks, Offset);

		// <summary>
		// The specified time is clock time (not utc)
		// </summary>
		//public UtcOffsetTime(ClockTime ct, Func<TimeSpan[], TimeSpan> choseOffsetIfAmbigious)
		//{
		//	throw new NotImplementedException();
		//}


		/// <summary>
		/// ticks in Clock time
		/// </summary>
		public OffsetTime(long ticks, TimeSpan offset)
		{
			var dt = new DateTime(ticks, DateTimeKind.Unspecified);
			Init(UtcTime.FromUnspecifiedDateTime(dt, offset), offset);
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// </summary>
		public OffsetTime(int year, int month, int day, TimeSpan offset)
			: this(year, month, day, 0, 0, 0, 0, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// </summary>
		public OffsetTime(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
			: this(year, month, day, hour, minute, second, 0, offset)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// </summary>
		public OffsetTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeSpan offset)
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			Init(UtcTime.FromUnspecifiedDateTime(dt, offset), offset);
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// Will pick the offset from the tz
		/// </summary>
		public OffsetTime(int year, int month, int day, TimeZoneInfo tz)
			: this(year, month, day, 0, 0, 0, 0, tz)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// Will pick the offset from the tz
		/// </summary>
		public OffsetTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz)
			: this(year, month, day, hour, minute, second, 0, tz)
		{
		}

		/// <summary>
		/// year, month, day, etc. in Clock time
		/// Will pick the offset from the tz
		/// </summary>
		public OffsetTime(int year, int month, int day, int hour, int minute, int second, int millis, TimeZoneInfo tz) 
		{
			var dt = new DateTime(year, month, day, hour, minute, second, millis, DateTimeKind.Unspecified);
			var offset = tz.GetUtcOffset(dt);
			Init(UtcTime.FromUnspecifiedDateTime(dt, offset), offset);
		}


		/// <summary>
		/// Parse ISO formats:
		/// {time}Z
		/// {time}+|-{offset}
		/// </summary>
		public static OffsetTime Parse(string str)
		{
			if (TryParse(str, out var ut))
				return ut;
			throw new FormatException();
		}

		/// <summary>
		/// Parse ISO formats:
		/// "{time}Z"
		/// "{time}{offset}"
		/// </summary>
		public static bool TryParse(string utcOffsetString, out OffsetTime uo)
		{
			uo = default;

			if (IsoTimeParser.TryParseAsIso(utcOffsetString, out DateTimeOffset dto, out var offsetKind) && offsetKind != OffsetKind.None)
			{
				uo = dto.ToOffsetTime();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Parse ISO formats:
		/// "{time}Z"
		/// "{time}{offset}"
		/// "{time}" (tzIfUnspecified will be called)
		/// </summary>
		public static bool TryParse(string utcOffsetString, out OffsetTime uo, Func<DateTimeOffset, TimeSpan> getOffsetOfNone)
		{
			if (getOffsetOfNone == null)
				throw new ArgumentNullException(nameof(getOffsetOfNone));

			uo = default;

			if (IsoTimeParser.TryParseAsIso(utcOffsetString, out DateTimeOffset dto, out var offsetKind))
			{
				if (offsetKind == OffsetKind.None)
				{
					var offset = getOffsetOfNone(dto);
					var utc = UtcTime.FromUnspecifiedDateTime(dto.DateTime, offset);
//					var offset = (dto.DateTime - utc.UtcDateTime);
					uo = new OffsetTime(utc, offset);
					return true;
				}
				else
				{
					uo = dto.ToOffsetTime();
					return true;
				}
			}

			return false;
		}


		// <summary>
		//
		// </summary>
		//public UtcOffsetTime(DateTimeOffset dto)
		//{
		//	// what about dto.ToUniversalTime? versus  dto.UtcDateTime ???
		//	//dto.ToUniversalTime sets offset to 0. But they are still equal!!
		//	// Yes, but so are WE. We only compare _utc too.

		//	//_dto = dto;
		//	_utc = dto.UtcDateTime.ToUtcTime();
		//	// TODO: create some tests to make sure this roundtrips
		//	_offsetMinutes = Shared.GetWholeMinutes(dto.Offset);
		//}

		/// <summary>
		/// offsetMinutes: utc+offsetMinutes=local
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public OffsetTime(UtcTime utc, TimeSpan offset)
		{
			Init(utc, offset);
		}

		private void Init(UtcTime utc, TimeSpan offset)
		{
			_offsetMinutes = Shared.ValidateOffset(offset);
			_utc = utc;
		}

		//private DateTime ClockDateTime_KindUtc => _utc.UtcDateTime.AddMinutes(_offsetMins);// _utc.AddMinutes(_offsetMins);

		// name: OffsettedUtcDateTime, OffsetDateTime, OffsettedUtcDateTime, UtcOffsetDateTime, AdjustedDateTime, BaseDateTime, IsoDateTime

		internal DateTime ClockDateTime_KindUnspecified => DateTime.SpecifyKind(_utc.UtcDateTime.AddMinutes(_offsetMinutes), DateTimeKind.Unspecified);// _utc.AddMinutes(_offsetMins);

		/// <summary>
		/// UtcTime + Offset = Clock Time
		/// Unspecified Kind.
		/// </summary>
		public DateTime ClockDateTime => ClockDateTime_KindUnspecified;

		/// <summary>
		/// Utc Kind
		/// </summary>
		public DateTime UtcDateTime => _utc.UtcDateTime;

		/// <summary>
		/// Iso format.
		/// {local}+|-{offset}
		/// </summary>
		/// <returns></returns>
		public override string ToString() => ToString(false);

		internal string ToString(bool tzIsUtc)
		{
			var local = ClockDateTime_KindUnspecified;

			//int seconds = 10000; //or whatever time you have
			//string.Format("{0:00}':'{1:00}", seconds / 3600, (seconds / 60) % 60);
			var mins = _offsetMinutes;
			bool neg = mins < 0;
			if (neg)
				mins *= -1;

			var strNoZ = local.ToString(Constants.VariableLengthMicrosIsoFormatWithoutZ);

			// Do not use Z here because we do not know it is Utc, only that offset is 0
	//		var off = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
//			var res = $"{strNoZ}{(neg ? '-' : '+')}{off}";

			var tzString = tzIsUtc ? "Z" : string.Format("{0}{1:00}:{2:00}", neg ? '-' : '+', mins / 60, mins % 60);
			var res = $"{strNoZ}{tzString}";

			return res;
		}

		/// <summary>
		/// Parse fixed length utc (28 chars, ends with Z)
		/// </summary>
		public static OffsetTime ParseCosmosDb(string utc, TimeSpan offset)
		{
			var utcs = UtcTime.ParseCosmosDb(utc);
			return new OffsetTime(utcs, offset);
		}

		/// <summary>
		/// Returns DateTimeOffset...
		/// </summary>
		public DateTimeOffset ToDateTimeOffset()
		{
			//return _dto;
			//var local = LocalDateTime_KindUtc;// _utc.UtcDateTime.AddMinutes(_offsetMins);

			// can not use Local kind as DTO will validate the offset against current local offset...
			// "DateTimeOffset Error: UTC offset of local dateTime does not match the offset argument"
			// KE?? but why not use the utc time directly?? It is not possible, must be unspecified time.
			return new DateTimeOffset(ClockDateTime_KindUnspecified, Offset);
		}

		// TODO: remove this?
		//public DateTime ToLocalDateTime() => _utc.ToLocalDateTime();

		/// <inheritdoc/>
		public override int GetHashCode() => _utc.GetHashCode();

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is OffsetTime other && Equals(other);

		/// <summary>
		/// Equal if the Utc time is equal.
		/// So there can be two OffsetTime's with same Utc time but different Offsets, and they will be equal.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(OffsetTime other) => this._utc == other._utc;


		/// <summary>
		/// Also check that offset is the same
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool EqualsExact(OffsetTime other)
		{
			return _utc == other._utc && _offsetMinutes == other._offsetMinutes;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(OffsetTime other) => this._utc.CompareTo(other._utc);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((OffsetTime)obj);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public OffsetTime Add(TimeSpan t) => new OffsetTime(_utc + t, Offset);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static TimeSpan operator -(OffsetTime a, OffsetTime b) => a._utc - b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="d"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static OffsetTime operator +(OffsetTime d, TimeSpan t) => new OffsetTime(d._utc + t, d.Offset);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="d"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static OffsetTime operator -(OffsetTime d, TimeSpan t) => new OffsetTime(d._utc - t, d.Offset);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(OffsetTime a, OffsetTime b) => a._utc == b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(OffsetTime a, OffsetTime b) => a._utc != b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <(OffsetTime a, OffsetTime b) => a._utc < b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >(OffsetTime a, OffsetTime b) => a._utc > b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator <=(OffsetTime a, OffsetTime b) => a._utc <= b._utc;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator >=(OffsetTime a, OffsetTime b) => a._utc >= b._utc;

	}
}
