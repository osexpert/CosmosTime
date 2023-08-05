using CosmosTime.TimeZone;
using CosmosTime;

namespace test7
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var poison = TimeZoneInfo.FindSystemTimeZoneById("uTc");

			var list = TimeZoneInfo.GetSystemTimeZones().Where(tz => tz.DisplayName == "(UTC) Coordinated Universal Time" || tz.DisplayName == "UTC").ToList();
			var tz1 = list.Single(); // tz1.DisplayName: "(UTC) Coordinated Universal Time"
			var tz2 = TimeZoneInfo.Utc; // tz1.DisplayName: "UTC"
			var eq = tz1 == tz2; // false

			TimeZoneInfo.Utc.HasIanaId();

			var l = TimeZoneInfo.GetSystemTimeZones().Where(tz => tz.DisplayName == "(UTC) Coordinated Universal Time" || tz.DisplayName == "UTC").ToList();


			var tzIsUtc2 = TimeZoneInfo.FindSystemTimeZoneById("UTC") == TimeZoneInfo.Utc; // true
			var tzIsUtc3 = TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC") == TimeZoneInfo.Utc; // false
			var tzIsUtc4 = TimeZoneInfo.FindSystemTimeZoneById("Etc/Zulu") == TimeZoneInfo.Utc; // false
			var tzIsUtc5 = TimeZoneInfo.FindSystemTimeZoneById("Zulu") == TimeZoneInfo.Utc; // false

			var f = IanaTimeZone.GetTimeZoneInfo("South Africa Standard Time");

			var ggg = TimeZoneInfo.FindSystemTimeZoneById("Central Standard TIME");
			var ggg44 = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");

			var tz44 = TimeZoneInfo.FindSystemTimeZoneById("Dateline Standard Time");
			var tz55 = TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12");

			//{ "Dateline Standard Time", "Etc/GMT+12"},


			Console.WriteLine("Hello, World!");
			var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
			var utz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
			var tzz = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
			var tzz2 = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
			//var usstz = TimeZoneInfo.FindSystemTimeZoneById("lolz");

			
		}
	}
}
