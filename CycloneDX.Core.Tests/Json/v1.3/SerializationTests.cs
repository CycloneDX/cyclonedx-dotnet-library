using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Json;

namespace CycloneDX.Tests.Json.v1_3
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-assembly-1.3.json")]
        [InlineData("valid-bom-1.3.json")]
        [InlineData("valid-component-hashes-1.3.json")]
        [InlineData("valid-component-ref-1.3.json")]
        [InlineData("valid-component-swid-1.3.json")]
        [InlineData("valid-component-swid-full-1.3.json")]
        [InlineData("valid-component-types-1.3.json")]
        [InlineData("valid-compositions-1.3.json")]
        [InlineData("valid-dependency-1.3.json")]
        [InlineData("valid-empty-components-1.3.json")]
        [InlineData("valid-evidence-1.3.json")]
        [InlineData("valid-license-expression-1.3.json")]
        [InlineData("valid-license-id-1.3.json")]
        [InlineData("valid-license-name-1.3.json")]
        [InlineData("valid-metadata-author-1.3.json")]
        [InlineData("valid-metadata-license-1.3.json")]
        [InlineData("valid-metadata-manufacture-1.3.json")]
        [InlineData("valid-metadata-supplier-1.3.json")]
        [InlineData("valid-metadata-timestamp-1.3.json")]
        [InlineData("valid-metadata-tool-1.3.json")]
        [InlineData("valid-minimal-viable-1.3.json")]
        [InlineData("valid-patch-1.3.json")]
        [InlineData("valid-properties-1.3.json")]
        [InlineData("valid-service-1.3.json")]
        [InlineData("valid-service-empty-objects-1.3.json")]
        public void JsonRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.3", filename);
            var jsonBom = File.ReadAllText(resourceFilename);

            var bom = Deserializer.Deserialize_v1_3(jsonBom);
            jsonBom = Serializer.Serialize(bom);

            Snapshot.Match(jsonBom, SnapshotNameExtension.Create(filename));
        }
    }
}
