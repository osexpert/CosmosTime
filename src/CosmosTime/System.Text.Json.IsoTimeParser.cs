// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Snatched from https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/JsonHelpers.Date.cs#L496
// https://github.com/dotnet/runtime/commit/2897f5ea6585f656abcf80f62d9da7fb2e05557c
// Unchanged, except removed all paring of local times. Local time must die.

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CosmosTime
{
	internal static class IsoTimeParser // JsonHelpers
	{
		[StructLayout(LayoutKind.Auto)]
		private struct DateTimeParseData
		{
			public int Year;
			public int Month;
			public int Day;
			public bool IsCalendarDateOnly;
			public int Hour;
			public int Minute;
			public int Second;
			public int Fraction; // This value should never be greater than 9_999_999.
			public int OffsetHours;
			public int OffsetMinutes;
			public bool OffsetNegative => OffsetToken == JsonConstants.Hyphen;
			public byte OffsetToken;
		}

		///// <summary>
		///// Returns <see langword="true"/> iff <paramref name="value"/> is between
		///// <paramref name="lowerBound"/> and <paramref name="upperBound"/>, inclusive.
		///// </summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound) => (value - lowerBound) <= (upperBound - lowerBound);


		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool IsValidDateTimeOffsetParseLength(int length)
		//{
		//	return IsInRangeInclusive(length, JsonConstants.MinimumDateTimeParseLength, JsonConstants.MaximumEscapedDateTimeOffsetParseLength);
		//}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool IsValidUnescapedDateTimeOffsetParseLength(int length)
		//{
		//	return IsInRangeInclusive(length, JsonConstants.MinimumDateTimeParseLength, JsonConstants.MaximumDateTimeOffsetParseLength);
		//}

		public static bool TryParseAsIso(string source, out DateTime value, out TimeZoneKind tzk)
			=> TryParseAsIso(source.Select(c => (byte)c).ToArray(), out value, out tzk);

		public static bool TryParseAsIso(string source, out DateTimeOffset value, out DateTime dtValue, out TimeZoneKind tzk)
			=> TryParseAsIso(source.Select(c => (byte)c).ToArray(), out value, out dtValue, out tzk );

		/// <summary>
		/// Parse the given UTF-8 <paramref name="source"/> as extended ISO 8601 format.
		/// </summary>
		/// <param name="source">UTF-8 source to parse.</param>
		/// <param name="value">The parsed <see cref="DateTime"/> if successful.</param>
		/// <returns>"true" if successfully parsed.</returns>
		public static bool TryParseAsIso(ReadOnlySpan<byte> source, out DateTime value, out TimeZoneKind tzk)
		{
			tzk = TimeZoneKind.None;

			if (!TryParseDateTimeOffset(source, out DateTimeParseData parseData))
			{
				value = default;
				return false;
			}

			if (parseData.OffsetToken == JsonConstants.UtcOffsetToken)
			{
				tzk = TimeZoneKind.Utc;
				return TryCreateDateTime(parseData, DateTimeKind.Utc, out value);
			}
			else if (parseData.OffsetToken == JsonConstants.Plus || parseData.OffsetToken == JsonConstants.Hyphen)
			{
				if (!TryCreateDateTimeOffset(ref parseData, out DateTimeOffset dateTimeOffset))
				{
					value = default;
					return false;
				}

				tzk = TimeZoneKind.Offset;
				value = dateTimeOffset.LocalDateTime;
				return true;
			}

			return TryCreateDateTime(parseData, DateTimeKind.Unspecified, out value);
		}

		/// <summary>
		/// Parse the given UTF-8 <paramref name="source"/> as extended ISO 8601 format.
		/// 
		/// value is set if tzk is Utc or Offset.
		/// dtValue is set if tzk is None
		/// 
		/// </summary>
		/// <param name="source">UTF-8 source to parse.</param>
		/// <param name="dtoValue">The parsed <see cref="DateTimeOffset"/> if successful.</param>
		/// <returns>"true" if successfully parsed.</returns>
		public static bool TryParseAsIso(ReadOnlySpan<byte> source, out DateTimeOffset dtoValue, out DateTime dtValue, out TimeZoneKind tzk)
		{
			dtValue = default;

			tzk = TimeZoneKind.None;

			if (!TryParseDateTimeOffset(source, out DateTimeParseData parseData))
			{
				dtoValue = default;
				return false;
			}

			if (parseData.OffsetToken == JsonConstants.UtcOffsetToken || // Same as specifying an offset of "+00:00", except that DateTime's Kind gets set to UTC rather than Local
				parseData.OffsetToken == JsonConstants.Plus || parseData.OffsetToken == JsonConstants.Hyphen)
			{
				tzk = parseData.OffsetToken == JsonConstants.UtcOffsetToken ? TimeZoneKind.Utc : TimeZoneKind.Offset;
				return TryCreateDateTimeOffset(ref parseData, out dtoValue);
			}

			// No offset, attempt to read as local time.
			// NO...keep local time out of this, dammit...
			//return TryCreateDateTimeOffsetInterpretingDataAsLocalTime(parseData, out value);
			return TryCreateDateTime(parseData, DateTimeKind.Unspecified, out dtValue);
		}

#if NETCOREAPP
        public static bool TryParseAsIso(ReadOnlySpan<byte> source, out DateOnly value)
        {
            if (TryParseDateTimeOffset(source, out DateTimeParseData parseData) &&
                parseData.IsCalendarDateOnly &&
                TryCreateDateTime(parseData, DateTimeKind.Unspecified, out DateTime dateTime))
            {
                value = DateOnly.FromDateTime(dateTime);
                return true;
            }

            value = default;
            return false;
        }
#endif

		/// <summary>
		/// ISO 8601 date time parser (ISO 8601-1:2019).
		/// </summary>
		/// <param name="source">The date/time to parse in UTF-8 format.</param>
		/// <param name="parseData">The parsed <see cref="DateTimeParseData"/> for the given <paramref name="source"/>.</param>
		/// <remarks>
		/// Supports extended calendar date (5.2.2.1) and complete (5.4.2.1) calendar date/time of day
		/// representations with optional specification of seconds and fractional seconds.
		///
		/// Times can be explicitly specified as UTC ("Z" - 5.3.3) or offsets from UTC ("+/-hh:mm" 5.3.4.2).
		/// If unspecified they are considered to be local per spec.
		///
		/// Examples: (TZD is either "Z" or hh:mm offset from UTC)
		///
		///  YYYY-MM-DD               (eg 1997-07-16)
		///  YYYY-MM-DDThh:mm         (eg 1997-07-16T19:20)
		///  YYYY-MM-DDThh:mm:ss      (eg 1997-07-16T19:20:30)
		///  YYYY-MM-DDThh:mm:ss.s    (eg 1997-07-16T19:20:30.45)
		///  YYYY-MM-DDThh:mmTZD      (eg 1997-07-16T19:20+01:00)
		///  YYYY-MM-DDThh:mm:ssTZD   (eg 1997-07-16T19:20:3001:00)
		///  YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45Z)
		///
		/// Generally speaking we always require the "extended" option when one exists (3.1.3.5).
		/// The extended variants have separator characters between components ('-', ':', '.', etc.).
		/// Spaces are not permitted.
		/// </remarks>
		/// <returns>"true" if successfully parsed.</returns>
		private static bool TryParseDateTimeOffset(ReadOnlySpan<byte> source, out DateTimeParseData parseData)
		{
			parseData = default;

			// too short datetime
			Debug.Assert(source.Length >= 10);

			// Parse the calendar date
			// -----------------------
			// ISO 8601-1:2019 5.2.2.1b "Calendar date complete extended format"
			//  [dateX] = [year]["-"][month]["-"][day]
			//  [year]  = [YYYY] [0000 - 9999] (4.3.2)
			//  [month] = [MM] [01 - 12] (4.3.3)
			//  [day]   = [DD] [01 - 28, 29, 30, 31] (4.3.4)
			//
			// Note: 5.2.2.2 "Representations with reduced precision" allows for
			// just [year]["-"][month] (a) and just [year] (b), but we currently
			// don't permit it.

			{
				uint digit1 = source[0] - (uint)'0';
				uint digit2 = source[1] - (uint)'0';
				uint digit3 = source[2] - (uint)'0';
				uint digit4 = source[3] - (uint)'0';

				if (digit1 > 9 || digit2 > 9 || digit3 > 9 || digit4 > 9)
				{
					return false;
				}

				parseData.Year = (int)(digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4);
			}

			if (source[4] != JsonConstants.Hyphen
				|| !TryGetNextTwoDigits(source.Slice(start: 5, length: 2), ref parseData.Month)
				|| source[7] != JsonConstants.Hyphen
				|| !TryGetNextTwoDigits(source.Slice(start: 8, length: 2), ref parseData.Day))
			{
				return false;
			}

			// We now have YYYY-MM-DD [dateX]
			if (source.Length == 10)
			{
				parseData.IsCalendarDateOnly = true;
				return true;
			}

			// Parse the time of day
			// ---------------------
			//
			// ISO 8601-1:2019 5.3.1.2b "Local time of day complete extended format"
			//  [timeX]   = ["T"][hour][":"][min][":"][sec]
			//  [hour]    = [hh] [00 - 23] (4.3.8a)
			//  [minute]  = [mm] [00 - 59] (4.3.9a)
			//  [sec]     = [ss] [00 - 59, 60 with a leap second] (4.3.10a)
			//
			// ISO 8601-1:2019 5.3.3 "UTC of day"
			//  [timeX]["Z"]
			//
			// ISO 8601-1:2019 5.3.4.2 "Local time of day with the time shift between
			// local time scale and UTC" (Extended format)
			//
			//  [shiftX] = ["+"|"-"][hour][":"][min]
			//
			// Notes:
			//
			// "T" is optional per spec, but _only_ when times are used alone. In our
			// case, we're reading out a complete date & time and as such require "T".
			// (5.4.2.1b).
			//
			// For [timeX] We allow seconds to be omitted per 5.3.1.3a "Representations
			// with reduced precision". 5.3.1.3b allows just specifying the hour, but
			// we currently don't permit this.
			//
			// Decimal fractions are allowed for hours, minutes and seconds (5.3.14).
			// We only allow fractions for seconds currently. Lower order components
			// can't follow, i.e. you can have T23.3, but not T23.3:04. There must be
			// one digit, but the max number of digits is implementation defined. We
			// currently allow up to 16 digits of fractional seconds only. While we
			// support 16 fractional digits we only parse the first seven, anything
			// past that is considered a zero. This is to stay compatible with the
			// DateTime implementation which is limited to this resolution.

			if (source.Length < 16)
			{
				// Source does not have enough characters for YYYY-MM-DDThh:mm
				return false;
			}

			// Parse THH:MM (e.g. "T10:32")
			if (source[10] != JsonConstants.TimePrefix || source[13] != JsonConstants.Colon
				|| !TryGetNextTwoDigits(source.Slice(start: 11, length: 2), ref parseData.Hour)
				|| !TryGetNextTwoDigits(source.Slice(start: 14, length: 2), ref parseData.Minute))
			{
				return false;
			}

			// We now have YYYY-MM-DDThh:mm
			Debug.Assert(source.Length >= 16);
			if (source.Length == 16)
			{
				return true;
			}

			byte curByte = source[16];
			int sourceIndex = 17;

			// Either a TZD ['Z'|'+'|'-'] or a seconds separator [':'] is valid at this point
			switch (curByte)
			{
				case JsonConstants.UtcOffsetToken:
					parseData.OffsetToken = JsonConstants.UtcOffsetToken;
					return sourceIndex == source.Length;
				case JsonConstants.Plus:
				case JsonConstants.Hyphen:
					parseData.OffsetToken = curByte;
					return ParseOffset(ref parseData, source.Slice(sourceIndex));
				case JsonConstants.Colon:
					break;
				default:
					return false;
			}

			// Try reading the seconds
			if (source.Length < 19
				|| !TryGetNextTwoDigits(source.Slice(start: 17, length: 2), ref parseData.Second))
			{
				return false;
			}

			// We now have YYYY-MM-DDThh:mm:ss
			Debug.Assert(source.Length >= 19);
			if (source.Length == 19)
			{
				return true;
			}

			curByte = source[19];
			sourceIndex = 20;

			// Either a TZD ['Z'|'+'|'-'] or a seconds decimal fraction separator ['.'] is valid at this point
			switch (curByte)
			{
				case JsonConstants.UtcOffsetToken:
					parseData.OffsetToken = JsonConstants.UtcOffsetToken;
					return sourceIndex == source.Length;
				case JsonConstants.Plus:
				case JsonConstants.Hyphen:
					parseData.OffsetToken = curByte;
					return ParseOffset(ref parseData, source.Slice(sourceIndex));
				case JsonConstants.Period:
					break;
				default:
					return false;
			}

			// Source does not have enough characters for second fractions (i.e. ".s")
			// YYYY-MM-DDThh:mm:ss.s
			if (source.Length < 21)
			{
				return false;
			}

			// Parse fraction. This value should never be greater than 9_999_999
			{
				int numDigitsRead = 0;
				int fractionEnd = Math.Min(sourceIndex + JsonConstants.DateTimeParseNumFractionDigits, source.Length);

				while (sourceIndex < fractionEnd && IsDigit(curByte = source[sourceIndex]))
				{
					if (numDigitsRead < JsonConstants.DateTimeNumFractionDigits)
					{
						parseData.Fraction = (parseData.Fraction * 10) + (int)(curByte - (uint)'0');
						numDigitsRead++;
					}

					sourceIndex++;
				}

				if (parseData.Fraction != 0)
				{
					while (numDigitsRead < JsonConstants.DateTimeNumFractionDigits)
					{
						parseData.Fraction *= 10;
						numDigitsRead++;
					}
				}
			}

			// We now have YYYY-MM-DDThh:mm:ss.s
			Debug.Assert(sourceIndex <= source.Length);
			if (sourceIndex == source.Length)
			{
				return true;
			}

			curByte = source[sourceIndex++];

			// TZD ['Z'|'+'|'-'] is valid at this point
			switch (curByte)
			{
				case JsonConstants.UtcOffsetToken:
					parseData.OffsetToken = JsonConstants.UtcOffsetToken;
					return sourceIndex == source.Length;
				case JsonConstants.Plus:
				case JsonConstants.Hyphen:
					parseData.OffsetToken = curByte;
					return ParseOffset(ref parseData, source.Slice(sourceIndex));
				default:
					return false;
			}


			static bool ParseOffset(ref DateTimeParseData parseData, ReadOnlySpan<byte> offsetData)
			{
				// Parse the hours for the offset
				if (offsetData.Length < 2
					|| !TryGetNextTwoDigits(offsetData.Slice(0, 2), ref parseData.OffsetHours))
				{
					return false;
				}

				// We now have YYYY-MM-DDThh:mm:ss.s+|-hh

				if (offsetData.Length == 2)
				{
					// Just hours offset specified
					return true;
				}

				// Ensure we have enough for ":mm"
				// GD: the basci format support +-nnnn, but the extended format is +-nn or +-nn:nn. This support only extended format (good).
				if (offsetData.Length != 5
					|| offsetData[2] != JsonConstants.Colon
					|| !TryGetNextTwoDigits(offsetData.Slice(3), ref parseData.OffsetMinutes))
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="value"/> is in the range [0..9].
		/// Otherwise, returns <see langword="false"/>.
		/// </summary>
		private static bool IsDigit(byte value) => (uint)(value - '0') <= '9' - '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryGetNextTwoDigits(ReadOnlySpan<byte> source, ref int value)
		{
			Debug.Assert(source.Length == 2);

			uint digit1 = source[0] - (uint)'0';
			uint digit2 = source[1] - (uint)'0';

			if (digit1 > 9 || digit2 > 9)
			{
				value = default;
				return false;
			}

			value = (int)(digit1 * 10 + digit2);
			return true;
		}

		// The following methods are borrowed verbatim from src/Common/src/CoreLib/System/Buffers/Text/Utf8Parser/Utf8Parser.Date.Helpers.cs

		/// <summary>
		/// Overflow-safe DateTimeOffset factory.
		/// </summary>
		private static bool TryCreateDateTimeOffset(DateTime dateTime, ref DateTimeParseData parseData, out DateTimeOffset value)
		{
			if (((uint)parseData.OffsetHours) > JsonConstants.MaxDateTimeUtcOffsetHours)
			{
				value = default;
				return false;
			}

			if (((uint)parseData.OffsetMinutes) > 59)
			{
				value = default;
				return false;
			}

			if (parseData.OffsetHours == JsonConstants.MaxDateTimeUtcOffsetHours && parseData.OffsetMinutes != 0)
			{
				value = default;
				return false;
			}

			long offsetTicks = (((long)parseData.OffsetHours) * 3600 + ((long)parseData.OffsetMinutes) * 60) * TimeSpan.TicksPerSecond;
			if (parseData.OffsetNegative)
			{
				offsetTicks = -offsetTicks;
			}

			try
			{
				value = new DateTimeOffset(ticks: dateTime.Ticks, offset: new TimeSpan(ticks: offsetTicks));
			}
			catch (ArgumentOutOfRangeException)
			{
				// If we got here, the combination of the DateTime + UTC offset strayed outside the 1..9999 year range. This case seems rare enough
				// that it's better to catch the exception rather than replicate DateTime's range checking (which it's going to do anyway.)
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Overflow-safe DateTimeOffset factory.
		/// </summary>
		private static bool TryCreateDateTimeOffset(ref DateTimeParseData parseData, out DateTimeOffset value)
		{
			if (!TryCreateDateTime(parseData, kind: DateTimeKind.Unspecified, out DateTime dateTime))
			{
				value = default;
				return false;
			}

			if (!TryCreateDateTimeOffset(dateTime: dateTime, ref parseData, out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Overflow-safe DateTimeOffset/Local time conversion factory.
		/// </summary>
		private static bool TryCreateDateTimeOffsetInterpretingDataAsLocalTime(DateTimeParseData parseData, out DateTimeOffset value)
		{
			if (!TryCreateDateTime(parseData, DateTimeKind.Local, out DateTime dateTime))
			{
				value = default;
				return false;
			}

			try
			{
				value = new DateTimeOffset(dateTime);
			}
			catch (ArgumentOutOfRangeException)
			{
				// If we got here, the combination of the DateTime + UTC offset strayed outside the 1..9999 year range. This case seems rare enough
				// that it's better to catch the exception rather than replicate DateTime's range checking (which it's going to do anyway.)
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Overflow-safe DateTime factory.
		/// </summary>
		private static bool TryCreateDateTime(DateTimeParseData parseData, DateTimeKind kind, out DateTime value)
		{
			if (parseData.Year == 0)
			{
				value = default;
				return false;
			}

			Debug.Assert(parseData.Year <= 9999); // All of our callers to date parse the year from fixed 4-digit fields so this value is trusted.

			if ((((uint)parseData.Month) - 1) >= 12)
			{
				value = default;
				return false;
			}

			uint dayMinusOne = ((uint)parseData.Day) - 1;
			if (dayMinusOne >= 28 && dayMinusOne >= DateTime.DaysInMonth(parseData.Year, parseData.Month))
			{
				value = default;
				return false;
			}

			if (((uint)parseData.Hour) > 23)
			{
				value = default;
				return false;
			}

			if (((uint)parseData.Minute) > 59)
			{
				value = default;
				return false;
			}

			// This needs to allow leap seconds when appropriate.
			// See https://github.com/dotnet/runtime/issues/30135.
			if (((uint)parseData.Second) > 59)
			{
				value = default;
				return false;
			}

			Debug.Assert(parseData.Fraction >= 0 && parseData.Fraction <= JsonConstants.MaxDateTimeFraction); // All of our callers to date parse the fraction from fixed 7-digit fields so this value is trusted.

			ReadOnlySpan<int> days = DateTime.IsLeapYear(parseData.Year) ? DaysToMonth366 : DaysToMonth365;
			int yearMinusOne = parseData.Year - 1;
			int totalDays = (yearMinusOne * 365) + (yearMinusOne / 4) - (yearMinusOne / 100) + (yearMinusOne / 400) + days[parseData.Month - 1] + parseData.Day - 1;
			long ticks = totalDays * TimeSpan.TicksPerDay;
			int totalSeconds = (parseData.Hour * 3600) + (parseData.Minute * 60) + parseData.Second;
			ticks += totalSeconds * TimeSpan.TicksPerSecond;
			ticks += parseData.Fraction;
			value = new DateTime(ticks: ticks, kind: kind);
			return true;
		}

		private static ReadOnlySpan<int> DaysToMonth365 => new int[] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
		private static ReadOnlySpan<int> DaysToMonth366 => new int[] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
	}
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CosmosTime
{
	internal static partial class JsonConstants
	{
		public const byte OpenBrace = (byte)'{';
		public const byte CloseBrace = (byte)'}';
		public const byte OpenBracket = (byte)'[';
		public const byte CloseBracket = (byte)']';
		public const byte Space = (byte)' ';
		public const byte CarriageReturn = (byte)'\r';
		public const byte LineFeed = (byte)'\n';
		public const byte Tab = (byte)'\t';
		public const byte ListSeparator = (byte)',';
		public const byte KeyValueSeparator = (byte)':';
		public const byte Quote = (byte)'"';
		public const byte BackSlash = (byte)'\\';
		public const byte Slash = (byte)'/';
		public const byte BackSpace = (byte)'\b';
		public const byte FormFeed = (byte)'\f';
		public const byte Asterisk = (byte)'*';
		public const byte Colon = (byte)':';
		public const byte Period = (byte)'.';
		public const byte Plus = (byte)'+';
		public const byte Hyphen = (byte)'-';
		public const byte UtcOffsetToken = (byte)'Z';
		public const byte TimePrefix = (byte)'T';

		// \u2028 and \u2029 are considered respectively line and paragraph separators
		// UTF-8 representation for them is E2, 80, A8/A9
		public const byte StartingByteOfNonStandardSeparator = 0xE2;

		public static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };
		public static ReadOnlySpan<byte> TrueValue => "true"u8;
		public static ReadOnlySpan<byte> FalseValue => "false"u8;
		public static ReadOnlySpan<byte> NullValue => "null"u8;

		public static ReadOnlySpan<byte> NaNValue => "NaN"u8;
		public static ReadOnlySpan<byte> PositiveInfinityValue => "Infinity"u8;
		public static ReadOnlySpan<byte> NegativeInfinityValue => "-Infinity"u8;

		// Used to search for the end of a number
		public static ReadOnlySpan<byte> Delimiters => ",}] \n\r\t/"u8;

		// Explicitly skipping ReverseSolidus since that is handled separately
		public static ReadOnlySpan<byte> EscapableChars => "\"nrt/ubf"u8;

		public const int SpacesPerIndent = 2;
		public const int RemoveFlagsBitMask = 0x7FFFFFFF;

		// In the worst case, an ASCII character represented as a single utf-8 byte could expand 6x when escaped.
		// For example: '+' becomes '\u0043'
		// Escaping surrogate pairs (represented by 3 or 4 utf-8 bytes) would expand to 12 bytes (which is still <= 6x).
		// The same factor applies to utf-16 characters.
		public const int MaxExpansionFactorWhileEscaping = 6;

		// In the worst case, a single UTF-16 character could be expanded to 3 UTF-8 bytes.
		// Only surrogate pairs expand to 4 UTF-8 bytes but that is a transformation of 2 UTF-16 characters going to 4 UTF-8 bytes (factor of 2).
		// All other UTF-16 characters can be represented by either 1 or 2 UTF-8 bytes.
		public const int MaxExpansionFactorWhileTranscoding = 3;

		// When transcoding from UTF8 -> UTF16, the byte count threshold where we rent from the array pool before performing a normal alloc.
		public const long ArrayPoolMaxSizeBeforeUsingNormalAlloc = 1024 * 1024;

		// The maximum number of characters allowed when writing raw UTF-16 JSON. This is the maximum length that we can guarantee can
		// be safely transcoded to UTF-8 and fit within an integer-length span, given the max expansion factor of a single character (3).
		public const int MaxUtf16RawValueLength = int.MaxValue / MaxExpansionFactorWhileTranscoding;

		public const int MaxEscapedTokenSize = 1_000_000_000;   // Max size for already escaped value.
		public const int MaxUnescapedTokenSize = MaxEscapedTokenSize / MaxExpansionFactorWhileEscaping;  // 166_666_666 bytes
		public const int MaxCharacterTokenSize = MaxEscapedTokenSize / MaxExpansionFactorWhileEscaping; // 166_666_666 characters

		public const int MaximumFormatBooleanLength = 5;
		public const int MaximumFormatInt64Length = 20;   // 19 + sign (i.e. -9223372036854775808)
		public const int MaximumFormatUInt64Length = 20;  // i.e. 18446744073709551615
		public const int MaximumFormatDoubleLength = 128;  // default (i.e. 'G'), using 128 (rather than say 32) to be future-proof.
		public const int MaximumFormatSingleLength = 128;  // default (i.e. 'G'), using 128 (rather than say 32) to be future-proof.
		public const int MaximumFormatDecimalLength = 31; // default (i.e. 'G')
		public const int MaximumFormatGuidLength = 36;    // default (i.e. 'D'), 8 + 4 + 4 + 4 + 12 + 4 for the hyphens (e.g. 094ffa0a-0442-494d-b452-04003fa755cc)
		public const int MaximumEscapedGuidLength = MaxExpansionFactorWhileEscaping * MaximumFormatGuidLength;
		public const int MaximumFormatDateTimeLength = 27;    // StandardFormat 'O', e.g. 2017-06-12T05:30:45.7680000
		public const int MaximumFormatDateTimeOffsetLength = 33;  // StandardFormat 'O', e.g. 2017-06-12T05:30:45.7680000-07:00
		public const int MaxDateTimeUtcOffsetHours = 14; // The UTC offset portion of a TimeSpan or DateTime can be no more than 14 hours and no less than -14 hours.
		public const int DateTimeNumFractionDigits = 7;  // TimeSpan and DateTime formats allow exactly up to many digits for specifying the fraction after the seconds.
		public const int MaxDateTimeFraction = 9_999_999;  // The largest fraction expressible by TimeSpan and DateTime formats
		public const int DateTimeParseNumFractionDigits = 16; // The maximum number of fraction digits the Json DateTime parser allows
		public const int MaximumDateTimeOffsetParseLength = (MaximumFormatDateTimeOffsetLength +
			(DateTimeParseNumFractionDigits - DateTimeNumFractionDigits)); // Like StandardFormat 'O' for DateTimeOffset, but allowing 9 additional (up to 16) fraction digits.
		public const int MinimumDateTimeParseLength = 10; // YYYY-MM-DD
		public const int MaximumEscapedDateTimeOffsetParseLength = MaxExpansionFactorWhileEscaping * MaximumDateTimeOffsetParseLength;

		public const int MaximumLiteralLength = 5; // Must be able to fit null, true, & false.

		// Encoding Helpers
		public const char HighSurrogateStart = '\ud800';
		public const char HighSurrogateEnd = '\udbff';
		public const char LowSurrogateStart = '\udc00';
		public const char LowSurrogateEnd = '\udfff';

		public const int UnicodePlane01StartValue = 0x10000;
		public const int HighSurrogateStartValue = 0xD800;
		public const int HighSurrogateEndValue = 0xDBFF;
		public const int LowSurrogateStartValue = 0xDC00;
		public const int LowSurrogateEndValue = 0xDFFF;
		public const int BitShiftBy10 = 0x400;

		// The maximum number of parameters a constructor can have where it can be considered
		// for a path on deserialization where we don't box the constructor arguments.
		public const int UnboxedParameterCountThreshold = 4;
	}

	internal enum TimeZoneKind
	{
		/// <summary>
		/// {time}
		/// </summary>
		None,
		/// <summary>
		/// {time}Z
		/// </summary>
		Utc,
		/// <summary>
		/// {time}+|-hh[:mm]
		/// </summary>
		Offset,
	}
}