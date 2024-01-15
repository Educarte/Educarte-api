using Api.Infrastructure.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Infrastructure.Converters
{
    public class StringSensitiveContentConverter : JsonConverter<SensitiveContent<string>>
    {
        public override SensitiveContent<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new SensitiveContent<string>
            {
                Value = reader.GetString(),
            };
        }

        public override void Write(Utf8JsonWriter writer, SensitiveContent<string> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("**REDACT**");
        }
    }
}
