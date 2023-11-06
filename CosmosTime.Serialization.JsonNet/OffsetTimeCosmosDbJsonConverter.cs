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
		string _utcTimeName;
		string _offsetMinutesName;

        /// <summary>
		/// TODO
		/// </summary>
		/// <param name="utcTimeName"></param>
		/// <param name="offsetMinutesName"></param>
		public OffsetTimeCosmosDbJsonConverter(string utcTimeName = "timeUtc", string offsetMinutesName = "offsetMinutes")
        {
			_utcTimeName = utcTimeName;
			_offsetMinutesName = offsetMinutesName;
        }

        /// <inheritdoc/>
        public override OffsetTime ReadJson(JsonReader reader, Type objectType, OffsetTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var obj = JObject.Load(reader);
			return OffsetTime.ParseCosmosDb(obj[_utcTimeName].Value<string>(), TimeSpan.FromMinutes(obj[_offsetMinutesName].Value<short>()));
		}

		/// <inheritdoc/>
		public override void WriteJson(JsonWriter writer, OffsetTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var rr = (OffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName(_utcTimeName);
			writer.WriteValue(rr.UtcTime.ToCosmosDb());

			writer.WritePropertyName(_offsetMinutesName);
			writer.WriteValue(Shared.GetWholeMinutes(rr.Offset));

			writer.WriteEndObject();
		}
	}
}
