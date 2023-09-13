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

namespace CycloneDX.Core.Tests.Protobuf.v1_5
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
        public void ValidProtobufTest(string filename)
        {
            using (var tempDir = new TempDirectoryWithProtoSchemas())
            {
                var resourceFilename = Path.Join("Resources", "v1.5", filename);
                var jsonBom = File.ReadAllText(resourceFilename);
                var inputBom = CycloneDX.Json.Serializer.Deserialize(jsonBom);

                var stream = new MemoryStream();
                CycloneDX.Protobuf.Serializer.Serialize(inputBom, stream);

                var protoBom = stream.ToArray();

                var runner = new ProtocRunner();
                var result = runner.Run(tempDir.DirectoryPath, protoBom, new string[]
                {
                    "--proto_path=./",
                    "--decode=cyclonedx.v1_5.Bom",
                    "bom-1.5.proto"
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
