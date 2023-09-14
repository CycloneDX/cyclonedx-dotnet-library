using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CycloneDX.Json;

namespace CycloneDX.Core.Tests.Json
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("valid-assembly-1.5.json", "v1.5")]
        [InlineData("valid-assembly-1.4.json", "v1.4")]
        [InlineData("valid-assembly-1.3.json", "v1.3")]
        [InlineData("valid-assembly-1.2.json", "v1.2")]
        public void ValidateJsonStringTest(string filename, string resourceVersion)
        {
            var resourceFilename = Path.Join("Resources", resourceVersion, filename);
            var jsonString = File.ReadAllText(resourceFilename);

            var validationResult = Validator.Validate(jsonString);

            Assert.True(validationResult.Valid);
        }
    }
}
