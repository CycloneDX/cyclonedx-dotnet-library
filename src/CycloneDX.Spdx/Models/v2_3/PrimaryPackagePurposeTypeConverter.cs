using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class PrimaryPackagePurposeTypeConverter : JsonConverter<PrimaryPackagePurposeType>
    {
        public override PrimaryPackagePurposeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string primaryPackagePurpose = reader.GetString();

            string normalizedAlgorithm = primaryPackagePurpose.Replace("-", "_");

            if (Enum.TryParse(normalizedAlgorithm, out PrimaryPackagePurposeType result))
            {
                return result;
            }

            throw new JsonException($"Invalid checksum algorithm: {primaryPackagePurpose}");
        }

        public override void Write(Utf8JsonWriter writer, PrimaryPackagePurposeType value, JsonSerializerOptions options)
        {
            string jsonValue = value.ToString().Replace("_", "-");
            writer.WriteStringValue(jsonValue);
        }
    }
}
