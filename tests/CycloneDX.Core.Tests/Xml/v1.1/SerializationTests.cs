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
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Core.Tests.Xml.v1_1
{
    public class SerializationTests
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
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }

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
        public void XmlRoundTripStreamTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.1", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            using var ms = new MemoryStream();
            Serializer.Serialize(bom, ms);

            Snapshot.Match(Encoding.UTF8.GetString(ms.ToArray()), SnapshotNameExtension.Create(filename));
        }

        // TODO: modify test data to conform to v1.0 spec, i.e. component types, hash algorithms, and component modified
        // [Theory]
        // [InlineData("valid-bom-1.1.xml")]
        // [InlineData("valid-component-hashes-1.1.xml")]
        // [InlineData("valid-component-ref-1.1.xml")]
        // [InlineData("valid-component-types-1.1.xml")]
        // [InlineData("valid-empty-components-1.1.xml")]
        // // [InlineData("valid-external-elements-1.1.xml")]
        // [InlineData("valid-license-expression-1.1.xml")]
        // [InlineData("valid-license-id-1.1.xml")]
        // [InlineData("valid-license-name-1.1.xml")]
        // [InlineData("valid-minimal-viable-1.1.xml")]
        // [InlineData("valid-random-attributes-1.1.xml")]
        // [InlineData("valid-xml-signature-1.1.xml")]
        // public void XmlDowngradeTest(string filename)
        // {
        //     var resourceFilename = Path.Join("Resources", "v1.1", filename);
        //     var xmlBom = File.ReadAllText(resourceFilename);

        //     var bom = Serializer.Deserialize(xmlBom);
        //     bom.SpecVersion = SpecificationVersion.v1_0;
        //     xmlBom = Serializer.Serialize(bom);

        //     var result = Validator.Validate(xmlBom, SpecificationVersion.v1_0);

        //     Assert.True(result.Valid, $"BOM version downgrade failed validation: Validation failed: {result}");
        // }
    }
}
