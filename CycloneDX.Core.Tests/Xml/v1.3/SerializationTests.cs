using System;
using System.IO;
using CycloneDX.Models.v1_3;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;
using Property = CycloneDX.Models.v1_3.Property;

namespace CycloneDX.Tests.Xml.v1_3
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-assembly-1.3.xml")]
        [InlineData("valid-bom-1.3.xml")]
        [InlineData("valid-component-hashes-1.3.xml")]
        [InlineData("valid-component-ref-1.3.xml")]
        [InlineData("valid-component-swid-1.3.xml")]
        [InlineData("valid-component-swid-full-1.3.xml")]
        [InlineData("valid-component-types-1.3.xml")]
        [InlineData("valid-compositions-1.3.xml")]
        [InlineData("valid-dependency-1.3.xml")]
        [InlineData("valid-empty-components-1.3.xml")]
        // [InlineData("valid-evidence-1.3.xml")]
        // [InlineData("valid-external-elements-1.3.xml")]
        [InlineData("valid-license-expression-1.3.xml")]
        [InlineData("valid-license-id-1.3.xml")]
        [InlineData("valid-license-name-1.3.xml")]
        [InlineData("valid-metadata-author-1.3.xml")]
        [InlineData("valid-metadata-license-1.3.xml")]
        [InlineData("valid-metadata-manufacture-1.3.xml")]
        [InlineData("valid-metadata-supplier-1.3.xml")]
        [InlineData("valid-metadata-timestamp-1.3.xml")]
        [InlineData("valid-metadata-tool-1.3.xml")]
        [InlineData("valid-minimal-viable-1.3.xml")]
        [InlineData("valid-patch-1.3.xml")]
        [InlineData("valid-properties-1.3.xml")]
        // [InlineData("valid-random-attributes-1.3.xml")]
        [InlineData("valid-service-1.3.xml")]
        [InlineData("valid-service-empty-objects-1.3.xml")]
        // [InlineData("valid-xml-signature-1.3.xml")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.3", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Deserializer.Deserialize_v1_3(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
