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
	[TypeConverter(typeof(UtcOffsetTimeTypeConverter))]
	public struct UtcOffsetTime : IEquatable<UtcOffsetTime>, IComparable<UtcOffsetTime>, IComparable
	{
		UtcTime _utc;
		short _offsetMinutes;

		/// <summary>
		/// Will capture Utc time + local offset to utc
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// Same as Now(TimeZoneInfo.Local)
		/// </summary>
		public static UtcOffsetTime LocalNow => Now(TimeZoneInfo.Local);

		/// <summary>
		/// An UtcOffsetTime with utc time without offset (no time zone info)
		/// Same as Now(TimeZoneInfo.Utc)
		/// </summary>
		public static UtcOffsetTime UtcNow => Now(TimeZoneInfo.Utc);

		/// <summary>
		/// Get Now in a zone (time will always be utc, but the offset captured will depend on the tz)
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static UtcOffsetTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			if (tz == TimeZoneInfo.Local)
				return DateTimeOffset.Now.ToUtcOffsetTime();
			else if (tz == TimeZoneInfo.Utc)
				return DateTimeOffset.UtcNow.ToUtcOffsetTime();
			else // convert to time in the zone
			{
				var utcNow = UtcTime.Now;
				var dtInTz = TimeZoneInfo.ConvertTime(utcNow.UtcDateTime, tz);
				var offset = (dtInTz - utcNow.UtcDateTime); // same as tz.GetUtcOffset(dtInTz), I think.
				return new UtcOffsetTime(utcNow, offset);
			}
		}


		public static readonly UtcOffsetTime MinValue = DateTimeOffset.MinValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MinValue, 0);
		public static readonly UtcOffsetTime MaxValue = DateTimeOffset.MaxValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

		public UtcTime UtcTime => _utc;

		/// <summary>
		/// Offset from Utc
		/// </summary>
		internal short OffsetMinutes => _offsetMinutes;

		public TimeSpan Offset => TimeSpan.FromMinutes(_offsetMinutes);

		public long Ticks => ClockDateTime_KindUnspecified.Ticks;

		public long UtcTicks => _utc.Ticks;


		/// <summary>
		/// The specified time is clock time (not utc)
		/// </summary>
		public UtcOffsetTime(ClockTime ct, Func<TimeSpan[], TimeSpan> choseOffsetIfAmbigious)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// Parse ISO formats:
		/// {time}Z
		/// {time}+|-{offset}
		/// </summary>
		public static UtcOffsetTime Parse(string str)
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
		public static bool TryParse(string utcOffsetString, out UtcOffsetTime uo)
		{
			uo = default;

			if (IsoTimeParser.TryParseAsIso(utcOffsetString, out DateTimeOffset dto, out var tzk) && tzk != TimeZoneKind.None)
			{
				uo = dto.ToUtcOffsetTime();
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
		public static bool TryParse(string utcOffsetString, out UtcOffsetTime uo, Func<DateTimeOffset, TimeZoneInfo> tzIfUnspecified)
		{
			if (tzIfUnspecified == null)
				throw new ArgumentNullException(nameof(tzIfUnspecified));

			uo = default;

			if (IsoTimeParser.TryParseAsIso(utcOffsetString, out DateTimeOffset dto, out var tzk))
			{
				if (tzk == TimeZoneKind.None)
				{
					var tz = tzIfUnspecified(dto);
					var utc = dto.DateTime.ToUtcTime(tz);
					var offset = (dto.DateTime - utc.UtcDateTime);
					uo = new UtcOffsetTime(utc, offset);
					return true;
				}
				else
				{
					uo = dto.ToUtcOffsetTime();
					return true;
				}
			}

			return false;
		}


		/// <summary>
		///
		/// </summary>
		public UtcOffsetTime(DateTimeOffset dto)
		{
			// what about dto.ToUniversalTime? versus  dto.UtcDateTime ???
			//dto.ToUniversalTime sets offset to 0. But they are still equal!!
			// Yes, but so are WE. We only compare _utc too.

			//_dto = dto;
			_utc = dto.UtcDateTime.ToUtcTime();
			// TODO: create some tests to make sure this roundtrips
			_offsetMinutes = Shared.GetWholeMinutes(dto.Offset);
		}

		/// <summary>
		/// offsetMinutes: utc+offsetMinutes=local
		/// </summary>
		/// <param name="utcs"></param>
		/// <param name="offsetMinutes"></param>
		/// <exception cref="ArgumentException"></exception>
		public UtcOffsetTime(UtcTime utc, TimeSpan offset)
		{
			_offsetMinutes = Shared.GetWholeMinutes(offset);
			if (_offsetMinutes < -840 || _offsetMinutes > 840)
				throw new ArgumentException("offset must be max [+-] 14 hours");
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

		public DateTime UtcDateTime => _utc.UtcDateTime;

		/// <summary>
		/// Variable length local[+-]offset
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
		public static UtcOffsetTime ParseCosmosDb(string utc, TimeSpan offset)
		{
			var utcs = UtcTime.ParseCosmosDb(utc);
			return new UtcOffsetTime(utcs, offset);
		}

		public DateTimeOffset ToDateTimeOffset()
		{
			//return _dto;
			//var local = LocalDateTime_KindUtc;// _utc.UtcDateTime.AddMinutes(_offsetMins);

			// can not use Local kind as DTO will validate the offset against current local offset...
			// "DateTimeOffset Error: UTC offset of local dateTime does not match the offset argument"
			// KE?? but why not use the utc time directly?? It is not possible, must be unspecified time.
			return new DateTimeOffset(ClockDateTime_KindUnspecified, TimeSpan.FromMinutes(_offsetMinutes));
		}

		// TODO: remove this?
		//public DateTime ToLocalDateTime() => _utc.ToLocalDateTime();

		public override int GetHashCode() => _utc.GetHashCode();
		
		public override bool Equals(object obj) => obj is UtcOffsetTime other && Equals(other);

		/// <summary>
		/// Equal if the Utc time is equal.
		/// The offset is ignored, it is only used to make local times.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(UtcOffsetTime other) => this._utc == other._utc;
		
		public int CompareTo(UtcOffsetTime other) => this._utc.CompareTo(other._utc);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((UtcOffsetTime)obj);
		}

		public static TimeSpan operator -(UtcOffsetTime a, UtcOffsetTime b) => a._utc - b._utc;

		public static UtcOffsetTime operator +(UtcOffsetTime d, TimeSpan t) => new UtcOffsetTime(d._utc + t, d.Offset); 
		public static UtcOffsetTime operator -(UtcOffsetTime d, TimeSpan t) => new UtcOffsetTime(d._utc - t, d.Offset);

		public static bool operator ==(UtcOffsetTime a, UtcOffsetTime b) => a._utc == b._utc;
		public static bool operator !=(UtcOffsetTime a, UtcOffsetTime b) => a._utc != b._utc;
		public static bool operator <(UtcOffsetTime a, UtcOffsetTime b) => a._utc < b._utc;
		public static bool operator >(UtcOffsetTime a, UtcOffsetTime b) => a._utc > b._utc;
		public static bool operator <=(UtcOffsetTime a, UtcOffsetTime b) => a._utc <= b._utc;
		public static bool operator >=(UtcOffsetTime a, UtcOffsetTime b) => a._utc >= b._utc;

	}
}
