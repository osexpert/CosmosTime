﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{

	[TypeConverter(typeof(UtcOffsetTimeTypeConverter))]
	public struct UtcOffsetTime : IEquatable<UtcOffsetTime>, IComparable<UtcOffsetTime>, IComparable
	{
		UtcTime _utc;
		short _offsetMins;

		/// <summary>
		/// It make little sense to call this on a server, it will capture the server offset to utc, and that make little sense.
		/// So rename to NowLocal\LocalNow
		/// </summary>
		public static UtcOffsetTime LocalNow => DateTimeOffset.Now.ToUtcOffsetTime();

		/// <summary>
		/// An UtcOffsetTime without offset (no time zone info)
		/// </summary>
		public static UtcOffsetTime UtcNow => DateTimeOffset.UtcNow.ToUtcOffsetTime();

		public UtcTime UtcTime => _utc;

		public static readonly UtcOffsetTime MinValue = new UtcOffsetTime(UtcTime.MinValue, 0);
		public static readonly UtcOffsetTime MaxValue = new UtcOffsetTime(UtcTime.MaxValue, 0); // yes, offset should be 0 just as DateTimeOffset does

		/// <summary>
		/// Offset from Utc
		/// </summary>
		public int OffsetMins => _offsetMins;
		public TimeSpan Offset => TimeSpan.FromMinutes(_offsetMins);

		public static UtcOffsetTime Parse(string str)
		{
			if (TryParse(str, out var ut))
				return ut;
			throw new FormatException("not utc or local[+-]offset");
		}

		public static bool TryParse(string utcOffsetString, out UtcOffsetTime uo)
		{
			/* 2020-10-27T10:59:54Z -> offset 0
			 * 2020-10-27T10:59:54 -> local time (BAD) Will not allow this... must check manually
			 * 2020-10-27T10:59:54+00:10  -> offset 10min

			 * 
			 * DateTimeStyles.AdjustToUniversal and DateTimeStyles.RoundtripKind are very similar in a way, and mutually exlusive (cannot be used together)
			 * */

			// offset local time(BAD) Will not allow this... must check manually
			Func<string, bool> endsWithZ = (str) => str.Length > 0 && str[str.Length - 1] == 'Z';
			Func<string, bool> hasOffset = (str) =>
			{
				for (int i = 2; i <= 6 && i <= str.Length; i++)
				{
					var c = str[str.Length - i];
					if (c == '-' || c == '+')
						return true;
				}
				return false;
			};

			// DateTimeStyles.RoundtripKind seem to have no effect on DateTimeOffset. but set it anyways
			if (endsWithZ(utcOffsetString) || hasOffset(utcOffsetString))
				if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
				{
					uo = dto.ToUtcOffsetTime();
					return true;
				}

			uo = UtcOffsetTime.MinValue;
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
			_offsetMins = (short)dto.Offset.TotalMinutes;
		}

		/// <summary>
		/// offsetMinutes: specify it is the offset from local time to utc
		/// </summary>
		/// <param name="utcs"></param>
		/// <param name="offsetMinutes"></param>
		/// <exception cref="ArgumentException"></exception>
		public UtcOffsetTime(UtcTime utcs, short offsetMinutes) : this()
		{
			_utc = utcs;

			if (offsetMinutes < -840 || offsetMinutes > 840)
				throw new ArgumentException("offset must be max [+-] 14 hours");

			_offsetMins = offsetMinutes;
		}

		/// <summary>
		/// Variable length local[+-]offset
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var local = _utc.AddMinutes(_offsetMins);

			//int seconds = 10000; //or whatever time you have
			//string.Format("{0:00}':'{1:00}", seconds / 3600, (seconds / 60) % 60);
			var mins = _offsetMins;
			bool neg = mins < 0;
			if (neg)
				mins *= -1;

			var strNoZ = local.ToString(Constants.VariableLengthIsoFormatWithoutZ);

			var off = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
			var res = $"{strNoZ}{(neg ? '-' : '+')}{off}";

			return res;
		}

		public static UtcOffsetTime ParseCosmosDb(string utc, short offsetMinutes)
		{
			var utcs = UtcTime.ParseCosmosDb(utc);
			return new UtcOffsetTime(utcs, offsetMinutes);
		}

		public DateTimeOffset ToDateTimeOffset()
		{
			var local = _utc.UtcDateTime.AddMinutes(_offsetMins);

			// can not use Local kind as DTO will validate the offset against current local offset...
			// "DateTimeOffset Error: UTC offset of local dateTime does not match the offset argument"
			// KE?? but why not use the utc time directly?? It is not possible, must be unspecified time.
			return new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeSpan.FromMinutes(_offsetMins));
		}

		public DateTime ToLocalDateTime() => _utc.ToLocalDateTime();

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

		public static UtcOffsetTime operator +(UtcOffsetTime d, TimeSpan t) => new UtcOffsetTime(d._utc + t, d._offsetMins); 
		public static UtcOffsetTime operator -(UtcOffsetTime d, TimeSpan t) => new UtcOffsetTime(d._utc - t, d._offsetMins);
		public static TimeSpan operator -(UtcOffsetTime a, UtcOffsetTime b) => a._utc - b._utc;

		public static bool operator ==(UtcOffsetTime a, UtcOffsetTime b) => a._utc == b._utc;
		public static bool operator !=(UtcOffsetTime a, UtcOffsetTime b) => a._utc != b._utc;
		public static bool operator <(UtcOffsetTime a, UtcOffsetTime b) => a._utc < b._utc;
		public static bool operator >(UtcOffsetTime a, UtcOffsetTime b) => a._utc > b._utc;
		public static bool operator <=(UtcOffsetTime a, UtcOffsetTime b) => a._utc <= b._utc;
		public static bool operator >=(UtcOffsetTime a, UtcOffsetTime b) => a._utc >= b._utc;

	}
}
