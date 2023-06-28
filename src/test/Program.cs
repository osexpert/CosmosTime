using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
			var n = LocalTime.Now;
			var s = n.ToString();

			var nz = ZonedTime.Now(TimeZoneInfo.Local);
			var nzs = nz.ToString();

			var utc = UtcTime.Now.ToString();


			MsSqlTimeZoneTool.Generate(@"d:\sqltimezones.txt", @"d:\msSqlTimeZoneMap.cs");

			IanaTimeZoneTool.Generate(@"d:\timeZoneMap10.cs");
		}
	}
}
