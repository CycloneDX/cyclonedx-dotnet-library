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
using CycloneDX.Json;

namespace CycloneDX.Core.Tests.Json.v1_5
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("valid-annotation-1.5.json")]
        [InlineData("valid-assembly-1.5.json")]
        [InlineData("valid-bom-1.5.json")]
        [InlineData("valid-component-hashes-1.5.json")]
        [InlineData("valid-component-ref-1.5.json")]
        [InlineData("valid-component-swid-1.5.json")]
        [InlineData("valid-component-swid-full-1.5.json")]
        [InlineData("valid-component-types-1.5.json")]
        [InlineData("valid-compositions-1.5.json")]
        [InlineData("valid-dependency-1.5.json")]
        [InlineData("valid-empty-components-1.5.json")]
        [InlineData("valid-evidence-1.5.json")]
        [InlineData("valid-external-reference-1.5.json")]
        [InlineData("valid-formulation-1.5.json")]
        [InlineData("valid-license-expression-1.5.json")]
        [InlineData("valid-license-id-1.5.json")]
        [InlineData("valid-license-licensing-1.5.json")]
        [InlineData("valid-license-name-1.5.json")]
        [InlineData("valid-machine-learning-1.5.json")]
        [InlineData("valid-metadata-author-1.5.json")]
        [InlineData("valid-metadata-license-1.5.json")]
        [InlineData("valid-metadata-lifecycle-1.5.json")]
        [InlineData("valid-metadata-manufacture-1.5.json")]
        [InlineData("valid-metadata-supplier-1.5.json")]
        [InlineData("valid-metadata-timestamp-1.5.json")]
        [InlineData("valid-metadata-tool-1.5.json")]
        [InlineData("valid-metadata-tool-deprecated-1.5.json")]
        [InlineData("valid-minimal-viable-1.5.json")]
        [InlineData("valid-patch-1.5.json")]
        [InlineData("valid-properties-1.5.json")]
        [InlineData("valid-release-notes-1.5.json")]
        [InlineData("valid-saasbom-1.5.json")]
        [InlineData("valid-service-1.5.json")]
        [InlineData("valid-service-empty-objects-1.5.json")]
        [InlineData("valid-signatures-1.5.json")]
        [InlineData("valid-vulnerability-1.5.json")]
        public void ValidateJsonStringTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            var jsonString = File.ReadAllText(resourceFilename);

            var validationResult = Validator.Validate(jsonString, SpecificationVersion.v1_5);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("valid-annotation-1.5.json")]
        [InlineData("valid-assembly-1.5.json")]
        [InlineData("valid-bom-1.5.json")]
        [InlineData("valid-component-hashes-1.5.json")]
        [InlineData("valid-component-ref-1.5.json")]
        [InlineData("valid-component-swid-1.5.json")]
        [InlineData("valid-component-swid-full-1.5.json")]
        [InlineData("valid-component-types-1.5.json")]
        [InlineData("valid-compositions-1.5.json")]
        [InlineData("valid-dependency-1.5.json")]
        [InlineData("valid-empty-components-1.5.json")]
        [InlineData("valid-evidence-1.5.json")]
        [InlineData("valid-external-reference-1.5.json")]
        [InlineData("valid-formulation-1.5.json")]
        [InlineData("valid-license-expression-1.5.json")]
        [InlineData("valid-license-id-1.5.json")]
        [InlineData("valid-license-licensing-1.5.json")]
        [InlineData("valid-license-name-1.5.json")]
        [InlineData("valid-machine-learning-1.5.json")]
        [InlineData("valid-metadata-author-1.5.json")]
        [InlineData("valid-metadata-license-1.5.json")]
        [InlineData("valid-metadata-lifecycle-1.5.json")]
        [InlineData("valid-metadata-manufacture-1.5.json")]
        [InlineData("valid-metadata-supplier-1.5.json")]
        [InlineData("valid-metadata-timestamp-1.5.json")]
        [InlineData("valid-metadata-tool-1.5.json")]
        [InlineData("valid-metadata-tool-deprecated-1.5.json")]
        [InlineData("valid-minimal-viable-1.5.json")]
        [InlineData("valid-patch-1.5.json")]
        [InlineData("valid-properties-1.5.json")]
        [InlineData("valid-release-notes-1.5.json")]
        [InlineData("valid-saasbom-1.5.json")]
        [InlineData("valid-service-1.5.json")]
        [InlineData("valid-service-empty-objects-1.5.json")]
        [InlineData("valid-signatures-1.5.json")]
        [InlineData("valid-vulnerability-1.5.json")]
        public async Task ValidateJsonStreamTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            using (var jsonStream = File.OpenRead(resourceFilename))
            {
                var validationResult = await Validator.ValidateAsync(jsonStream, SpecificationVersion.v1_5).ConfigureAwait(false);

                Assert.True(validationResult.Valid);
            }
        }

        [Theory]
        [InlineData("invalid-bomformat-1.5.json")]
        [InlineData("invalid-component-ref-1.5.json")]
        [InlineData("invalid-component-swid-1.5.json")]
        [InlineData("invalid-component-type-1.5.json")]
        [InlineData("invalid-dependency-1.5.json")]
        [InlineData("invalid-empty-component-1.5.json")]
        [InlineData("invalid-hash-alg-1.5.json")]
        [InlineData("invalid-hash-md5-1.5.json")]
        [InlineData("invalid-hash-sha1-1.5.json")]
        [InlineData("invalid-hash-sha256-1.5.json")]
        [InlineData("invalid-hash-sha512-1.5.json")]
        [InlineData("invalid-issue-type-1.5.json")]
        [InlineData("invalid-license-choice-1.5.json")]
        [InlineData("invalid-license-encoding-1.5.json")]
        [InlineData("invalid-license-id-1.5.json")]
        [InlineData("invalid-metadata-license-1.5.json")]
        [InlineData("invalid-metadata-timestamp-1.5.json")]
        [InlineData("invalid-missing-component-type-1.5.json")]
        [InlineData("invalid-patch-type-1.5.json")]
        [InlineData("invalid-scope-1.5.json")]
        [InlineData("invalid-serialnumber-1.5.json")]
        [InlineData("invalid-service-data-1.5.json")]
        public void InvalidJsonTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            var jsonBom = File.ReadAllText(resourceFilename);

            var validationResult = Validator.Validate(jsonBom, SpecificationVersion.v1_5);

            Assert.False(validationResult.Valid);
        }
    }
}
