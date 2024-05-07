using System;
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
        public override OffsetTime ReadJson(JsonReader reader, Type objectType, OffsetTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            var obj = JObject.Load(reader);

            string utcTime = obj[_timeUtcName]?.Value<string>() ?? throw new InvalidOperationException(_timeUtcName);
            short offsetMinutes = obj[_offsetMinutesName]?.Value<short>() ?? throw new InvalidOperationException(_offsetMinutesName);

            return OffsetTime.ParseCosmosDb(utcTime, TimeSpan.FromMinutes(offsetMinutes));
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, OffsetTime ot, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            writer.WriteStartObject();

            writer.WritePropertyName(_timeUtcName);
            writer.WriteValue(ot.UtcTime.ToCosmosDb());

            writer.WritePropertyName(_offsetMinutesName);
            writer.WriteValue(Shared.GetWholeMinutes(ot.Offset));

            writer.WriteEndObject();
        }
    }
}
