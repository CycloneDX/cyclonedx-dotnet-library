// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
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

namespace CycloneDX.Core.Tests.Protobuf.v1_7
{
    [Collection("Protoc Validation")]
    public class ValidationTests
    {
        private readonly ITestOutputHelper output;

        public ValidationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // I can't be bothered setting up protoc in the github workflow for all platforms
        // if anyone wants to have a crack at it please go for it
        [LinuxOnlyForCITheory]
        [InlineData("valid-annotation-1.7.json")]
        [InlineData("valid-assembly-1.7.json")]
        [InlineData("valid-attestation-1.7.json")]
        [InlineData("valid-bom-1.7.json")]
        [InlineData("valid-citations-1.7.json")]
        [InlineData("valid-component-data-1.7.json")]
        [InlineData("valid-component-external-with-version.json")]
        [InlineData("valid-component-external-with-versionRange.json")]
        [InlineData("valid-component-external-without-version.json")]
        [InlineData("valid-component-hashes-1.7.json")]
        [InlineData("valid-component-identifiers-1.7.json")]
        [InlineData("valid-component-ref-1.7.json")]
        [InlineData("valid-component-swid-1.7.json")]
        [InlineData("valid-component-swid-full-1.7.json")]
        [InlineData("valid-component-types-1.7.json")]
        [InlineData("valid-compositions-1.7.json")]
        [InlineData("valid-cryptography-certificate-1.7.json")]
        [InlineData("valid-cryptography-certificate-advanced-1.7.json")]
        [InlineData("valid-cryptography-full-1.7.json")]
        [InlineData("valid-cryptography-full-deprecated-1.7.json")]
        [InlineData("valid-cryptography-implementation-1.7.json")]
        [InlineData("valid-dependency-1.7.json")]
        [InlineData("valid-empty-components-1.7.json")]
        [InlineData("valid-evidence-1.7.json")]
        [InlineData("valid-external-reference-1.7.json")]
        [InlineData("valid-external-reference-properties-1.7.json")]
        [InlineData("valid-formulation-1.7.json")]
        [InlineData("valid-license-choice-1.7.json")]
        [InlineData("valid-license-declared-concluded-mix-1.7.json")]
        [InlineData("valid-license-expression-1.7.json")]
        [InlineData("valid-license-expression-with-licensing-1.7.json")]
        [InlineData("valid-license-expression-with-text-1.7.json")]
        [InlineData("valid-license-id-1.7.json")]
        [InlineData("valid-license-id-with-text-1.7.json")]
        [InlineData("valid-license-name-1.7.json")]
        [InlineData("valid-license-name-with-licensing-1.7.json")]
        [InlineData("valid-license-name-with-text-1.7.json")]
        [InlineData("valid-machine-learning-1.7.json")]
        [InlineData("valid-machine-learning-considerations-env-1.7.json")]
        [InlineData("valid-metadata-author-1.7.json")]
        [InlineData("valid-metadata-distribution-1.7.json")]
        [InlineData("valid-metadata-license-1.7.json")]
        [InlineData("valid-metadata-lifecycle-1.7.json")]
        [InlineData("valid-metadata-manufacture-1.7.json")]
        [InlineData("valid-metadata-manufacturer-1.7.json")]
        [InlineData("valid-metadata-supplier-1.7.json")]
        [InlineData("valid-metadata-timestamp-1.7.json")]
        [InlineData("valid-metadata-tool-1.7.json")]
        [InlineData("valid-metadata-tool-deprecated-1.7.json")]
        [InlineData("valid-minimal-viable-1.7.json")]
        [InlineData("valid-patch-1.7.json")]
        [InlineData("valid-patent-1.7.json")]
        [InlineData("valid-properties-1.7.json")]
        [InlineData("valid-release-notes-1.7.json")]
        [InlineData("valid-saasbom-1.7.json")]
        [InlineData("valid-service-1.7.json")]
        [InlineData("valid-service-empty-objects-1.7.json")]
        [InlineData("valid-signatures-1.7.json")]
        [InlineData("valid-standard-1.7.json")]
        [InlineData("valid-tags-1.7.json")]
        [InlineData("valid-vulnerability-1.7.json")]
        public void ValidProtobufTest(string filename)
        {
            using (var tempDir = new TempDirectoryWithProtoSchemas())
            {
                var resourceFilename = Path.Join("Resources", "v1.7", filename);
                var jsonBom = File.ReadAllText(resourceFilename);
                var inputBom = CycloneDX.Json.Serializer.Deserialize(jsonBom);

                var stream = new MemoryStream();
                CycloneDX.Protobuf.Serializer.Serialize(inputBom, stream);

                var protoBom = stream.ToArray();

                var runner = new ProtocRunner();
                var result = runner.Run(tempDir.DirectoryPath, protoBom, new []
                {
                    "--proto_path=./",
                    "--decode=cyclonedx.v1_7.Bom",
                    "bom-1.7.proto"
                });

                if (result.ExitCode == 0)
                {
                    Snapshot.Match(result.Output, SnapshotNameExtension.Create(filename));
                }
                else
                {
                    output.WriteLine(result.Output);
                    output.WriteLine(result.Errors);
                    Assert.Equal(0, result.ExitCode);
                }
            }
        }
    }
}
