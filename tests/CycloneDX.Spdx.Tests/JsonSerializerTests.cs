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
using CycloneDX.Spdx.Serialization;

namespace CycloneDX.Spdx.Tests;

public class JsonSerializerTests
{
    [Theory]
    [InlineData("document")]
    public void JsonRoundTripTest(string baseFilename)
    {
        var resourceFilename = Path.Join("Resources", "v2.2", baseFilename + ".json");
        var document = File.ReadAllText(resourceFilename);

        var spdxDocument = JsonSerializer.Deserialize(document);
        var result = JsonSerializer.Serialize(spdxDocument);

        Snapshot.Match(result, SnapshotNameExtension.Create(baseFilename));
    }

    [Theory]
    [InlineData("document")]
    public async Task JsonAsyncRoundTripTest(string baseFilename)
    {
        var resourceFilename = Path.Join("Resources", "v2.2", baseFilename + ".json");

        using (var jsonStream = File.OpenRead(resourceFilename))
        using (var outputStream = new MemoryStream())
        {
            var spdxDocument = await JsonSerializer.DeserializeAsync(jsonStream);
            await JsonSerializer.SerializeAsync(spdxDocument, outputStream);
            var result = System.Text.Encoding.UTF8.GetString(outputStream.ToArray());
            Snapshot.Match(result, SnapshotNameExtension.Create(baseFilename));
        }
    }
}