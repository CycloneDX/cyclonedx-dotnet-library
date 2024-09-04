using System.IO;
using Xunit;
using CycloneDX.Spdx.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;
using CycloneDX.Spdx.Models.v2_3;
using Snapshooter;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace CycloneDX.Spdx.Tests
{
    public class UnderscoreConverterTests
    {

        private static JsonSerializerOptions _options;
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }


        [Fact]
        public void underscoreDeserializer()
        {
            var resourceFilename = Path.Join("Resources", "v2.3", "underscoreConvert.json");
            var document = System.IO.File.ReadAllText(resourceFilename);
            _options = GetJsonSerializerOptions();

            var checksum = System.Text.Json.JsonSerializer.Deserialize<Checksum>(document, _options);
            Assert.Equal(ChecksumAlgorithm.SHA3_256, checksum.Algorithm);
            Assert.Equal("d6a770ba38583ed4bb4525bd96e50461655d2759", checksum.ChecksumValue);
        }

        [Fact]
        public void underscoreSerializer()
        {
            var resourceFilename = Path.Join("Resources", "v2.3", "underscoreConvertResult.json");
            _options = GetJsonSerializerOptions();

            Checksum expected = new Checksum();
            expected.Algorithm = ChecksumAlgorithm.SHA3_256;
            expected.ChecksumValue = "d6a770ba38583ed4bb4525bd96e50461655d2759";

            var jsonString = System.Text.Json.JsonSerializer.Serialize(expected, _options);
            System.IO.File.WriteAllText(resourceFilename, jsonString);
            var document = System.IO.File.ReadAllText(resourceFilename);
            var actual = System.Text.Json.JsonSerializer.Deserialize<Checksum>(document, _options);
            Assert.Equal(expected.Algorithm, actual.Algorithm);
            Assert.Equal(expected.ChecksumValue, actual.ChecksumValue);
        }

        //private static JsonSerializerSettings _settings;

        //public static JsonSerializerSettings GetJsonSerializerSettings()
        //{
        //    var settings = new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented, // Similar to WriteIndented in System.Text.Json
        //        ContractResolver = new CamelCasePropertyNamesContractResolver(), // Similar to PropertyNamingPolicy.CamelCase
        //        NullValueHandling = NullValueHandling.Ignore // Similar to DefaultIgnoreCondition.WhenWritingNull
        //    };
        //    settings.Converters.Add(new StringEnumConverter()); // Similar to JsonStringEnumConverter in System.Text.Json
        //    return settings;
        //}


        //[Fact]
        //public void underscoreDeserializer()
        //{
        //    var resourceFilename = Path.Join("Resources", "v2.3", "underscoreConvert.json");
        //    var document = System.IO.File.ReadAllText(resourceFilename);
        //    _settings = GetJsonSerializerSettings();

        //    var checksum = JsonConvert.DeserializeObject<Checksum>(document, _settings);
        //    Assert.Equal(ChecksumAlgorithm.SHA1, checksum.Algorithm);
        //    Assert.Equal("d6a770ba38583ed4bb4525bd96e50461655d2759", checksum.ChecksumValue);
        //}

        //[Fact]
        //public void underscoreSerializer()
        //{
        //    var resourceFilename = Path.Join("Resources", "v2.3", "underscoreConvertResult.json");
        //    _settings = GetJsonSerializerSettings();

        //    Checksum expected = new Checksum();
        //    expected.Algorithm = ChecksumAlgorithm.SHA3_256;
        //    expected.ChecksumValue = "d6a770ba38583ed4bb4525bd96e50461655d2759";

        //    var jsonString = JsonConvert.SerializeObject(expected, _settings);
        //    System.IO.File.WriteAllText(resourceFilename, jsonString);
        //    var document = System.IO.File.ReadAllText(resourceFilename);
        //    var actual = JsonConvert.DeserializeObject<Checksum>(document, _settings);

        //    Assert.Equal(expected.Algorithm, actual.Algorithm);
        //    Assert.Equal(expected.ChecksumValue, actual.ChecksumValue);
        //}
    }
}

