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
	public class OffsetTimeCosmosDbJsonConverter : JsonConverter<OffsetTime>
	{
		/// <inheritdoc/>
		public override OffsetTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			//var obj = JObject.Load(reader);
			var obj = JsonObject.Parse(ref reader);
			
			return OffsetTime.ParseCosmosDb((string)obj["timeUtc"], TimeSpan.FromMinutes((short)obj["offsetMinutes"]));
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, OffsetTime value, JsonSerializerOptions options)
		{
		
			var rr = (OffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName("timeUtc");
			writer.WriteStringValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName("offsetMinutes");
			writer.WriteNumberValue(Shared.GetWholeMinutes(rr.Offset));

			writer.WriteEndObject();
		}
	}
}
