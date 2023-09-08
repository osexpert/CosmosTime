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
	///   "offsetMinutes": -30
	/// }
	/// </summary>
	public class OffsetTimeCosmosDbJsonConverter : JsonConverter<OffsetTime>
	{
		string _timeUtcName;
		string _offsetMinutesName;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="timeUtcName"></param>
		/// <param name="offsetMinutesName"></param>
		public OffsetTimeCosmosDbJsonConverter(string timeUtcName = "timeUtc", string offsetMinutesName = "offsetMinutes")
        {
			_timeUtcName = timeUtcName;
			_offsetMinutesName = offsetMinutesName;
		}

        /// <inheritdoc/>
        public override OffsetTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			//var obj = JObject.Load(reader);
			var obj = JsonObject.Parse(ref reader);
			
			return OffsetTime.ParseCosmosDb((string)obj[_timeUtcName], TimeSpan.FromMinutes((short)obj[_offsetMinutesName]));
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, OffsetTime value, JsonSerializerOptions options)
		{
		
			var rr = (OffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName(_timeUtcName);
			writer.WriteStringValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName(_offsetMinutesName);
			writer.WriteNumberValue(Shared.GetWholeMinutes(rr.Offset));

			writer.WriteEndObject();
		}
	}
}
