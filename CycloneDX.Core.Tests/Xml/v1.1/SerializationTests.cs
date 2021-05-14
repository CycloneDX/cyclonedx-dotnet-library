using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Tests.Xml.v1_1
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-bom-1.1.xml")]
        [InlineData("valid-component-hashes-1.1.xml")]
        [InlineData("valid-component-ref-1.1.xml")]
        [InlineData("valid-component-types-1.1.xml")]
        [InlineData("valid-empty-components-1.1.xml")]
        // [InlineData("valid-external-elements-1.1.xml")]
        [InlineData("valid-license-expression-1.1.xml")]
        [InlineData("valid-license-id-1.1.xml")]
        [InlineData("valid-license-name-1.1.xml")]
        [InlineData("valid-minimal-viable-1.1.xml")]
        // [InlineData("valid-random-attributes-1.1.xml")]
        // [InlineData("valid-xml-signature-1.1.xml")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Deserializer.Deserialize_v1_1(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
