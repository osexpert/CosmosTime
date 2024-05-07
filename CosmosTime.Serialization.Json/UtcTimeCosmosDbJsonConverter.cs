using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosmosTime.Serialization.Json
{

    /// <summary>
    /// Format\parse fixed length utc "yyyy-MM-ddTHH:mm:ss.fffffffZ"
    /// </summary>
    public class UtcTimeCosmosDbJsonConverter : JsonConverter<UtcTime>
    {
        /// <inheritdoc/>
        public override UtcTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return UtcTime.ParseCosmosDb(reader.GetString() ?? throw new InvalidOperationException());
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, UtcTime utc, JsonSerializerOptions options)
        {
            writer.WriteStringValue(utc.ToCosmosDb());
        }
    }
}
