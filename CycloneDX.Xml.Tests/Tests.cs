using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Xml.Tests
{
    public class XmlBomDeserializerTests
    {
        [Theory] 
        [InlineData("bom")]
        [InlineData("valid-component-hashes-1.2")]
        [InlineData("valid-component-ref-1.2")]
        [InlineData("valid-component-swid-1.2")]
        [InlineData("valid-component-swid-full-1.2")]
        [InlineData("valid-component-types-1.2")]
        [InlineData("valid-dependency-1.2")]
        [InlineData("valid-empty-components-1.2")]
        [InlineData("valid-license-expression-1.2")]
        [InlineData("valid-license-id-1.2")]
        [InlineData("valid-license-name-1.2")]
        [InlineData("valid-metadata-author-1.2")]
        [InlineData("valid-metadata-manufacture-1.2")]
        [InlineData("valid-metadata-supplier-1.2")]
        [InlineData("valid-metadata-timestamp-1.2")]
        [InlineData("valid-metadata-tool-1.2")]
        [InlineData("valid-minimal-viable-1.2")]
        [InlineData("valid-patch-1.2")]
        [InlineData("valid-service-1.2")]
        [InlineData("valid-service-empty-objects-1.2")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + ".xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = XmlBomDeserializer.Deserialize(xmlBom);
            xmlBom = XmlBomSerializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
