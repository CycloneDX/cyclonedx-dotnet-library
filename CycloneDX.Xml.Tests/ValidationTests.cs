using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Xml.Tests
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-hashes")]
        public async Task ValidXmlTest_v1_0(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.0.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await XmlBomValidator.Validate(xmlBom, Models.SchemaVersion.v1_0);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-hashes")]
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-types")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-name")]
        [InlineData("valid-minimal-viable")]
        public async Task ValidXmlTest_v1_1(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.1.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await XmlBomValidator.Validate(xmlBom, Models.SchemaVersion.v1_1);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-component-ref")]
        [InlineData("invalid-component-type")]
        [InlineData("invalid-empty-component")]
        [InlineData("invalid-hash-alg")]
        [InlineData("invalid-hash-md5")]
        [InlineData("invalid-hash-sha1")]
        [InlineData("invalid-hash-sha256")]
        [InlineData("invalid-hash-sha512")]
        [InlineData("invalid-license-choice")]
        [InlineData("invalid-license-encoding")]
        [InlineData("invalid-license-id")]
        [InlineData("invalid-license-id-count")]
        [InlineData("invalid-license-name-count")]
        [InlineData("invalid-missing-component-type")]
        [InlineData("invalid-namespace")]
        [InlineData("invalid-serialnumber")]
        [InlineData("invalid-scope")]
        public async Task InvalidXmlTest_v1_1(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.1.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await XmlBomValidator.Validate(xmlBom, Models.SchemaVersion.v1_1);

            Assert.False(validationResult.Valid);
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
        public async Task ValidXmlTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.2.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await XmlBomValidator.Validate(xmlBom, Models.SchemaVersion.v1_2);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-service-data")]
        [InlineData("invalid-metadata-timestamp")]
        [InlineData("invalid-dependency")]
        [InlineData("invalid-component-swid")]
        [InlineData("invalid-patch-type")]
        [InlineData("invalid-issue-type")]

        public async Task InvalidXmlTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.2.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await XmlBomValidator.Validate(xmlBom, Models.SchemaVersion.v1_2);

            Assert.False(validationResult.Valid);
        }

    }
}
