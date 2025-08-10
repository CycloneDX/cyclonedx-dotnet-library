// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the “License”);
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an “AS IS” BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Spdx.Validation;

namespace CycloneDX.Spdx.Tests
{
    public class JsonValidatorTests
    {
        [Theory]
        [InlineData("v2.2", "document")]
        [InlineData("v2.2", "document-with-hyphens-in-external-reference-category")]
        [InlineData("v2.3", "document")]
        [InlineData("v2.3", "document-with-hyphens-in-external-reference-category")]
        public void ValidateJsonStringTest(string version, string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", version, baseFilename + ".json");
            var document = File.ReadAllText(resourceFilename);

            var result = JsonValidator.Validate(document);

            Assert.True(result.Valid);
        }

        [Theory]
        [InlineData("v2.2", "document")]
        [InlineData("v2.2", "document-with-hyphens-in-external-reference-category")]
        [InlineData("v2.3", "document")]
        [InlineData("v2.3", "document-with-hyphens-in-external-reference-category")]
        public async Task ValidateJsonStreamTest(string version, string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", version, baseFilename + ".json");
            using (var jsonStream = File.OpenRead(resourceFilename))
            {
                var validationResult = await JsonValidator.ValidateAsync(jsonStream).ConfigureAwait(false);

                Assert.True(validationResult.Valid);
            }
        }

        [Fact]
        public async Task ValidateInvalidPrimaryPackagePurpose()
        {
            var resourceFilename = Path.Join("Resources", "v2.3", "invalidPrimaryPackagePurpose" + ".json");
            using (var jsonStream = File.OpenRead(resourceFilename))
            {
                var validationResult = await JsonValidator.ValidateAsync(jsonStream).ConfigureAwait(false);

                Assert.False(validationResult.Valid);
            }
        }



    }
}
