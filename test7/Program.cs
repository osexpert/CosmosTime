using CosmosTime.TimeZone;

namespace test7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var poison = TimeZoneInfo.FindSystemTimeZoneById("uTc");

            TimeZoneInfo.Utc.HasIanaId();

            var l = TimeZoneInfo.GetSystemTimeZones().Where(tz => tz.StandardName == "Coordinated Universal Time" || tz.StandardName == "UTC").ToList();

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
            var ttz = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            var utz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            var tzz = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
            var tzz2 = TimeZoneInfo.CreateCustomTimeZone("lolz", utz.BaseUtcOffset, utz.DisplayName, utz.StandardName, utz.DaylightName, utz.GetAdjustmentRules());
            //var usstz = TimeZoneInfo.FindSystemTimeZoneById("lolz");


        }
    }
}
