using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Represent an ISO week that starts on monday
	/// </summary>
	public struct IsoWeek : IEquatable<IsoWeek>, IComparable<IsoWeek>, IComparable
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <param name="number"></param>
		/// <exception cref="ArgumentException"></exception>
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

		/// <summary>
		/// Iso format
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{this.Year}-W{this.Number:00}";
		}

		/// <summary>
		/// TODO
		/// </summary>
		public int Number { get; private set; }
		/// <summary>
		/// TODO
		/// </summary>
		public int Year { get; private set; }

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(IsoWeek other)
		{
			var i = this.Year.CompareTo(other.Year);
			if (i == 0)
				i = this.Number.CompareTo(other.Number);
			return i;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((IsoWeek)obj);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) => obj is IsoWeek other && Equals(other);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IsoWeek other)
		{
			return this.Year == other.Year && this.Number == other.Number;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (Year, Number).GetHashCode();
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static IsoWeek GetWeek(DateTime dt)
		{
			var weekAndYear = ISOWeek.GetWeekAndYear(dt);
			return new IsoWeek() { Year = weekAndYear.Year, Number = weekAndYear.Week };
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public DateOnly GetFirstDate()
		{
			//return DateOnly.FromDateTime(ISOWeek.ToDateTime(Year, Number, DayOfWeek.Monday));
			return DateOnly.FromDateTime(ISOWeek.ToDateTime(Year, Number, DayOfWeek.Monday));
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public DateOnly GetLastDate()
		{
			//return DateOnly.FromDateTime(
			return DateOnly.FromDateTime(ISOWeek.ToDateTime(Year, Number, DayOfWeek.Sunday));
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		public static int GetWeeksInYear(int year) => ISOWeek.GetWeeksInYear(year);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		public static IEnumerable<IsoWeek> GetWeeksInYearIterator(int year)
		{
			var num = ISOWeek.GetWeeksInYear(year);
			for (int i = 1; i <= num; i++)
			{
				yield return new IsoWeek { Year = year, Number = i };
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static IEnumerable<IsoWeek> GetWeeksInRangeIterator(DateTime from, DateTime to)
		{
			if (from > to)
				throw new ArgumentException("from > to");

			IsoWeek lastWeek = to.GetWeek();

			for (IsoWeek w = from.GetWeek(); w <= lastWeek; w = w.GetNext())
				yield return w;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator ==(IsoWeek w1, IsoWeek w2)
		{
			return w1.Equals(w2);
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator !=(IsoWeek w1, IsoWeek w2)
		{
			return !w1.Equals(w2);
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator <(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) < 0;
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator <=(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) <= 0;
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator >(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) > 0;
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns></returns>
		public static bool operator >=(IsoWeek w1, IsoWeek w2)
		{
			return w1.CompareTo(w2) >= 0;
		}
	}
}
