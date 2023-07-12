using CosmosTime.TimeZone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CosmosTime
{

//	[TypeConverter(typeof(ZonedOffsetTimeTypeConverter))]
	public struct ZonedOffsetTime : IEquatable<ZonedOffsetTime>, IComparable<ZonedOffsetTime>, IComparable
	{
		ZonedTime _zoned;
		short _offsetMinutes;


		public ZonedTime ZonedTime => _zoned;

		/// <summary>
		/// Will capture Utc time + local offset to utc
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// Same as Now(TimeZoneInfo.Local)
		/// </summary>
//		public static ZonedOffsetTime LocalNow => DateTimeOffset.Now.ToUtcOffsetTime();

		/// <summary>
		/// An UtcOffsetTime with utc time without offset (no time zone info)
		/// Same as Now(TimeZoneInfo.Utc)
		/// </summary>
//		public static ZonedOffsetTime UtcNow => DateTimeOffset.UtcNow.ToUtcOffsetTime();

		/// <summary>
		/// Get Now in a zone (time will always be utc, but the offset captured will depend on the tz)
		/// </summary>
		/// <param name="tz"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static ZonedOffsetTime Now(TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			var zoned = ZonedTime.Now(tz);
			var off = tz.GetUtcOffset(zoned.ZonedDateTime);

			return new ZonedOffsetTime(zoned, GetWholeMinutes(off.TotalMinutes));

			//if (tz == TimeZoneInfo.Local)
			//	return DateTimeOffset.Now.ToZonedOffsetTime();
			//else if (tz == TimeZoneInfo.Utc)
			//	return DateTimeOffset.UtcNow.ToZonedOffsetTime();
			//else // convert to time in the zone
			//{
			//	//throw new NotImplementedException();

			//	//var timeInZone = DateTime.UtcNow.ToUtcTime(tz);
			//	var utcNow = UtcTime.Now;

			//	var dtInTz = TimeZoneInfo.ConvertTime(utcNow.UtcDateTime, tz);
			//	////var timeInZone = TimeZoneInfo.ConvertTime(utcNow, tz);

			//	var offsetMinsDbl = (dtInTz - utcNow.UtcDateTime).TotalMinutes;

			//	return new ToZonedOffsetTime(utcNow, GetWholeMinutes(offsetMinsDbl));
			//}
		}

		private static short GetWholeMinutes(double mins)
		{
			var res = (short)mins;
			if (res != mins)
				throw new Exception("fractions lost in offset");
			return res;
		}

	//	public static readonly ZonedOffsetTime MinValue = DateTimeOffset.MinValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MinValue, 0);
//		public static readonly ZonedOffsetTime MaxValue = DateTimeOffset.MaxValue.ToUtcOffsetTime();// new OffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

//		public UtcTime UtcTime => _utc;

		/// <summary>
		/// Offset from Utc
		/// </summary>
		public short OffsetMinutes => _offsetMinutes;

		public TimeSpan Offset => TimeSpan.FromMinutes(_offsetMinutes);

		/// <summary>
		/// FIXME: correct?? yes
		/// </summary>
		public long Ticks => ClockDateTime_KindUnspecified.Ticks;

		//		public long UtcTicks => _utc.Ticks;



		/// <summary>
		///
		/// </summary>
		//public ZonedOffsetTime(DateTimeOffset dto)
		//{
		//	// what about dto.ToUniversalTime? versus  dto.UtcDateTime ???
		//	//dto.ToUniversalTime sets offset to 0. But they are still equal!!
		//	// Yes, but so are WE. We only compare _utc too.

		//	//_dto = dto;
		//	// FIXME: DateTime or LocalDateTime?????????????
		//	_zoned = dto.LocalDateTime.ToZonedTime()
		//	// TODO: create some tests to make sure this roundtrips
		//	_offsetMinutes = GetWholeMinutes(dto.Offset.TotalMinutes);
		//}

		/// <summary>
		/// offsetMinutes: utc+offsetMinutes=local
		/// </summary>
		/// <param name="utcs"></param>
		/// <param name="offsetMinutes"></param>
		/// <exception cref="ArgumentException"></exception>
		//public ZonedOffsetTime(UtcTime utc, short offsetMinutes) : this()
		//{
		//	//		var local = utc.UtcDateTime + TimeSpan.FromMinutes(offsetMinutes);
		//	//			_dto = new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeSpan.FromMinutes(offsetMinutes));

		//	_utc = utc;

		//	if (offsetMinutes < -840 || offsetMinutes > 840)
		//		throw new ArgumentException("offset must be max [+-] 14 hours");

		//	_offsetMinutes = offsetMinutes;
		//}


		/// <summary>
		/// If time is ambigous, uses the standard time\offset
		/// </summary>
		public ZonedOffsetTime(ZonedTime zoned) : this(zoned, GetWholeMinutes(zoned.Zone.GetUtcOffset(zoned.ZonedDateTime).TotalMinutes))
		{
		}

		/// <summary>
		/// If time is ambigous, can specifify offset yourself
		/// </summary>
		public ZonedOffsetTime(ZonedTime zoned, short offsetMinutes) : this()
		{
			//		var local = utc.UtcDateTime + TimeSpan.FromMinutes(offsetMinutes);
			//			_dto = new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeSpan.FromMinutes(offsetMinutes));

			_zoned = zoned;

			if (offsetMinutes < -840 || offsetMinutes > 840)
				throw new ArgumentException("offset must be max [+-] 14 hours");

			// FIXME: is there an easier\more effective way to validate this?
			if (zoned.Zone.IsAmbiguousTime(zoned.ZonedDateTime))
			{
				var validOffsets = zoned.Zone.GetAmbiguousTimeOffsets(zoned.ZonedDateTime);

				if (!validOffsets.Any(o => o.TotalMinutes == offsetMinutes))
					throw new ArgumentException("Offset is not valid in zone (none of the ambiguous offsets)");
			}
			else if (zoned.Zone.GetUtcOffset(zoned.ZonedDateTime).TotalMinutes != offsetMinutes)
			{
				throw new ArgumentException("Offset is not valid in zone");
			}

			_offsetMinutes = offsetMinutes;

			// validate offset? well...the time can have 2 different offsets, and we don't know which.
//			zoned.Zone.GetUtcOffset()
		}


		//private DateTime ClockDateTime_KindUtc => _utc.UtcDateTime.AddMinutes(_offsetMins);// _utc.AddMinutes(_offsetMins);
		private DateTime ClockDateTime_KindUnspecified => DateTime.SpecifyKind(_zoned.ZonedDateTime, DateTimeKind.Unspecified);// _utc.AddMinutes(_offsetMins);

		/// <summary>
		/// Variable length local[+-]offset
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var local = ClockDateTime_KindUnspecified;// _dto.DateTime;// _utc.AddMinutes(_offsetMins);

			//int seconds = 10000; //or whatever time you have
			//string.Format("{0:00}':'{1:00}", seconds / 3600, (seconds / 60) % 60);
			var mins = _offsetMinutes;
			bool neg = mins < 0;
			if (neg)
				mins *= -1;

			var strNoZ = local.ToString(Constants.VariableLengthMicrosIsoFormatWithoutZ);

			var off = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
			var res = $"{strNoZ}{(neg ? '-' : '+')}{off}";

			if (IanaTimeZone.TryGetIanaId(_zoned.Zone, out var ianaId))
				return $"{res}[{ianaId}]";
			else
				return res;
		}

		/// <summary>
		/// Parse fixed length utc (28 chars, ends with Z)
		/// </summary>
		//public static ZonedOffsetTime ParseCosmosDb(string utc, short offsetMinutes)
		//{
		//	var utcs = UtcTime.ParseCosmosDb(utc);
		//	return new ZonedOffsetTime(utcs, offsetMinutes);
		//}

		public DateTimeOffset ToDateTimeOffset()
		{
			//return _dto;
			//var local = LocalDateTime_KindUtc;// _utc.UtcDateTime.AddMinutes(_offsetMins);

			// can not use Local kind as DTO will validate the offset against current local offset...
			// "DateTimeOffset Error: UTC offset of local dateTime does not match the offset argument"
			// KE?? but why not use the utc time directly?? It is not possible, must be unspecified time.
			return new DateTimeOffset(ClockDateTime_KindUnspecified, TimeSpan.FromMinutes(_offsetMinutes));
		}

		public UtcOffsetTime ToUtcOffsetTime()
		{
			return new UtcOffsetTime(_zoned.ToUtcTime(), _offsetMinutes);
		}

		// TODO: remove this?
		//public DateTime ToLocalDateTime() => _utc.ToLocalDateTime();

		public override int GetHashCode() => _zoned.GetHashCode();

		public override bool Equals(object obj) => obj is ZonedOffsetTime other && Equals(other);

		/// <summary>
		/// Equal if the Utc time is equal.
		/// The offset is ignored, it is only used to make local times.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ZonedOffsetTime other) => this._zoned == other._zoned;

		public int CompareTo(ZonedOffsetTime other) => this._zoned.CompareTo(other._zoned);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ZonedOffsetTime)obj);
		}

		public static ZonedOffsetTime operator +(ZonedOffsetTime d, TimeSpan t) => new ZonedOffsetTime(d._zoned + t, d._offsetMinutes);
		public static ZonedOffsetTime operator -(ZonedOffsetTime d, TimeSpan t) => new ZonedOffsetTime(d._zoned - t, d._offsetMinutes);
		public static TimeSpan operator -(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned - b._zoned;

		public static bool operator ==(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned == b._zoned;
		public static bool operator !=(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned != b._zoned;
		public static bool operator <(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned < b._zoned;
		public static bool operator >(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned > b._zoned;
		public static bool operator <=(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned <= b._zoned;
		public static bool operator >=(ZonedOffsetTime a, ZonedOffsetTime b) => a._zoned >= b._zoned;

	}
}
