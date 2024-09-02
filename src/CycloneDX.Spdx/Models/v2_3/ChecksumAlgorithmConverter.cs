using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class ChecksumAlgorithmConverter : JsonConverter<ChecksumAlgorithm>
    {
        public override ChecksumAlgorithm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string algorithm = reader.GetString();

            string normalizedAlgorithm = algorithm.Replace("-", "_");

            if (Enum.TryParse(normalizedAlgorithm, out ChecksumAlgorithm result))
            {
                return result;
            }

            throw new JsonException($"Invalid checksum algorithm: {algorithm}");
        }

        public override void Write(Utf8JsonWriter writer, ChecksumAlgorithm value, JsonSerializerOptions options)
        {
            string jsonValue = value.ToString().Replace("_", "-");
            writer.WriteStringValue(jsonValue);
        }
    }
}
