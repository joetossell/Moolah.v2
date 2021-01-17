using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moolah.Monzo.Client.Serialization
{
    public class EmptyDateTimeOffsetOffsetToNullConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.HasValueSequence && reader.ValueSequence.Length == 0 
                || !reader.HasValueSequence && reader.ValueSpan.Length == 0)
            { 
                return null;
            }
            return reader.GetDateTimeOffset();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if(value == null) writer.WriteNullValue();
            else writer.WriteStringValue((DateTimeOffset) value);
        }
    }
}
