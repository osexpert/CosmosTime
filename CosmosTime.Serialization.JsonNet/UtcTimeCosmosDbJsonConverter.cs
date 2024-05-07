using System;
using Newtonsoft.Json;

namespace CosmosTime.Serialization.JsonNet
{
    /// <summary>
    /// Format\parse fixed length utc "yyyy-MM-ddTHH:mm:ss.fffffffZ"
    /// </summary>
    public class UtcTimeCosmosDbJsonConverter : JsonConverter<UtcTime>
    {
        /// <inheritdoc/>
        public override UtcTime ReadJson(JsonReader reader, Type objectType, UtcTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            return UtcTime.ParseCosmosDb((string?)reader.Value ?? throw new InvalidOperationException());
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, UtcTime value, JsonSerializer serializer)
        {
            if (serializer.DateParseHandling != DateParseHandling.None)
                throw new NotSupportedException("DateParseHandling.None required");

            var utc = (UtcTime)value;

            writer.WriteValue(utc.ToCosmosDb());
        }
    }
}
