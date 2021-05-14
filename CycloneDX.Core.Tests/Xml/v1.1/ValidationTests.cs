using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Tests.Xml.v1_1
{
    public class ValidationTests
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
        public async Task ValidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await Validator.Validate(xmlBom, SchemaVersion.v1_1);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-component-ref-1.1.xml")]
        [InlineData("invalid-component-type-1.1.xml")]
        [InlineData("invalid-empty-component-1.1.xml")]
        [InlineData("invalid-hash-alg-1.1.xml")]
        [InlineData("invalid-hash-md5-1.1.xml")]
        [InlineData("invalid-hash-sha1-1.1.xml")]
        [InlineData("invalid-hash-sha256-1.1.xml")]
        [InlineData("invalid-hash-sha512-1.1.xml")]
        [InlineData("invalid-license-choice-1.1.xml")]
        [InlineData("invalid-license-encoding-1.1.xml")]
        [InlineData("invalid-license-id-1.1.xml")]
        [InlineData("invalid-license-id-count-1.1.xml")]
        [InlineData("invalid-license-name-count-1.1.xml")]
        [InlineData("invalid-missing-component-type-1.1.xml")]
        [InlineData("invalid-namespace-1.1.xml")]
        [InlineData("invalid-scope-1.1.xml")]
        [InlineData("invalid-serialnumber-1.1.xml")]
        public async Task InvalidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await Validator.Validate(xmlBom, SchemaVersion.v1_1);

            Assert.False(validationResult.Valid);
        }
    }
}
