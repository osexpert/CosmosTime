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
	public class OffsetTimeCosmosDbJsonConverter : JsonConverter<OffsetTime>
	{
		/// <inheritdoc/>
		public override OffsetTime ReadJson(JsonReader reader, Type objectType, OffsetTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var obj = JObject.Load(reader);
			return OffsetTime.ParseCosmosDb(obj["timeUtc"].Value<string>(), TimeSpan.FromMinutes(obj["offsetMinutes"].Value<short>()));
		}

		/// <inheritdoc/>
		public override void WriteJson(JsonWriter writer, OffsetTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var rr = (OffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName("timeUtc");
			writer.WriteValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName("offsetMinutes");
			writer.WriteValue(Shared.GetWholeMinutes(rr.Offset));

			writer.WriteEndObject();
		}
	}
}
