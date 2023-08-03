namespace test7
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var tz44 = TimeZoneInfo.FindSystemTimeZoneById("Dateline Standard Time");
			var tz55 = TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12");

			//{ "Dateline Standard Time", "Etc/GMT+12"},


			Console.WriteLine("Hello, World!");
			var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
			var utz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
			var tzz = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
			var tzz2 = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
			var usstz = TimeZoneInfo.FindSystemTimeZoneById("lolz");
		}
	}
}