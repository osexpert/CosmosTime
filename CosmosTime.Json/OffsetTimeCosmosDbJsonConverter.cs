using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CosmosTime.Json
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

            string utcTime = (string?)obj?[_timeUtcName] ?? throw new InvalidOperationException(_timeUtcName);
            short offsetMinutes = (short?)obj?[_offsetMinutesName] ?? throw new InvalidOperationException(_offsetMinutesName);

            return OffsetTime.ParseCosmosDb(utcTime, TimeSpan.FromMinutes(offsetMinutes));
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, OffsetTime ot, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(_timeUtcName);
            writer.WriteStringValue(ot.UtcTime.ToCosmosDb());

            writer.WritePropertyName(_offsetMinutesName);
            writer.WriteNumberValue(Shared.GetWholeMinutes(ot.Offset));

            writer.WriteEndObject();
        }
    }
}
