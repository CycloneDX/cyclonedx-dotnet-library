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
using CycloneDX.Xml;

namespace CycloneDX.Core.Tests.Xml.v1_5
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("valid-annotation-1.5.xml")]
        [InlineData("valid-assembly-1.5.xml")]
        [InlineData("valid-bom-1.5.xml")]
        [InlineData("valid-component-hashes-1.5.xml")]
        [InlineData("valid-component-ref-1.5.xml")]
        [InlineData("valid-component-swid-1.5.xml")]
        [InlineData("valid-component-swid-full-1.5.xml")]
        [InlineData("valid-component-types-1.5.xml")]
        [InlineData("valid-compositions-1.5.xml")]
        [InlineData("valid-dependency-1.5.xml")]
        [InlineData("valid-empty-components-1.5.xml")]
        [InlineData("valid-evidence-1.5.xml")]
        [InlineData("valid-external-elements-1.5.xml")]
        [InlineData("valid-external-reference-1.5.xml")]
        [InlineData("valid-formulation-1.5.xml")]
        [InlineData("valid-license-expression-1.5.xml")]
        [InlineData("valid-license-id-1.5.xml")]
        [InlineData("valid-license-licensing-1.5.xml")]
        [InlineData("valid-license-name-1.5.xml")]
        [InlineData("valid-machine-learning-1.5.xml")]
        [InlineData("valid-metadata-author-1.5.xml")]
        [InlineData("valid-metadata-license-1.5.xml")]
        [InlineData("valid-metadata-lifecycle-1.5.xml")]
        [InlineData("valid-metadata-manufacture-1.5.xml")]
        [InlineData("valid-metadata-supplier-1.5.xml")]
        [InlineData("valid-metadata-timestamp-1.5.xml")]
        [InlineData("valid-metadata-tool-1.5.xml")]
        [InlineData("valid-metadata-tool-deprecated-1.5.xml")]
        [InlineData("valid-minimal-viable-1.5.xml")]
        [InlineData("valid-patch-1.5.xml")]
        [InlineData("valid-properties-1.5.xml")]
        [InlineData("valid-random-attributes-1.5.xml")]
        [InlineData("valid-release-notes-1.5.xml")]
        [InlineData("valid-saasbom-1.5.xml")]
        [InlineData("valid-service-1.5.xml")]
        [InlineData("valid-service-empty-objects-1.5.xml")]
        [InlineData("valid-vulnerability-1.5.xml")]
        [InlineData("valid-xml-signature-1.5.xml")]
        public void ValidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = Validator.Validate(xmlBom, SpecificationVersion.v1_5);

            Assert.True(validationResult.Valid);
        }

        [Theory]
        [InlineData("invalid-component-ref-1.5.xml")]
        [InlineData("invalid-component-swid-1.5.xml")]
        [InlineData("invalid-component-type-1.5.xml")]
        [InlineData("invalid-dependency-1.5.xml")]
        [InlineData("invalid-empty-component-1.5.xml")]
        [InlineData("invalid-hash-alg-1.5.xml")]
        [InlineData("invalid-hash-md5-1.5.xml")]
        [InlineData("invalid-hash-sha1-1.5.xml")]
        [InlineData("invalid-hash-sha256-1.5.xml")]
        [InlineData("invalid-hash-sha512-1.5.xml")]
        [InlineData("invalid-issue-type-1.5.xml")]
        [InlineData("invalid-license-choice-1.5.xml")]
        [InlineData("invalid-license-encoding-1.5.xml")]
        [InlineData("invalid-license-id-1.5.xml")]
        [InlineData("invalid-license-id-count-1.5.xml")]
        [InlineData("invalid-license-name-count-1.5.xml")]
        [InlineData("invalid-metadata-license-1.5.xml")]
        [InlineData("invalid-metadata-timestamp-1.5.xml")]
        [InlineData("invalid-missing-component-type-1.5.xml")]
        [InlineData("invalid-namespace-1.5.xml")]
        [InlineData("invalid-patch-type-1.5.xml")]
        [InlineData("invalid-scope-1.5.xml")]
        [InlineData("invalid-serialnumber-1.5.xml")]
        [InlineData("invalid-service-data-1.5.xml")]
        public void InvalidXmlTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var validationResult = Validator.Validate(xmlBom, SpecificationVersion.v1_5);

            Assert.False(validationResult.Valid);
        }

        [Theory]
        [InlineData("valid-annotation-1.5.xml")]
        [InlineData("valid-assembly-1.5.xml")]
        [InlineData("valid-bom-1.5.xml")]
        [InlineData("valid-component-hashes-1.5.xml")]
        [InlineData("valid-component-ref-1.5.xml")]
        [InlineData("valid-component-swid-1.5.xml")]
        [InlineData("valid-component-swid-full-1.5.xml")]
        [InlineData("valid-component-types-1.5.xml")]
        [InlineData("valid-compositions-1.5.xml")]
        [InlineData("valid-dependency-1.5.xml")]
        [InlineData("valid-empty-components-1.5.xml")]
        [InlineData("valid-evidence-1.5.xml")]
        [InlineData("valid-external-elements-1.5.xml")]
        [InlineData("valid-external-reference-1.5.xml")]
        [InlineData("valid-formulation-1.5.xml")]
        [InlineData("valid-license-expression-1.5.xml")]
        [InlineData("valid-license-id-1.5.xml")]
        [InlineData("valid-license-licensing-1.5.xml")]
        [InlineData("valid-license-name-1.5.xml")]
        [InlineData("valid-machine-learning-1.5.xml")]
        [InlineData("valid-metadata-author-1.5.xml")]
        [InlineData("valid-metadata-license-1.5.xml")]
        [InlineData("valid-metadata-lifecycle-1.5.xml")]
        [InlineData("valid-metadata-manufacture-1.5.xml")]
        [InlineData("valid-metadata-supplier-1.5.xml")]
        [InlineData("valid-metadata-timestamp-1.5.xml")]
        [InlineData("valid-metadata-tool-1.5.xml")]
        [InlineData("valid-metadata-tool-deprecated-1.5.xml")]
        [InlineData("valid-minimal-viable-1.5.xml")]
        [InlineData("valid-patch-1.5.xml")]
        [InlineData("valid-properties-1.5.xml")]
        [InlineData("valid-random-attributes-1.5.xml")]
        [InlineData("valid-release-notes-1.5.xml")]
        [InlineData("valid-saasbom-1.5.xml")]
        [InlineData("valid-service-1.5.xml")]
        [InlineData("valid-service-empty-objects-1.5.xml")]
        [InlineData("valid-vulnerability-1.5.xml")]
        [InlineData("valid-xml-signature-1.5.xml")]
        public void ValidateRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.5", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            var validationResult = Validator.Validate(xmlBom, SpecificationVersion.v1_5);

            Assert.True(validationResult.Valid, string.Join(Environment.NewLine, validationResult.Messages));
        }

    }
}
