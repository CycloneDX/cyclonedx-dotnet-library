using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Json;

namespace CycloneDX.Tests.Json.v1_2
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-assembly-1.2.json")]
        [InlineData("valid-bom-1.2.json")]
        [InlineData("valid-component-hashes-1.2.json")]
        [InlineData("valid-component-ref-1.2.json")]
        [InlineData("valid-component-swid-1.2.json")]
        [InlineData("valid-component-swid-full-1.2.json")]
        [InlineData("valid-component-types-1.2.json")]
        [InlineData("valid-dependency-1.2.json")]
        [InlineData("valid-empty-components-1.2.json")]
        [InlineData("valid-license-expression-1.2.json")]
        [InlineData("valid-license-id-1.2.json")]
        [InlineData("valid-license-name-1.2.json")]
        [InlineData("valid-metadata-author-1.2.json")]
        [InlineData("valid-metadata-manufacture-1.2.json")]
        [InlineData("valid-metadata-supplier-1.2.json")]
        [InlineData("valid-metadata-timestamp-1.2.json")]
        [InlineData("valid-metadata-tool-1.2.json")]
        [InlineData("valid-minimal-viable-1.2.json")]
        [InlineData("valid-patch-1.2.json")]
        [InlineData("valid-service-1.2.json")]
        [InlineData("valid-service-empty-objects-1.2.json")]
        public void JsonRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.2", filename);
            var jsonBom = File.ReadAllText(resourceFilename);

            var bom = Deserializer.Deserialize_v1_2(jsonBom);
            jsonBom = Serializer.Serialize(bom);

            Snapshot.Match(jsonBom, SnapshotNameExtension.Create(filename));
        }
    }
}
