using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Infrastructure.Converters
{
    public class StreamConverter : JsonConverter<Stream>
    {
        public override Stream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(reader.GetString() ?? ""));
        }

        public override void Write(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("**BLOB**");
        }
    }
}
