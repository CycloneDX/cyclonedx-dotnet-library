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
using CycloneDX.Models;
using CycloneDX.Spdx.Serialization;

namespace CycloneDX.Spdx.Interop.Tests
{
    public class ConverterTests
    {
        [Theory]
        [InlineData("document")]
        public void FromSpdxToCDXToSpdxRoundTripTest(string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", "Spdx", "v2.2", baseFilename + ".json");
            var fileContents = File.ReadAllText(resourceFilename);

            var spdxDocument = JsonSerializer.Deserialize(fileContents);
            var cdxBom = spdxDocument.ToCycloneDX();
            var result = cdxBom.ToSpdx();
            var resultString = JsonSerializer.Serialize(result);

            Snapshot.Match(resultString, SnapshotNameExtension.Create(baseFilename));
        }

        [Theory]
        [InlineData("document")]
        public void FromSpdxToCDXTest(string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", "Spdx", "v2.2", baseFilename + ".json");
            var fileContents = File.ReadAllText(resourceFilename);

            var spdxDocument = JsonSerializer.Deserialize(fileContents);
            var cdxBom = spdxDocument.ToCycloneDX();
            var resultString = CycloneDX.Json.Serializer.Serialize(cdxBom);

            Snapshot.Match(resultString, SnapshotNameExtension.Create(baseFilename));
        }

        [Theory]
        [InlineData("assembly")]
        [InlineData("bom")]
        [InlineData("component-hashes")]
        [InlineData("component-ref")]
        [InlineData("component-swid")]
        [InlineData("component-swid-full")]
        [InlineData("component-types")]
        [InlineData("compositions")]
        [InlineData("dependency")]
        [InlineData("empty-components")]
        [InlineData("evidence")]
        [InlineData("external-reference")]
        [InlineData("license-expression")]
        [InlineData("license-id")]
        [InlineData("license-name")]
        [InlineData("metadata-author")]
        [InlineData("metadata-license")]
        [InlineData("metadata-manufacture")]
        [InlineData("metadata-supplier")]
        [InlineData("metadata-timestamp")]
        [InlineData("metadata-tool")]
        [InlineData("minimal-viable")]
        [InlineData("patch")]
        [InlineData("properties")]
        [InlineData("service")]
        [InlineData("service-empty-objects")]
        public void FromCDXToSpdxTest(string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", "CycloneDX", "v1.3", $"valid-{baseFilename}-1.3.json");
            var fileContents = File.ReadAllText(resourceFilename);

            var cdxBom = CycloneDX.Json.Serializer.Deserialize(fileContents);
            // set the BOM timestamp if it hasn't been already
            // it's required for converting to SPDX
            // and if we don't set it the tests will fail because the time has changed from last test run
            cdxBom.Metadata = cdxBom.Metadata ?? new Metadata();
            cdxBom.Metadata.Timestamp = cdxBom.Metadata.Timestamp ?? new DateTime(2021, 12, 24, 12, 00, 00, DateTimeKind.Utc);

            var spdxDocument = cdxBom.ToSpdx();
            var resultString = JsonSerializer.Serialize(spdxDocument);

            Snapshot.Match(resultString, SnapshotNameExtension.Create(baseFilename));
        }

        [Theory]
        [InlineData("assembly")]
        [InlineData("bom")]
        [InlineData("component-hashes")]
        [InlineData("component-ref")]
        [InlineData("component-swid")]
        [InlineData("component-swid-full")]
        [InlineData("component-types")]
        [InlineData("compositions")]
        [InlineData("dependency")]
        [InlineData("empty-components")]
        [InlineData("evidence")]
        [InlineData("external-reference")]
        [InlineData("license-expression")]
        [InlineData("license-id")]
        [InlineData("license-name")]
        [InlineData("metadata-author")]
        [InlineData("metadata-license")]
        [InlineData("metadata-manufacture")]
        [InlineData("metadata-supplier")]
        [InlineData("metadata-timestamp")]
        [InlineData("metadata-tool")]
        [InlineData("minimal-viable")]
        [InlineData("patch")]
        [InlineData("properties")]
        [InlineData("service")]
        [InlineData("service-empty-objects")]
        public void FromCDXToSpdxToCDXRoundTripTest(string baseFilename)
        {
            var resourceFilename = Path.Join("Resources", "CycloneDX", "v1.3", $"valid-{baseFilename}-1.3.json");
            var fileContents = File.ReadAllText(resourceFilename);

            var cdxBom = CycloneDX.Json.Serializer.Deserialize(fileContents);
            // set the BOM timestamp if it hasn't been already
            // it's required for converting to SPDX
            // and if we don't set it the tests will fail because the time has changed from last test run
            cdxBom.Metadata = cdxBom.Metadata ?? new Metadata();
            cdxBom.Metadata.Timestamp = cdxBom.Metadata.Timestamp ?? new DateTime(2021, 12, 24, 12, 00, 00, DateTimeKind.Utc);

            var spdxDocument = cdxBom.ToSpdx();
            var resultBom = spdxDocument.ToCycloneDX();
            var resultString = CycloneDX.Json.Serializer.Serialize(resultBom);

            Snapshot.Match(resultString, SnapshotNameExtension.Create(baseFilename));
        }
    }
}
