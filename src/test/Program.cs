using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CosmosTime;
using CosmosTime.TimeZone;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{

			var w = new DateTime();

			//			Now say you want to add a certain number of days instead. Doing that on the universal time-line may end up working not quite as expected:

			//λ > --It's 23:30 on 2022-03-12 in the America/Winnipeg time zone.
			//λ > tz = TZ.tzByLabel TZ.America__Winnipeg
			//λ > t1 = LocalTime(YearMonthDay 2022 3 12)(TimeOfDay 23 30 0)

			//λ > --Convert to UTC, add 1 day, convert back to our time zone.
			//λ > TZ.LTUUnique t1utc _ = TZ.localTimeToUTCFull tz t1
			//λ > t2utc = addUTCTime nominalDay t1utc
			//λ > TZ.utcToLocalTimeTZ tz t2utc
			//2022 - 03 - 14 00:30:00

			//We've accidentally landed two days ahead instead of just one. This happened because, on 2022-03-13, the clocks were turned forward 1 hour, so that day only had 23 hours on that time zone. Adding 24 hours on the universal time-line ended up being too much.

			var tz = IanaTimeZone.GetTimeZoneInfo("America/Winnipeg");
			var tiemInZone = new ZoneTime(new ClockTime(2022, 3, 12, 23, 30, 0, 0), tz);

			var next = tiemInZone + TimeSpan.FromDays(1);

			var u = tiemInZone.OffsetTime.UtcTime;//.UtcDateTime;
			u = u + TimeSpan.FromDays(1);
//			var backInZone = u.to FIXME


			Console.WriteLine("ff");



			//			λ > t1 = [tz | 2022 - 03 - 12 23:30:00[America / Winnipeg] |]
			//λ > modifyLocal(addCalendarClip(calendarDays 1)) t1
			//2022 - 03 - 13 23:30:00 - 05:00[America / Winnipeg]

			var tz2 = IanaTimeZone.GetTimeZoneInfo("America/Winnipeg");
			var tiemInZone2 = new ZoneTime(new ClockTime(2022, 3, 12, 23, 30, 0, 0), tz);
			var newTime2 = tiemInZone + TimeSpan.FromDays(1);

			Console.WriteLine("ff");

			//var n = LocalTime.Now;
			//var s = n.ToString();

			var nz = ZoneTime.Now(TimeZoneInfo.Local);
			var nzs = nz.ToString();

			var utc = UtcTime.Now.ToString();


//			MsSqlTimeZoneTool.Generate(@"d:\sqltimezones.txt", @"d:\msSqlTimeZoneMap.cs");

			IanaTimeZoneTool.Generate(@"d:\timeZoneMap10.cs");

	//		TypeDescriptor.AddAttributes(typeof(Guid), new TypeConverterAttribute(
	//typeof(MyGuidConverter)));


		}
	}
}
