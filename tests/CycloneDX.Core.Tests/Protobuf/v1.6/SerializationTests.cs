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
using Snapshooter;
using Snapshooter.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace CycloneDX.Core.Tests.Protobuf.v1_6
{
    [Collection("Protoc Serialization")]
    public class SerializationTests
    {
        private readonly ITestOutputHelper output;

        public SerializationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // I can't be bothered setting up protoc in the github workflow for all platforms
        // if anyone wants to have a crack at it please go for it
        [LinuxOnlyForCITheory]
        [InlineData("valid-annotation-1.6.textproto")]
        [InlineData("valid-assembly-1.6.textproto")]
        [InlineData("valid-attestation-1.6.textproto")]
        [InlineData("valid-bom-1.6.textproto")]
        [InlineData("valid-component-data-1.6.textproto")]
        [InlineData("valid-component-hashes-1.6.textproto")]
        [InlineData("valid-component-identifiers-1.6.textproto")]
        [InlineData("valid-component-ref-1.6.textproto")]
        [InlineData("valid-component-swid-1.6.textproto")]
        [InlineData("valid-component-swid-full-1.6.textproto")]
        [InlineData("valid-component-types-1.6.textproto")]
        [InlineData("valid-compositions-1.6.textproto")]
        [InlineData("valid-cryptography-full-1.6.textproto")]
        [InlineData("valid-cryptography-implementation-1.6.textproto")]
        [InlineData("valid-dependency-1.6.textproto")]
        [InlineData("valid-empty-components-1.6.textproto")]
        [InlineData("valid-evidence-1.6.textproto")]
        [InlineData("valid-external-reference-1.6.textproto")]
        [InlineData("valid-formulation-1.6.textproto")]
        [InlineData("valid-license-expression-1.6.textproto")]
        [InlineData("valid-license-id-1.6.textproto")]
        [InlineData("valid-license-licensing-1.6.textproto")]
        [InlineData("valid-license-name-1.6.textproto")]
        [InlineData("valid-machine-learning-1.6.textproto")]
        [InlineData("valid-machine-learning-considerations-env-1.6.textproto")]
        [InlineData("valid-metadata-author-1.6.textproto")]
        [InlineData("valid-metadata-license-1.6.textproto")]
        [InlineData("valid-metadata-lifecycle-1.6.textproto")]
        [InlineData("valid-metadata-manufacture-1.6.textproto")]
        [InlineData("valid-metadata-manufacturer-1.6.textproto")]
        [InlineData("valid-metadata-supplier-1.6.textproto")]
        [InlineData("valid-metadata-timestamp-1.6.textproto")]
        [InlineData("valid-metadata-tool-1.6.textproto")]
        [InlineData("valid-metadata-tool-deprecated-1.6.textproto")]
        [InlineData("valid-minimal-viable-1.6.textproto")]
        [InlineData("valid-patch-1.6.textproto")]
        [InlineData("valid-properties-1.6.textproto")]
        [InlineData("valid-release-notes-1.6.textproto")]
        [InlineData("valid-saasbom-1.6.textproto")]
        [InlineData("valid-service-1.6.textproto")]
        [InlineData("valid-service-empty-objects-1.6.textproto")]
        [InlineData("valid-standard-1.6.textproto")]
        [InlineData("valid-tags-1.6.textproto")]
        [InlineData("valid-vulnerability-1.6.textproto")]
        public void ProtobufDeserializationTest(string filename)
        {
            using (var tempDir = new CycloneDX.Core.Tests.Protobuf.TempDirectoryWithProtoSchemas())
            {
                var protobufResourceFilename = Path.Join("Resources", "v1.6", filename);
                var protobufTextString = File.ReadAllText(protobufResourceFilename);

                var runner = new ProtocRunner();
                var result = runner.Run(tempDir.DirectoryPath, protobufTextString, new[]
                {
                    "--proto_path=./",
                    "--encode=cyclonedx.v1_6.Bom",
                    "bom-1.6.proto"
                });

                if (result.ExitCode == 0)
                {
                    result.Output.Seek(0, SeekOrigin.Begin);
                    var bom = CycloneDX.Protobuf.Serializer.Deserialize(result.Output);

                    Snapshot.Match(bom, SnapshotNameExtension.Create(filename));
                }
                else
                {
                    output.WriteLine(result.Errors);
                    Assert.Equal(0, result.ExitCode);
                }
            }
        }
    }
}
