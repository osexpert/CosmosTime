using System;
using CosmosTime.TimeZone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosTime.JsonNet
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
        public override ZoneTime ReadJson(JsonReader reader, Type objectType, ZoneTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            var obj = JObject.Load(reader);

            string utcTime = obj[_timeUtcName]?.Value<string>() ?? throw new InvalidOperationException(_timeUtcName);
            short offsetMinutes = obj[_offsetMinutesName]?.Value<short>() ?? throw new InvalidOperationException(_offsetMinutesName);
            string tzIana = obj[_tzIanaName]?.Value<string>() ?? throw new InvalidOperationException(_tzIanaName);

            return ZoneTime.ParseCosmosDb(
                utcTime,
                TimeSpan.FromMinutes(offsetMinutes),
                tzIana);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, ZoneTime zt, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            writer.WriteStartObject();

            writer.WritePropertyName(_timeUtcName);
            writer.WriteValue(zt.OffsetTime.UtcTime.ToCosmosDb());

            writer.WritePropertyName(_offsetMinutesName);
            writer.WriteValue(Shared.GetWholeMinutes(zt.OffsetTime.Offset));

            writer.WritePropertyName(_tzIanaName);
            writer.WriteValue(IanaTimeZone.GetIanaId(zt.Zone));

            writer.WriteEndObject();
        }
    }
}
