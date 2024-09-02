using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class UnderscoreConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumValue = reader.GetString();

            string normalizedEnumValue = enumValue.Replace("-", "_");

            if (Enum.TryParse(normalizedEnumValue, out T result))
            {
                return result;
            }

            throw new JsonException($"Invalid enum value: {enumValue}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            string jsonValue = value.ToString().Replace("_", "-");
            writer.WriteStringValue(jsonValue);
        }
    }
}
