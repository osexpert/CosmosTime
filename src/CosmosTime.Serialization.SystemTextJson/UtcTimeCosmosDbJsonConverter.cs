using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CosmosTime.Serialization.SystemTextJson
{

	/// <summary>
	/// Format\parse fixed length utc "yyyy-MM-ddTHH:mm:ss.fffffffZ"
	/// </summary>
	public class UtcTimeCosmosDbJsonConverter : JsonConverter<UtcTime>
	{
		/// <inheritdoc/>
		public override UtcTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return UtcTime.ParseCosmosDb(reader.GetString());
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, UtcTime value, JsonSerializerOptions options)
		{
			var utc = (UtcTime)value;

			writer.WriteStringValue(utc.ToCosmosDb());
		}
	}
}
