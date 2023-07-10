using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using System.Text.Json.Nodes;

namespace CosmosTime.Serialization.SystemTextJson
{
	/// <summary>
	/// Format\parse composite format 
	/// { 
	///   "timeUtc": "yyyy-MM-ddTHH:mm:ss.fffffffZ", // fixed length utc
	///   "offsetMins": -30
	/// }
	/// </summary>
	public class UtcOffsetTimeCosmosDbJsonConverter : JsonConverter<UtcOffsetTime>
	{
		public override UtcOffsetTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			//var obj = JObject.Load(reader);
			var obj = JsonObject.Parse(ref reader);
			
			return UtcOffsetTime.ParseCosmosDb((string)obj["timeUtc"], (short)obj["offsetMinutes"]);
		}

		public override void Write(Utf8JsonWriter writer, UtcOffsetTime value, JsonSerializerOptions options)
		{
		
			var rr = (UtcOffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName("timeUtc");
			writer.WriteStringValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName("offsetMinutes");
			writer.WriteNumberValue(rr.OffsetMinutes);

			writer.WriteEndObject();
		}
	}
}
