using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Tests.Xml.v1_2
{
    public class ValidationTests
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
        public async Task ValidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.2", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await Validator.Validate(xmlBom, SchemaVersion.v1_2);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-component-ref-1.2.xml")]
        [InlineData("invalid-component-swid-1.2.xml")]
        [InlineData("invalid-component-type-1.2.xml")]
        [InlineData("invalid-dependency-1.2.xml")]
        [InlineData("invalid-empty-component-1.2.xml")]
        [InlineData("invalid-hash-alg-1.2.xml")]
        [InlineData("invalid-hash-md5-1.2.xml")]
        [InlineData("invalid-hash-sha1-1.2.xml")]
        [InlineData("invalid-hash-sha256-1.2.xml")]
        [InlineData("invalid-hash-sha512-1.2.xml")]
        [InlineData("invalid-issue-type-1.2.xml")]
        [InlineData("invalid-license-choice-1.2.xml")]
        [InlineData("invalid-license-encoding-1.2.xml")]
        [InlineData("invalid-license-id-1.2.xml")]
        [InlineData("invalid-license-id-count-1.2.xml")]
        [InlineData("invalid-license-name-count-1.2.xml")]
        [InlineData("invalid-metadata-timestamp-1.2.xml")]
        [InlineData("invalid-missing-component-type-1.2.xml")]
        [InlineData("invalid-namespace-1.2.xml")]
        [InlineData("invalid-patch-type-1.2.xml")]
        [InlineData("invalid-scope-1.2.xml")]
        [InlineData("invalid-serialnumber-1.2.xml")]
        [InlineData("invalid-service-data-1.2.xml")]

        public async Task InvalidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.2", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await Validator.Validate(xmlBom, SchemaVersion.v1_2);

            Assert.False(validationResult.Valid);
        }
    }
}
