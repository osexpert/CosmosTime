using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosTime;
using CosmosTime.TimeZone;

namespace test
{
	class Program
	{
		class lol
		{
			public UtcTime t;
		}

		static void Main(string[] args)
		{
			IanaGeneratorTool.Generate(@"d:\timeZoneMap10.cs");


			var ss = UtcOffsetTime.LocalNow.ToString();

			var lt = new lol();
			if (lt.t == UtcTime.MinValue)
			{

			}

			var u = UtcOffsetTime.Parse("2020-01-01T12:00:00+01");
			var str = u.ToString();


			var u2 = UtcOffsetTime.Parse("2020-01-01T12:00:00-01");
			var str2 = u2.ToString();

			var dto = u.ToDateTimeOffset().ToString("O", System.Globalization.CultureInfo.InvariantCulture);
			var dto2 = u2.ToDateTimeOffset().ToString("O", System.Globalization.CultureInfo.InvariantCulture);

			var off2 = new UtcOffsetTime(new UtcTime(2020, 1, 1, 11,0,0), 60);
			var off3 = new UtcOffsetTime(new UtcTime(2020, 1, 1, 13,0,0), -60);
		}
	}
}
