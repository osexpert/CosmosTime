using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosTime.Serialization.JsonNet
{
	/// <summary>
	/// Format\parse composite format 
	/// { 
	///   "timeUtc": "yyyy-MM-ddTHH:mm:ss.fffffffZ", // fixed length utc
	///   "offsetMinutes": -30
	/// }
	/// </summary>
	public class UtcOffsetTimeCosmosDbJsonConverter : JsonConverter<UtcOffsetTime>
	{
		public override UtcOffsetTime ReadJson(JsonReader reader, Type objectType, UtcOffsetTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var obj = JObject.Load(reader);
			return UtcOffsetTime.ParseCosmosDb(obj["timeUtc"].Value<string>(), obj["offsetMinutes"].Value<short>());
		}

		public override void WriteJson(JsonWriter writer, UtcOffsetTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var rr = (UtcOffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName("timeUtc");
			writer.WriteValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName("offsetMinutes");
			writer.WriteValue(rr.OffsetMinutes);

			writer.WriteEndObject();
		}
	}
}
