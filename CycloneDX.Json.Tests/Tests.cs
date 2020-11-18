using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Json;

namespace CycloneDX.Json.Tests
{
    public class Tests
    {
        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-hashes")]
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-swid")]
        [InlineData("valid-component-swid-full")]
        [InlineData("valid-component-types")]
        [InlineData("valid-dependency")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-name")]
        [InlineData("valid-metadata-author")]
        [InlineData("valid-metadata-manufacture")]
        [InlineData("valid-metadata-supplier")]
        [InlineData("valid-metadata-timestamp")]
        [InlineData("valid-metadata-tool")]
        [InlineData("valid-minimal-viable")]
        [InlineData("valid-patch")]
        [InlineData("valid-service")]
        [InlineData("valid-service-empty-objects")]
        public void JsonRoundTripTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.2.json");
            var jsonBom = File.ReadAllText(resourceFilename);

            var bom = JsonBomDeserializer.Deserialize_v1_2(jsonBom);
            jsonBom = JsonBomSerializer.Serialize(bom);

            Snapshot.Match(jsonBom, SnapshotNameExtension.Create(filename));
        }
    }
}
