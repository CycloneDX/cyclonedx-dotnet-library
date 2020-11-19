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
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-types")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-name")]
        [InlineData("valid-minimal-viable")]
        public void XmlRoundTripTest_v1_1(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename + "-1.1.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = XmlBomDeserializer.Deserialize_v1_1(xmlBom);
            xmlBom = XmlBomSerializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }

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
        public void XmlRoundTripTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.2", filename + "-1.2.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = XmlBomDeserializer.Deserialize_v1_2(xmlBom);
            xmlBom = XmlBomSerializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
