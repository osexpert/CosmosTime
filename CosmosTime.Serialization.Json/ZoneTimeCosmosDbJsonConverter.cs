using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CosmosTime.TimeZone;

namespace CosmosTime.Serialization.Json
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

            string utcTime = (string?)obj?[_timeUtcName] ?? throw new InvalidOperationException(_timeUtcName);
            short offsetMinutes = (short?)obj?[_offsetMinutesName] ?? throw new InvalidOperationException(_offsetMinutesName);
            string tzIana = (string?)obj?[_tzIanaName] ?? throw new InvalidOperationException(_tzIanaName);

            return ZoneTime.ParseCosmosDb(utcTime, TimeSpan.FromMinutes(offsetMinutes), tzIana);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ZoneTime zt, JsonSerializerOptions options)
        {
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
