using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;

namespace CycloneDX.Tests.Models.v1_3
{
    public class Tests
    {
        [Theory]
        [InlineData("valid-assembly-1.2.xml")]
        [InlineData("valid-bom-1.2.xml")]
        [InlineData("valid-component-hashes-1.2.xml")]
        [InlineData("valid-component-ref-1.2.xml")]
        [InlineData("valid-component-swid-1.2.xml")]
        [InlineData("valid-component-swid-full-1.2.xml")]
        [InlineData("valid-component-types-1.2.xml")]
        [InlineData("valid-dependency-1.2.xml")]
        [InlineData("valid-empty-components-1.2.xml")]
        // [InlineData("valid-external-elements-1.2.xml")]
        [InlineData("valid-license-expression-1.2.xml")]
        [InlineData("valid-license-id-1.2.xml")]
        [InlineData("valid-license-name-1.2.xml")]
        [InlineData("valid-metadata-author-1.2.xml")]
        [InlineData("valid-metadata-manufacture-1.2.xml")]
        [InlineData("valid-metadata-supplier-1.2.xml")]
        [InlineData("valid-metadata-timestamp-1.2.xml")]
        [InlineData("valid-metadata-tool-1.2.xml")]
        [InlineData("valid-minimal-viable-1.2.xml")]
        [InlineData("valid-patch-1.2.xml")]
        // [InlineData("valid-random-attributes-1.2.xml")]
        [InlineData("valid-service-1.2.xml")]
        [InlineData("valid-service-empty-objects-1.2.xml")]
        // [InlineData("valid-xml-signature-1.2.xml")]
        public void BomConversionTest_v1_2_to_v1_3_Test(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.2", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom =  CycloneDX.Xml.Deserializer.Deserialize_v1_2(xmlBom);
            var actualBom = new CycloneDX.Models.v1_3.Bom(bom);
            xmlBom =  CycloneDX.Xml.Serializer.Serialize(actualBom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
