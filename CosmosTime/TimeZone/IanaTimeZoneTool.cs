﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace CosmosTime.TimeZone
{
	/// <summary>
	/// TODO
	/// </summary>
	public class IanaTimeZoneTool
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="timeZoneMapFileToWrite"></param>
		public static void Generate(string timeZoneMapFileToWrite)
		{
			// Embed time zone map so we don't have to take dependency on a nuget package for this simple task.

			var f = File.OpenWrite(timeZoneMapFileToWrite);
			var ff = new StreamWriter(f, new UTF8Encoding(false));

			var http = new HttpClient();


			Dictionary<string, string> IanaToWindows = new Dictionary<string, string>();
			Dictionary<string, string> WindowsToIana = new Dictionary<string, string>();

			// Get from here:
			// https://raw.githubusercontent.com/unicode-org/cldr/master/common/supplemental/windowsZones.xml
			// https://github.com/unicode-org/cldr/blob/main/common/supplemental/windowsZones.xml

			var windowsZonesStream = http.GetStreamAsync(@"https://raw.githubusercontent.com/unicode-org/cldr/master/common/supplemental/windowsZones.xml").GetAwaiter().GetResult();
			var doc = XElement.Load(windowsZonesStream);

			foreach (var ele in doc.Element("windowsZones").Element("mapTimezones").Elements("mapZone"))
			{
				var win = ele.Attributes("other").Single().Value;
				var iana = ele.Attributes("type").Single().Value.Trim(); // trim because was ending with a space
				var terr = ele.Attributes("territory").Single().Value;
				if (terr == "001")
				{
					WindowsToIana.Add(win, iana);
				}
				else
				{
					foreach (var ianaSplit in iana.Split(' '))
					{
						IanaToWindows.Add(ianaSplit, win);
					}
				}
			}

			// Aliases:
			// https://raw.githubusercontent.com/unicode-org/cldr/master/common/bcp47/timezone.xml
			// https://github.com/unicode-org/cldr/blob/main/common/bcp47/timezone.xml
			var timezoneStream = http.GetStreamAsync(@"https://raw.githubusercontent.com/unicode-org/cldr/master/common/bcp47/timezone.xml").GetAwaiter().GetResult();

			doc = XElement.Load(timezoneStream);
			foreach (var ele in doc.Element("keyword").Element("key").Elements("type"))
			{
				var aliasA = ele.Attributes("alias").SingleOrDefault();//.Value;
				if (aliasA == null)
					continue;
				var alias = aliasA.Value;

				string? main = null;
				foreach (var alia in alias.Split(' '))
				{
					if (IanaToWindows.TryGetValue(alia, out var lol))
					{
						main = lol;
						break;
					}
				}

				if (main == null)
					continue; // Troll
							  //throw new Exception();

				foreach (var alia in alias.Split(' '))
				{
					if (!IanaToWindows.ContainsKey(alia))
					{
						IanaToWindows.Add(alia, main);
					}
				}
			}

			ff.WriteLine("\t// This map is generated by a tool. Do not modify. " + DateTime.UtcNow + "Z");
			ff.WriteLine("\t\tstatic readonly Dictionary<string, string> _ianaToWindows = new()");
			ff.WriteLine("\t\t{");
			foreach (var kv in IanaToWindows.OrderBy(kv => kv.Key))
				ff.WriteLine("\t\t\t{\"" + kv.Key + "\", \"" + kv.Value + "\"},");
			ff.WriteLine("\t\t};");

			ff.WriteLine();

			ff.WriteLine("\t// This map is generated by a tool. Do not modify. " + DateTime.UtcNow + "Z");
			ff.WriteLine("\t\tstatic readonly Dictionary<string, string> _windowsToIana = new()");
			ff.WriteLine("\t\t{");
			foreach (var kv in WindowsToIana.OrderBy(kv => kv.Value))
				ff.WriteLine("\t\t\t{\"" + kv.Key + "\", \"" + kv.Value + "\"},");
			ff.WriteLine("\t\t};");

			ff.Close();

			http.Dispose();
		}

		
	}


}
