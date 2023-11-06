using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using CosmosTime.TimeZone;

namespace CosmosTime.Serialization.SystemTextJson
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
		string _timeUtcName;
		string _offsetMinutesName;
		string _tzIanaName;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="timeUtcName"></param>
		/// <param name="offsetMinutesName"></param>
		/// <param name="tzIanaName"></param>
		public ZoneTimeCosmosDbJsonConverter(string timeUtcName = "timeUtc", string offsetMinutesName = "offsetMinutes", string tzIanaName = "tzIana")
		{
			_timeUtcName = timeUtcName;
			_offsetMinutesName = offsetMinutesName;
			_tzIanaName = tzIanaName;
		}

		/// <inheritdoc/>
		public override ZoneTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			//var obj = JObject.Load(reader);
			var obj = JsonObject.Parse(ref reader);

			return ZoneTime.ParseCosmosDb((string)obj[_timeUtcName], TimeSpan.FromMinutes((short)obj[_offsetMinutesName]), (string)obj[_tzIanaName]);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, ZoneTime value, JsonSerializerOptions options)
		{

			var zt = (ZoneTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName(_timeUtcName);
			writer.WriteStringValue(zt.OffsetTime.UtcTime.ToCosmosDb());

			writer.WritePropertyName(_offsetMinutesName);
			writer.WriteNumberValue(Shared.GetWholeMinutes(zt.OffsetTime.Offset));

			writer.WritePropertyName(_tzIanaName);
			writer.WriteStringValue(IanaTimeZone.GetIanaId(zt.Zone));

			writer.WriteEndObject();
		}
	}
}
