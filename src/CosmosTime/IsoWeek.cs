using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CosmosTime
{
	/// <summary>
	/// reprent an ISO week that start on moday
	/// </summary>
	public struct IsoWeek : IEquatable<IsoWeek>, IComparable<IsoWeek>, IComparable
	{
		public IsoWeek(int year, int number)
		{
			if (number < 1)
				throw new ArgumentException("number < 1");

			var wiy = GetWeeksInYear(year);
			if (number > wiy)
				throw new ArgumentException("number > weeks in year: " + wiy);

			Year = year;
			Number = number;
		}

		public override string ToString()
		{
			return $"{this.Year}-W{this.Number:00}"; // test 00 padding
		}

		public int Number { get; private set; }
		public int Year { get; private set; }

		public int CompareTo(IsoWeek other)
		{
			var i = this.Year.CompareTo(other.Year);
			if (i == 0)
				i = this.Number.CompareTo(other.Number);
			return i;
		}

		public int CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((IsoWeek)obj);
		}

		public override bool Equals(object obj) => obj is IsoWeek other && Equals(other);

		public bool Equals(IsoWeek other)
		{
			return this.Year == other.Year && this.Number == other.Number;
		}

		public override int GetHashCode()
		{
			return (Year, Number).GetHashCode();
		}

		public static IsoWeek GetWeek(DateTime dt)
		{
			var year = ISOWeek.GetYear(dt, out var week);
			return new IsoWeek() { Year = year, Number = week };
		}

		public DateTime GetFirstDate() // DateOnly?
		{
			//return DateOnly.FromDateTime(ISOWeek.ToDateTime(Year, Number, DayOfWeek.Monday));
			return ISOWeek.ToDateTime(Year, Number, DayOfWeek.Monday);
		}

		public DateTime GetLastDate() // DateOnly?
		{
			//return DateOnly.FromDateTime(
			return ISOWeek.ToDateTime(Year, Number, DayOfWeek.Sunday);
		}

		public static int GetWeeksInYear(int year) => ISOWeek.GetWeeksInYear(year);

		public static IEnumerable<IsoWeek> GetWeeksInYearIterator(int year)
		{
			var num = ISOWeek.GetWeeksInYear(year);
			for (int i = 1; i <= num; i++)
			{
				yield return new IsoWeek { Year = year, Number = i };
			}
		}

		public static IEnumerable<IsoWeek> GetWeeksInRangeIterator(DateTime from, DateTime to)
		{
			if (from > to)
				throw new ArgumentException("from > to");

			IsoWeek lastWeek = to.GetWeek();

			for (IsoWeek w = from.GetWeek(); w <= lastWeek; w = w.GetNext())
				yield return w;
		}

		public IsoWeek GetNext()
		{
			var next = this.Number + 1;
			var year = this.Year;

			if (next > GetWeeksInYear(year))
			{
				year++;
				next = 1;
			}
			return new IsoWeek { Year = year, Number = next};
		}

		public IsoWeek GetPrevious()
		{
			var prev = this.Number - 1;
			var year = this.Year;

			if (prev < 1)
			{
				year--;
				prev = GetWeeksInYear(year);
			}
			return new IsoWeek { Year = year, Number = prev };
		}

		public static bool operator ==(IsoWeek w1, IsoWeek w2)
		{
			return w1.Equals(w2);
		}
		public static bool operator !=(IsoWeek w1, IsoWeek w2)
		{
			return !w1.Equals(w2);
		}
		public static bool operator <(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) < 0;
		}
		public static bool operator <=(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) <= 0;
		}
		public static bool operator >(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) > 0;
		}
		public static bool operator >=(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) >= 0;
		}
	}
}
