using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime.TimeZone
{
    internal class MsSqlTimeZoneMap
    {
		public static string TIME_ZONE_ID_from_TIME_ZONE(string tz)
		{
			/// https://learn.microsoft.com/en-us/azure/azure-sql/managed-instance/timezones-overview
			
			// TODO: faster to use a Dictionary?

			switch (tz)
			{
				case "(UTC-12:00) International Date Line West": return "Dateline Standard Time";
				case "(UTC-11:00) Coordinated Universal Time-11": return "UTC-11";
				case "(UTC-10:00) Aleutian Islands": return "Aleutian Standard Time";
				case "(UTC-10:00) Hawaii": return "Hawaiian Standard Time";
				case "(UTC-09:30) Marquesas Islands": return "Marquesas Standard Time";
				case "(UTC-09:00) Alaska": return "Alaskan Standard Time";
				case "(UTC-09:00) Coordinated Universal Time-09": return "UTC-09";
				case "(UTC-08:00) Baja California": return "Pacific Standard Time (Mexico)";
				case "(UTC-08:00) Coordinated Universal Time-08": return "UTC-08";
				case "(UTC-08:00) Pacific Time (US & Canada)": return "Pacific Standard Time";
				case "(UTC-07:00) Arizona": return "US Mountain Standard Time";
				case "(UTC-07:00) Chihuahua, La Paz, Mazatlan": return "Mountain Standard Time (Mexico)";
				case "(UTC-07:00) Mountain Time (US & Canada)": return "Mountain Standard Time";
				case "(UTC-06:00) Central America": return "Central America Standard Time";
				case "(UTC-06:00) Central Time (US & Canada)": return "Central Standard Time";
				case "(UTC-06:00) Easter Island": return "Easter Island Standard Time";
				case "(UTC-06:00) Guadalajara, Mexico City, Monterrey": return "Central Standard Time (Mexico)";
				case "(UTC-06:00) Saskatchewan": return "Canada Central Standard Time";
				case "(UTC-05:00) Bogota, Lima, Quito, Rio Branco": return "SA Pacific Standard Time";
				case "(UTC-05:00) Chetumal": return "Eastern Standard Time (Mexico)";
				case "(UTC-05:00) Eastern Time (US & Canada)": return "Eastern Standard Time";
				case "(UTC-05:00) Haiti": return "Haiti Standard Time";
				case "(UTC-05:00) Havana": return "Cuba Standard Time";
				case "(UTC-05:00) Indiana (East)": return "US Eastern Standard Time";
				case "(UTC-05:00) Turks and Caicos": return "Turks And Caicos Standard Time";
				case "(UTC-04:00) Asuncion": return "Paraguay Standard Time";
				case "(UTC-04:00) Atlantic Time (Canada)": return "Atlantic Standard Time";
				case "(UTC-04:00) Caracas": return "Venezuela Standard Time";
				case "(UTC-04:00) Cuiaba": return "Central Brazilian Standard Time";
				case "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan": return "SA Western Standard Time";
				case "(UTC-04:00) Santiago": return "Pacific SA Standard Time";
				case "(UTC-03:30) Newfoundland": return "Newfoundland Standard Time";
				case "(UTC-03:00) Araguaina": return "Tocantins Standard Time";
				case "(UTC-03:00) Brasilia": return "E. South America Standard Time";
				case "(UTC-03:00) Cayenne, Fortaleza": return "SA Eastern Standard Time";
				case "(UTC-03:00) City of Buenos Aires": return "Argentina Standard Time";
				case "(UTC-03:00) Greenland": return "Greenland Standard Time";
				case "(UTC-03:00) Montevideo": return "Montevideo Standard Time";
				case "(UTC-03:00) Punta Arenas": return "Magallanes Standard Time";
				case "(UTC-03:00) Saint Pierre and Miquelon": return "Saint Pierre Standard Time";
				case "(UTC-03:00) Salvador": return "Bahia Standard Time";
				case "(UTC-02:00) Coordinated Universal Time-02": return "UTC-02";
				case "(UTC-02:00) Mid-Atlantic - Old": return "Mid-Atlantic Standard Time";
				case "(UTC-01:00) Azores": return "Azores Standard Time";
				case "(UTC-01:00) Cabo Verde Is.": return "Cabo Verde Standard Time";
				case "(UTC) Coordinated Universal Time": return "UTC";
				case "(UTC+00:00) Dublin, Edinburgh, Lisbon, London": return "GMT Standard Time";
				case "(UTC+00:00) Monrovia, Reykjavik": return "Greenwich Standard Time";
				case "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna": return "W. Europe Standard Time";
				case "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague": return "Central Europe Standard Time";
				case "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris": return "Romance Standard Time";
				case "(UTC+01:00) Casablanca": return "Morocco Standard Time";
				case "(UTC+01:00) Sao Tome": return "Sao Tome Standard Time";
				case "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb": return "Central European Standard Time";
				case "(UTC+01:00) West Central Africa": return "W. Central Africa Standard Time";
				case "(UTC+02:00) Amman": return "Jordan Standard Time";
				case "(UTC+02:00) Athens, Bucharest": return "GTB Standard Time";
				case "(UTC+02:00) Beirut": return "Middle East Standard Time";
				case "(UTC+02:00) Cairo": return "Egypt Standard Time";
				case "(UTC+02:00) Chisinau": return "E. Europe Standard Time";
				case "(UTC+02:00) Damascus": return "Syria Standard Time";
				case "(UTC+02:00) Gaza, Hebron": return "West Bank Standard Time";
				case "(UTC+02:00) Harare, Pretoria": return "South Africa Standard Time";
				case "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius": return "FLE Standard Time";
				case "(UTC+02:00) Jerusalem": return "Israel Standard Time";
				case "(UTC+02:00) Kaliningrad": return "Kaliningrad Standard Time";
				case "(UTC+02:00) Khartoum": return "Sudan Standard Time";
				case "(UTC+02:00) Tripoli": return "Libya Standard Time";
				case "(UTC+02:00) Windhoek": return "Namibia Standard Time";
				case "(UTC+03:00) Baghdad": return "Arabic Standard Time";
				case "(UTC+03:00) Istanbul": return "Türkiye Standard Time";
				case "(UTC+03:00) Kuwait, Riyadh": return "Arab Standard Time";
				case "(UTC+03:00) Minsk": return "Belarus Standard Time";
				case "(UTC+03:00) Moscow, St. Petersburg": return "Russian Standard Time";
				case "(UTC+03:00) Nairobi": return "E. Africa Standard Time";
				case "(UTC+03:30) Tehran": return "Iran Standard Time";
				case "(UTC+04:00) Abu Dhabi, Muscat": return "Arabian Standard Time";
				case "(UTC+04:00) Astrakhan, Ulyanovsk": return "Astrakhan Standard Time";
				case "(UTC+04:00) Baku": return "Azerbaijan Standard Time";
				case "(UTC+04:00) Izhevsk, Samara": return "Russia Time Zone 3";
				case "(UTC+04:00) Port Louis": return "Mauritius Standard Time";
				case "(UTC+04:00) Saratov": return "Saratov Standard Time";
				case "(UTC+04:00) Tbilisi": return "Georgian Standard Time";
				case "(UTC+04:00) Volgograd": return "Volgograd Standard Time";
				case "(UTC+04:00) Yerevan": return "Caucasus Standard Time";
				case "(UTC+04:30) Kabul": return "Afghanistan Standard Time";
				case "(UTC+05:00) Ashgabat, Tashkent": return "West Asia Standard Time";
				case "(UTC+05:00) Ekaterinburg": return "Ekaterinburg Standard Time";
				case "(UTC+05:00) Islamabad, Karachi": return "Pakistan Standard Time";
				case "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi": return "India Standard Time";
				case "(UTC+05:30) Sri Jayawardenepura": return "Sri Lanka Standard Time";
				case "(UTC+05:45) Kathmandu": return "Nepal Standard Time";
				case "(UTC+06:00) Nur-Sultan": return "Central Asia Standard Time";
				case "(UTC+06:00) Dhaka": return "Bangladesh Standard Time";
				case "(UTC+06:00) Omsk": return "Omsk Standard Time";
				case "(UTC+06:30) Yangon (Rangoon)": return "Myanmar Standard Time";
				case "(UTC+07:00) Bangkok, Hanoi, Jakarta": return "SE Asia Standard Time";
				case "(UTC+07:00) Barnaul, Gorno-Altaysk": return "Altai Standard Time";
				case "(UTC+07:00) Hovd": return "W. Mongolia Standard Time";
				case "(UTC+07:00) Krasnoyarsk": return "North Asia Standard Time";
				case "(UTC+07:00) Novosibirsk": return "N. Central Asia Standard Time";
				case "(UTC+07:00) Tomsk": return "Tomsk Standard Time";
				case "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi": return "China Standard Time";
				case "(UTC+08:00) Irkutsk": return "North Asia East Standard Time";
				case "(UTC+08:00) Kuala Lumpur, Singapore": return "Singapore Standard Time";
				case "(UTC+08:00) Perth": return "W. Australia Standard Time";
				case "(UTC+08:00) Taipei": return "Taipei Standard Time";
				case "(UTC+08:00) Ulaanbaatar": return "Ulaanbaatar Standard Time";
				case "(UTC+08:45) Eucla": return "Aus Central W. Standard Time";
				case "(UTC+09:00) Chita": return "Transbaikal Standard Time";
				case "(UTC+09:00) Osaka, Sapporo, Tokyo": return "Tokyo Standard Time";
				case "(UTC+09:00) Pyongyang": return "North Korea Standard Time";
				case "(UTC+09:00) Seoul": return "Korea Standard Time";
				case "(UTC+09:00) Yakutsk": return "Yakutsk Standard Time";
				case "(UTC+09:30) Adelaide": return "Cen. Australia Standard Time";
				case "(UTC+09:30) Darwin": return "AUS Central Standard Time";
				case "(UTC+10:00) Brisbane": return "E. Australia Standard Time";
				case "(UTC+10:00) Canberra, Melbourne, Sydney": return "AUS Eastern Standard Time";
				case "(UTC+10:00) Guam, Port Moresby": return "West Pacific Standard Time";
				case "(UTC+10:00) Hobart": return "Tasmania Standard Time";
				case "(UTC+10:00) Vladivostok": return "Vladivostok Standard Time";
				case "(UTC+10:30) Lord Howe Island": return "Lord Howe Standard Time";
				case "(UTC+11:00) Bougainville Island": return "Bougainville Standard Time";
				case "(UTC+11:00) Chokurdakh": return "Russia Time Zone 10";
				case "(UTC+11:00) Magadan": return "Magadan Standard Time";
				case "(UTC+11:00) Norfolk Island": return "Norfolk Standard Time";
				case "(UTC+11:00) Sakhalin": return "Sakhalin Standard Time";
				case "(UTC+11:00) Solomon Is., New Caledonia": return "Central Pacific Standard Time";
				case "(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky": return "Russia Time Zone 11";
				case "(UTC+12:00) Auckland, Wellington": return "New Zealand Standard Time";
				case "(UTC+12:00) Coordinated Universal Time+12": return "UTC+12";
				case "(UTC+12:00) Fiji": return "Fiji Standard Time";
				case "(UTC+12:00) Petropavlovsk-Kamchatsky - Old": return "Kamchatka Standard Time";
				case "(UTC+12:45) Chatham Islands": return "Chatham Islands Standard Time";
				case "(UTC+13:00) Coordinated Universal Time+13": return "UTC+13";
				case "(UTC+13:00) Nuku'alofa": return "Tonga Standard Time";
				case "(UTC+13:00) Samoa": return "Samoa Standard Time";
				case "(UTC+14:00) Kiritimati Island": return "Line Islands Standard Time";
			}

			throw new Exception("TimeZone not found: " + tz);
		}
	}
}
