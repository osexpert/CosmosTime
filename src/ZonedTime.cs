using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	// Ideally we want the internal dt to be kind Unspecified, but what if _tz is Local or Utc?
	// It all depend on how TimeZoneInfo work. If it, when convert to utz tz give kind Utc, and when convert to same tz as the local, produce Local kind..
	// Must test

	public struct ZonedTime : IEquatable<ZonedTime>, IComparable<ZonedTime>, IComparable
	{

		DateTime _zoned;
		TimeZoneInfo _tz;

	
		public static ZonedTime ZonedNow(TimeZoneInfo destTz)
		{
			if (destTz == null)
				throw new ArgumentNullException("tz");

			if (destTz == TimeZoneInfo.Local)
				return DateTime.Now.ToZonedTime();
			else if (destTz == TimeZoneInfo.Utc)
				return DateTime.UtcNow.ToZonedTime();
			else // convert to time in the zone
				return new ZonedTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, destTz), destTz);
		}
		

		public ZonedTime ZonedNow()
		{
			return ZonedNow(_tz);
		}

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
				_tz = TimeZoneInfo.Local;
				_zoned = DateTime.SpecifyKind(utcOrLocalTime, DateTimeKind.Unspecified);
			}
			else if (utcOrLocalTime.Kind == DateTimeKind.Utc)
			{
				_tz = TimeZoneInfo.Utc;
				_zoned = utcOrLocalTime;
			}
			else
				throw new Exception("impossible, still unspec");
		}


		public ZonedTime(UtcTime utcTime, TimeZoneInfo tz)
		{
			if (tz == null)
				throw new ArgumentNullException("tz");

			_tz = tz;
			_zoned = TimeZoneInfo.ConvertTimeFromUtc(utcTime.UtcDateTime, tz);
			// _zoned.Kind = unspec, except if tz=utc, then kind = utc...
		}

		public ZonedTime(DateTime anyTime, DateTimeKind kindIfUnspecified)
		{
			if (kindIfUnspecified == DateTimeKind.Unspecified)
				throw new ArgumentException("kindIfUnspecified can not be Unspecified");

			if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				anyTime = DateTime.SpecifyKind(anyTime, kindIfUnspecified);
			}

			// Since Kind now is either Utc or Local, its easy
			if (anyTime.Kind == DateTimeKind.Local)
			{
				_tz = TimeZoneInfo.Local;
				// Does it matter if the dateTime Kind is Local??
				_zoned = DateTime.SpecifyKind(anyTime, DateTimeKind.Unspecified);
			}
			else if (anyTime.Kind == DateTimeKind.Utc)
			{
				_tz = TimeZoneInfo.Utc;
				_zoned = anyTime;
			}
			else
				throw new Exception("impossible, still unspec");
		}


		public ZonedTime(DateTime anyTime, TimeZoneInfo tzIfUnspecified)
		{
			if (tzIfUnspecified == null)
				throw new ArgumentNullException("tz");

			if (anyTime.Kind == DateTimeKind.Local)
			{
				_tz = TimeZoneInfo.Local;
				_zoned = DateTime.SpecifyKind(anyTime, DateTimeKind.Unspecified);
			}
			else if (anyTime.Kind == DateTimeKind.Utc)
			{
				_tz = TimeZoneInfo.Utc;
				_zoned = anyTime;
			}
			else if (anyTime.Kind == DateTimeKind.Unspecified)
			{
				_tz = tzIfUnspecified;

				if (tzIfUnspecified == TimeZoneInfo.Utc)
					_zoned = DateTime.SpecifyKind(anyTime, DateTimeKind.Utc);
				else
					_zoned = anyTime;
			}
			else 
				throw new Exception("impossible");
		}

		public UtcTime ToUtcTime() => new UtcTime(_zoned, _tz);

		public bool Equals(ZonedTime other)
		{
			return this._zoned == other._zoned && this._tz == other._tz;
		}

		public int CompareTo(ZonedTime other)
		{
			if (this._tz != other._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return this._zoned.CompareTo(other._zoned);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((ZonedTime)obj);
		}

		/// <summary>
		/// kind: Unspecified or Utc?
		/// What if tz is same as Local? Can it be Local then?
		/// I think it would be best if it NEVER was Kind local...
		/// </summary>
		public DateTime ZonedDateTime => _zoned;


		public long Ticks => _zoned.Ticks;

		//	public ZonedTime ToLocalZoneTime() => new ZonedTime(this, TimeZoneInfo.Local);

		

		public ZonedTime(int year, int month, int day, TimeZoneInfo tz) : this()
		{
			if (tz == null)
				throw new ArgumentNullException();

			_tz = tz;
			_zoned = new DateTime(year, month, day, 0, 0, 0, 
				tz == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
		}

		public ZonedTime(int year, int month, int day, int hour, int minute, int second, TimeZoneInfo tz) : this()
		{
			if (tz == null)
				throw new ArgumentNullException();

			_tz = tz;
			_zoned = new DateTime(year, month, day, hour, minute, second, 
				tz == TimeZoneInfo.Utc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
		}

		public ZonedTime Min(ZonedTime other)
		{
			if (this._tz != other._tz)
				throw new InvalidOperationException("Can't compare in different time zones");

			if (this._zoned < other._zoned)
				return this;
			else
				return other;
		}
		public ZonedTime Max(ZonedTime other)
		{
			if (this._tz != other._tz)
				throw new InvalidOperationException("Can't compare in different time zones");


			if (this._zoned > other._zoned)
				return this;
			else
				return other;
		}

		public static TimeSpan operator -(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned - b._zoned;
		}
		public static ZonedTime operator -(ZonedTime d, TimeSpan t)
		{
			return (d._zoned - t).ToZonedTime(d._tz);
		}
		public static ZonedTime operator +(ZonedTime d, TimeSpan t)
		{
			return (d._zoned + t).ToZonedTime(d._tz);
		}

		public static bool operator ==(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned == b._zoned && a._tz == b._tz;
		}

		public static bool operator !=(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned != b._zoned || a._tz != b._tz;
		}
		public static bool operator <(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned < b._zoned;
		}
		public static bool operator >(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned > b._zoned;
		}
		public static bool operator <=(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");
			return a._zoned <= b._zoned;
		}
		public static bool operator >=(ZonedTime a, ZonedTime b)
		{
			if (a._tz != b._tz)
				throw new InvalidOperationException("Can't compare in different time zones");

			return a._zoned >= b._zoned;
		}

		public ZonedTime AddSeconds(double sec) => _zoned.AddSeconds(sec).ToZonedTime(_tz);
		public ZonedTime AddMinutes(double min) => _zoned.AddMinutes(min).ToZonedTime(_tz);
		public ZonedTime AddHours(double h) => _zoned.AddHours(h).ToZonedTime(_tz);
		public ZonedTime AddDays(double days) => _zoned.AddDays(days).ToZonedTime(_tz);

		// kind of both is utc
		//public bool Equals(ZonedTime other) => _utc.Equals(other._utc);

		public override bool Equals(object obj) => obj is ZonedTime other && Equals(other);

		public override int GetHashCode() => (_zoned, _tz).GetHashCode(); // verify
	}
}
