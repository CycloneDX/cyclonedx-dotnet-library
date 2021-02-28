using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Json;

namespace CycloneDX.Json.Tests
{
    public class ValidationTests
    {
        [Theory]
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
        public async Task ValidJsonTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.2.json");
            var jsonBom = File.ReadAllText(resourceFilename);

            var validationResult = await JsonBomValidator.Validate(jsonBom, Models.SchemaVersion.v1_2);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-bomformat")]
        [InlineData("invalid-component-ref")]
        [InlineData("invalid-component-swid")]
        [InlineData("invalid-component-type")]
        [InlineData("invalid-dependency")]
        [InlineData("invalid-empty-component")]
        [InlineData("invalid-hash-alg")]
        [InlineData("invalid-hash-md5")]
        [InlineData("invalid-hash-sha1")]
        [InlineData("invalid-hash-sha256")]
        [InlineData("invalid-hash-sha512")]
        [InlineData("invalid-issue-type")]
        [InlineData("invalid-license-choice")]
        [InlineData("invalid-license-encoding")]
        [InlineData("invalid-license-id")]
        [InlineData("invalid-metadata-timestamp")]
        [InlineData("invalid-missing-component-type")]
        [InlineData("invalid-patch-type")]
        [InlineData("invalid-scope")]
        [InlineData("invalid-serialnumber")]
        [InlineData("invalid-service-data")]
        [InlineData("invalid-specversion")]
        public async Task InvalidJsonTest_v1_2(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.2.json");
            var jsonBom = File.ReadAllText(resourceFilename);

            var validationResult = await JsonBomValidator.Validate(jsonBom, Models.SchemaVersion.v1_2);

            Assert.False(validationResult.Valid);
        }

    }
}
