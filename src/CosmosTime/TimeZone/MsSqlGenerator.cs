using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CosmosTime.TimeZone
{
	internal class MsSqlGenerator
	{
		public static void generator()
		{
			// https://learn.microsoft.com/en-us/azure/azure-sql/managed-instance/timezones-overview
			List<string> newLines = new List<string>();
			var liness = File.ReadLines(@"e:\sqltimezones", Encoding.UTF8);
			foreach (var l in liness)
			{
				var parts = Split(l, "(UTC");

				var p1 = parts[0].Trim();
				var p2 = ("(UTC" + parts[1]).Trim();
				//var s = $"case @tz = '{p2}' then '{p1}'";

				var s = $"case \"{p2}\": return \"{p1}\";";
				//case "lol": return "42";

				newLines.Add(s);
			}

			File.WriteAllLines(@"e:\sqlTimezonesnewcs.txt", newLines, Encoding.UTF8);
		}

		private static string[] Split(string l, string v)
		{
			throw new NotImplementedException();
		}
	}
}
