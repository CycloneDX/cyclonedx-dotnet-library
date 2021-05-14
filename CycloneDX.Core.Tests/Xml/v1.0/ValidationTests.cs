using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Tests.Xml.v1_0
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("valid-bom-1.0.xml")]
        [InlineData("valid-component-hashes-1.0.xml")]

        public async Task ValidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.0", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = await Validator.Validate(xmlBom, SchemaVersion.v1_0);

            Assert.True(validationResult.Valid);
        }
    }
}
