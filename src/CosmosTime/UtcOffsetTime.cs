using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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

//		delegate Boolean dHello(ReadOnlySpan<char> a, DateTimeFormatInfo b, DateTimeStyles c, out DateTime d, out TimeSpan e);

		public static bool TryParse(string utcOffsetString, out UtcOffsetTime uo)
		{
			uo = UtcOffsetTime.MinValue;

			if (IsoTimeParser.TryParseAsIso(utcOffsetString, allowLocal: false, out DateTimeOffset dto))
			{
				uo = dto.ToUtcOffsetTime();
				return true;
			}

//			var t = Type.GetType("System.DateTimeParse");

//			TimeSpan offset;// = TimeSpan.Zero;
//			DateTime dateResult;// = DateTime.MinValue;

//			var sp = new ReadOnlySpan<char>(utcOffsetString.ToArray());
//			//object oo = (object)sp;
//			//			object off;
//			//		object dr;

//			var m = t.GetMethod("TryParse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, 
//				null, System.Reflection.CallingConventions.Standard,
//				new Type[] { typeof(ReadOnlySpan<char>), typeof(DateTimeFormatInfo), typeof(DateTimeStyles), typeof(DateTime).MakeByRefType(), typeof(TimeSpan).MakeByRefType() }, null);
				
//			var dele = (dHello)m.CreateDelegate(typeof(dHello), null);
//			var res = dele(sp, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture), DateTimeStyles.RoundtripKind, out dateResult, out offset);

//			if (!res)
//				return false;

//			if (dateResult.Kind == DateTimeKind.Unspecified)
//				return false;

////			bool b = (bool)t.InvokeMember("TryParse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static, null,
//	//			null, new object[] { oo, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture), DateTimeStyles.None, dateResult, offset });
//			//Boolean parsed = DateTimeParse.TryParse(input,
//			//										DateTimeFormatInfo.CurrentInfo,
//			//										DateTimeStyles.None,
//			//										out dateResult,
//			//										out offset);


//			//public static Boolean TryParse(String input, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
//			//{
//			//	styles = ValidateStyles(styles, "styles");
//			//	TimeSpan offset;
//			//	DateTime dateResult;
//			//	Boolean parsed = DateTimeParse.TryParse(input,
//			//											DateTimeFormatInfo.GetInstance(formatProvider),
//			//											styles,
//			//											out dateResult,
//			//											out offset);
//			//	result = new DateTimeOffset(dateResult.Ticks, offset);
//			//	return parsed;
//			//}



//			/* 2020-10-27T10:59:54Z -> offset 0
//			 * 2020-10-27T10:59:54 -> local time (BAD) Will not allow this... must check manually
//			 * Only support 2 tiem zone formats: [+-]nn:nn and [+-]nnnn
//			 * 2020-10-27T10:59:54+00:10  -> offset 10min
//		 	 * 2020-10-27T10:59:54+0010  -> offset 10min

//			 * 
//			 * DateTimeStyles.AdjustToUniversal and DateTimeStyles.RoundtripKind are very similar in a way, and mutually exlusive (cannot be used together)
//			 * */

//			// offset local time(BAD) Will not allow this... must check manually
//			Func<string, bool> endsWithZ = (str) => str.Length > 0 && str[str.Length - 1] == 'Z';
//			// This is not reliable. It is really hard to detect\extract the offset, because there are so many allowed formats!
//			// And there is no way to tell DateTimeOffset to not allow parsing local times.
//			// So we need to fake it, for now:
//			//Func<string, bool> hasOffset = (str) =>
//			//{
//			//	for (int i = 2; i <= 5 && i <= str.Length; i++)
//			//	{
//			//		var c = str[str.Length - i];
//			//		if (c == '-' || c == '+')
//			//			return true;
//			//	}
//			//	return false;
//			//};

//			//if (DateTimeOffset.TryParseExact(utcOffsetString, new[]{
//			//	Constants.VariableLengthMicrosIsoFormatWithZ, Constants.VariableLengthMicrosIsoFormatWithTZ}, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
//			//{
//			//	uo = dto.ToUtcOffsetTime();
//			//	return true;
//			//}

//			// DateTimeStyles.RoundtripKind seem to have no effect on DateTimeOffset. but set it anyways
//			//			if (endsWithZ(utcOffsetString) || hasOffset(utcOffsetString))
//			//if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles., out var dto))
//			//{
//			//	uo = dto.ToUtcOffsetTime();
//			//	return true;
//			//}

//			uo = UtcOffsetTime.MinValue;
//			if (!utcOffsetString.Any())
//				return false;

//			// fast path for ending witn Z
//			if (utcOffsetString.Last() == 'Z')
//			{
//				if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto1))
//				{
//					uo = dto1.ToUtcOffsetTime();
//					return true;
//				}
//				// slow path
//				else
//				{
//					return false;
//				}
//			}

//			// todo: fast path for ending with TZ?
//			//-0400 or -00:30
//			else if (utcOffsetString.Length > 6)
//			{
//				if ((Char.IsDigit(utcOffsetString[utcOffsetString.Length - 1]) && Char.IsDigit(utcOffsetString[utcOffsetString.Length - 2])
//					&& utcOffsetString[utcOffsetString.Length - 3] == ':'
//					&& Char.IsDigit(utcOffsetString[utcOffsetString.Length - 4]) && Char.IsDigit(utcOffsetString[utcOffsetString.Length - 5])
//					&& (utcOffsetString[utcOffsetString.Length - 6] == '-' || utcOffsetString[utcOffsetString.Length - 6] == '+')
//					)
//					||
//					(Char.IsDigit(utcOffsetString[utcOffsetString.Length - 1]) && Char.IsDigit(utcOffsetString[utcOffsetString.Length - 2])
//					&& Char.IsDigit(utcOffsetString[utcOffsetString.Length - 3]) && Char.IsDigit(utcOffsetString[utcOffsetString.Length - 4])
//					&& (utcOffsetString[utcOffsetString.Length - 5] == '-' || utcOffsetString[utcOffsetString.Length - 5] == '+')
//					)
//					)
//				{
//					if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto2))
//					{
//						uo = dto2.ToUtcOffsetTime();
//						return true;
//					}
//					// slow path
//					else
//					{
//						return false;
//					}

//				}
//			}

			//if (DateTimeOffset.TryParse(utcOffsetString + "Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var _))
			//{
			//	// it must have been local time, since it could be parsed with an extra Z
			//	return false;
			//}

			//// else it must be a time with zone


			//if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
			//{
			//	uo = dto.ToUtcOffsetTime();
			//	return true;
			//}

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

			var strNoZ = local.ToString(Constants.VariableLengthMicrosIsoFormatWithoutZ);

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
