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
using System.Text;
using CycloneDX.Models;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;
using System.Linq;

namespace CycloneDX.Core.Tests.Xml.v1_6
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-annotation-1.6.xml")]
        [InlineData("valid-assembly-1.6.xml")]
        [InlineData("valid-attestation-1.6.xml")]
        [InlineData("valid-bom-1.6.xml")]
        [InlineData("valid-component-hashes-1.6.xml")]
        [InlineData("valid-component-identifiers-1.6.xml")]
        [InlineData("valid-component-ref-1.6.xml")]
        [InlineData("valid-component-swid-1.6.xml")]
        [InlineData("valid-component-swid-full-1.6.xml")]
        [InlineData("valid-component-types-1.6.xml")]
        [InlineData("valid-compositions-1.6.xml")]
        [InlineData("valid-cryptography-full-1.6.xml")]
        [InlineData("valid-cryptography-implementation-1.6.xml")]
        [InlineData("valid-dependency-1.6.xml")]
        [InlineData("valid-empty-components-1.6.xml")]
        [InlineData("valid-evidence-1.6.xml")]
        [InlineData("valid-external-elements-1.6.xml")]
        [InlineData("valid-external-reference-1.6.xml")]
        [InlineData("valid-formulation-1.6.xml")]
        [InlineData("valid-license-expression-1.6.xml")]
        [InlineData("valid-license-id-1.6.xml")]
        [InlineData("valid-license-licensing-1.6.xml")]
        [InlineData("valid-license-name-1.6.xml")]
        [InlineData("valid-machine-learning-1.6.xml")]
        [InlineData("valid-machine-learning-considerations-env-1.6.xml")]
        [InlineData("valid-metadata-author-1.6.xml")]
        [InlineData("valid-metadata-license-1.6.xml")]
        [InlineData("valid-metadata-lifecycle-1.6.xml")]
        [InlineData("valid-metadata-manufacture-1.6.xml")]
        [InlineData("valid-metadata-manufacturer-1.6.xml")]
        [InlineData("valid-metadata-supplier-1.6.xml")]
        [InlineData("valid-metadata-timestamp-1.6.xml")]
        [InlineData("valid-metadata-tool-1.6.xml")]
        [InlineData("valid-metadata-tool-deprecated-1.6.xml")]
        [InlineData("valid-minimal-viable-1.6.xml")]
        [InlineData("valid-patch-1.6.xml")]
        [InlineData("valid-properties-1.6.xml")]
        [InlineData("valid-random-attributes-1.6.xml")]
        [InlineData("valid-release-notes-1.6.xml")]
        [InlineData("valid-saasbom-1.6.xml")]
        [InlineData("valid-service-1.6.xml")]
        [InlineData("valid-service-empty-objects-1.6.xml")]
        [InlineData("valid-standard-1.6.xml")]
        [InlineData("valid-tags-1.6.xml")]
        [InlineData("valid-vulnerability-1.6.xml")]
        [InlineData("valid-xml-signature-1.6.xml")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.6", filename);
            var xmlBomInput = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBomInput);
            var xmlBomOutput = Serializer.Serialize(bom);


            var validationResult = Validator.Validate(xmlBomOutput, SpecificationVersion.v1_6);

            Assert.True(validationResult.Valid, validationResult.Messages?.FirstOrDefault());

            Snapshot.Match(xmlBomOutput, SnapshotNameExtension.Create(filename));
        }
        
        [Theory]
        [InlineData("valid-annotation-1.6.xml")]
        [InlineData("valid-assembly-1.6.xml")]
        [InlineData("valid-attestation-1.6.xml")]
        [InlineData("valid-bom-1.6.xml")]
        [InlineData("valid-component-hashes-1.6.xml")]
        [InlineData("valid-component-identifiers-1.6.xml")]
        [InlineData("valid-component-ref-1.6.xml")]
        [InlineData("valid-component-swid-1.6.xml")]
        [InlineData("valid-component-swid-full-1.6.xml")]
        [InlineData("valid-component-types-1.6.xml")]
        [InlineData("valid-compositions-1.6.xml")]
        [InlineData("valid-cryptography-full-1.6.xml")]
        [InlineData("valid-cryptography-implementation-1.6.xml")]
        [InlineData("valid-dependency-1.6.xml")]
        [InlineData("valid-empty-components-1.6.xml")]
        [InlineData("valid-evidence-1.6.xml")]
        [InlineData("valid-external-elements-1.6.xml")]
        [InlineData("valid-external-reference-1.6.xml")]
        [InlineData("valid-formulation-1.6.xml")]
        [InlineData("valid-license-expression-1.6.xml")]
        [InlineData("valid-license-id-1.6.xml")]
        [InlineData("valid-license-licensing-1.6.xml")]
        [InlineData("valid-license-name-1.6.xml")]
        [InlineData("valid-machine-learning-1.6.xml")]
        [InlineData("valid-machine-learning-considerations-env-1.6.xml")]
        [InlineData("valid-metadata-author-1.6.xml")]
        [InlineData("valid-metadata-license-1.6.xml")]
        [InlineData("valid-metadata-lifecycle-1.6.xml")]
        [InlineData("valid-metadata-manufacture-1.6.xml")]
        [InlineData("valid-metadata-manufacturer-1.6.xml")]
        [InlineData("valid-metadata-supplier-1.6.xml")]
        [InlineData("valid-metadata-timestamp-1.6.xml")]
        [InlineData("valid-metadata-tool-1.6.xml")]
        [InlineData("valid-metadata-tool-deprecated-1.6.xml")]
        [InlineData("valid-minimal-viable-1.6.xml")]
        [InlineData("valid-patch-1.6.xml")]
        [InlineData("valid-properties-1.6.xml")]
        [InlineData("valid-random-attributes-1.6.xml")]
        [InlineData("valid-release-notes-1.6.xml")]
        [InlineData("valid-saasbom-1.6.xml")]
        [InlineData("valid-service-1.6.xml")]
        [InlineData("valid-service-empty-objects-1.6.xml")]
        [InlineData("valid-standard-1.6.xml")]
        [InlineData("valid-tags-1.6.xml")]
        [InlineData("valid-vulnerability-1.6.xml")]
        [InlineData("valid-xml-signature-1.6.xml")]
        public void XmlRoundTripStreamTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.6", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            using var ms = new MemoryStream();
            Serializer.Serialize(bom, ms);
            Snapshot.Match(Encoding.UTF8.GetString(ms.ToArray()), SnapshotNameExtension.Create(filename));
        }

        [Theory]
        [InlineData("valid-annotation-1.6.xml")]
        [InlineData("valid-assembly-1.6.xml")]
        [InlineData("valid-attestation-1.6.xml")]
        [InlineData("valid-bom-1.6.xml")]
        [InlineData("valid-component-hashes-1.6.xml")]
        [InlineData("valid-component-identifiers-1.6.xml")]
        [InlineData("valid-component-ref-1.6.xml")]
        [InlineData("valid-component-swid-1.6.xml")]
        [InlineData("valid-component-swid-full-1.6.xml")]
        [InlineData("valid-component-types-1.6.xml")]
        [InlineData("valid-compositions-1.6.xml")]
        [InlineData("valid-cryptography-full-1.6.xml")]
        [InlineData("valid-cryptography-implementation-1.6.xml")]
        [InlineData("valid-dependency-1.6.xml")]
        [InlineData("valid-empty-components-1.6.xml")]
        [InlineData("valid-evidence-1.6.xml")]
        [InlineData("valid-external-elements-1.6.xml")]
        [InlineData("valid-external-reference-1.6.xml")]
        [InlineData("valid-formulation-1.6.xml")]
        [InlineData("valid-license-expression-1.6.xml")]
        [InlineData("valid-license-id-1.6.xml")]
        [InlineData("valid-license-licensing-1.6.xml")]
        [InlineData("valid-license-name-1.6.xml")]
        [InlineData("valid-machine-learning-1.6.xml")]
        [InlineData("valid-machine-learning-considerations-env-1.6.xml")]
        [InlineData("valid-metadata-author-1.6.xml")]
        [InlineData("valid-metadata-license-1.6.xml")]
        [InlineData("valid-metadata-lifecycle-1.6.xml")]
        [InlineData("valid-metadata-manufacture-1.6.xml")]
        [InlineData("valid-metadata-manufacturer-1.6.xml")]
        [InlineData("valid-metadata-supplier-1.6.xml")]
        [InlineData("valid-metadata-timestamp-1.6.xml")]
        [InlineData("valid-metadata-tool-1.6.xml")]
        [InlineData("valid-metadata-tool-deprecated-1.6.xml")]
        [InlineData("valid-minimal-viable-1.6.xml")]
        [InlineData("valid-patch-1.6.xml")]
        [InlineData("valid-properties-1.6.xml")]
        [InlineData("valid-random-attributes-1.6.xml")]
        [InlineData("valid-release-notes-1.6.xml")]
        [InlineData("valid-saasbom-1.6.xml")]
        [InlineData("valid-service-1.6.xml")]
        [InlineData("valid-service-empty-objects-1.6.xml")]
        [InlineData("valid-standard-1.6.xml")]
        [InlineData("valid-tags-1.6.xml")]
        [InlineData("valid-vulnerability-1.6.xml")]
        [InlineData("valid-xml-signature-1.6.xml")]
        public void XmlDowngradeTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.6", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            bom.SpecVersion = SpecificationVersion.v1_5;
            xmlBom = Serializer.Serialize(bom);


            var result = Validator.Validate(xmlBom, SpecificationVersion.v1_5);

            Assert.True(result.Valid, $"BOM version downgrade failed validation: Validation failed: {result}");
        }
    }
}
