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
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX;

namespace CycloneDX.Tests.Protobuf.v1_3
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-assembly-1.3.xml")]
        [InlineData("valid-bom-1.3.xml")]
        [InlineData("valid-component-hashes-1.3.xml")]
        [InlineData("valid-component-ref-1.3.xml")]
        [InlineData("valid-component-swid-1.3.xml")]
        [InlineData("valid-component-swid-full-1.3.xml")]
        [InlineData("valid-component-types-1.3.xml")]
        [InlineData("valid-compositions-1.3.xml")]
        [InlineData("valid-dependency-1.3.xml")]
        [InlineData("valid-empty-components-1.3.xml")]
        // [InlineData("valid-evidence-1.3.xml")]
        // [InlineData("valid-external-elements-1.3.xml")]
        [InlineData("valid-external-reference-1.3.xml")]
        [InlineData("valid-license-expression-1.3.xml")]
        [InlineData("valid-license-id-1.3.xml")]
        [InlineData("valid-license-name-1.3.xml")]
        [InlineData("valid-metadata-author-1.3.xml")]
        [InlineData("valid-metadata-license-1.3.xml")]
        [InlineData("valid-metadata-manufacture-1.3.xml")]
        [InlineData("valid-metadata-supplier-1.3.xml")]
        [InlineData("valid-metadata-timestamp-1.3.xml")]
        [InlineData("valid-metadata-tool-1.3.xml")]
        [InlineData("valid-minimal-viable-1.3.xml")]
        [InlineData("valid-patch-1.3.xml")]
        [InlineData("valid-properties-1.3.xml")]
        // [InlineData("valid-random-attributes-1.3.xml")]
        [InlineData("valid-service-1.3.xml")]
        [InlineData("valid-service-empty-objects-1.3.xml")]
        // [InlineData("valid-xml-signature-1.3.xml")]
        public void ProtobufRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.3", filename);
            var xmlBom = File.ReadAllText(resourceFilename);
            var inputBom = CycloneDX.Xml.Deserializer.Deserialize_v1_3(xmlBom);

            var stream = new MemoryStream();
            CycloneDX.Protobuf.Serializer.Serialize(inputBom, stream);
            var outputBom = CycloneDX.Protobuf.Deserializer.Deserialize(stream);
            xmlBom = CycloneDX.Xml.Serializer.Serialize(outputBom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
