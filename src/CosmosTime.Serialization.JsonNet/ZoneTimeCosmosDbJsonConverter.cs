using System;
using System.Collections.Generic;
using System.Text;
using CosmosTime.TimeZone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosTime.Serialization.JsonNet
{
	/// <summary>
	/// Format\parse composite format 
	/// { 
	///   "timeUtc": "yyyy-MM-ddTHH:mm:ss.fffffffZ", // fixed length utc
	///   "offsetMinutes": -30,
	///   "tzIana": "Europe/Berlin"
	/// }
	/// </summary>
	public class ZoneTimeCosmosDbJsonConverter : JsonConverter<ZoneTime>
	{
		string _utcTimeName;
		string _offsetMinutesName;
		string _tzIanaName;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="utcTimeName"></param>
		/// <param name="offsetMinutesName"></param>
		/// <param name="tzIanaName"></param>
		public ZoneTimeCosmosDbJsonConverter(string utcTimeName = "timeUtc", string offsetMinutesName = "offsetMinutes", string tzIanaName = "tzIana")
		{
			_utcTimeName = utcTimeName;
			_offsetMinutesName = offsetMinutesName;
			_tzIanaName = tzIanaName;
		}

		/// <inheritdoc/>
		public override ZoneTime ReadJson(JsonReader reader, Type objectType, ZoneTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var obj = JObject.Load(reader);
			return ZoneTime.ParseCosmosDb(
				obj[_utcTimeName].Value<string>(), 
				TimeSpan.FromMinutes(obj[_offsetMinutesName].Value<short>()),
				obj[_tzIanaName].Value<string>());
		}

		/// <inheritdoc/>
		public override void WriteJson(JsonWriter writer, ZoneTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var zt = (ZoneTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName(_utcTimeName);
			writer.WriteValue(zt.OffsetTime.UtcTime.ToCosmosDb());

			writer.WritePropertyName(_offsetMinutesName);
			writer.WriteValue(Shared.GetWholeMinutes(zt.OffsetTime.Offset));

			writer.WritePropertyName(_tzIanaName);
			writer.WriteValue(IanaTimeZone.GetIanaId(zt.Zone));

			writer.WriteEndObject();
		}
	}
}
