using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX;
using CycloneDX.Xml;

namespace CycloneDX.Models.Tests
{
    public class ModelsTests
    {
        [Theory]
        [InlineData("bom")]
        public void BomConversionTest_v1_0_to_v1_1_Test(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.0", filename + "-1.0.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = XmlBomDeserializer.Deserialize_v1_0(xmlBom);
            var actualBom = new Models.v1_1.Bom(bom);
            xmlBom = XmlBomSerializer.Serialize(actualBom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }

        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-types")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-name")]
        [InlineData("valid-minimal-viable")]
        public void BomConversionTest_v1_1_to_v1_2_Test(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename + "-1.1.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = XmlBomDeserializer.Deserialize_v1_1(xmlBom);
            var actualBom = new Models.v1_2.Bom(bom);
            xmlBom = XmlBomSerializer.Serialize(actualBom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
