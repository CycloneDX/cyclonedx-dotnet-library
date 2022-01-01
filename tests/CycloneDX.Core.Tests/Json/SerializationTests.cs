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
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Json;

namespace CycloneDX.Core.Tests.Json
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("v1.2", "valid-bom-1.2.json")]
        [InlineData("v1.3", "valid-bom-1.3.json")]
        public void JsonRoundTripTest(string resourceSubdir, string filename)
        {
            var resourceFilename = Path.Join("Resources", resourceSubdir, filename);
            var jsonBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(jsonBom);
            jsonBom = Serializer.Serialize(bom);

            Snapshot.Match(jsonBom, SnapshotNameExtension.Create(filename));
        }

        [Theory]
        [InlineData("v1.2", "valid-bom-1.2.json")]
        [InlineData("v1.3", "valid-bom-1.3.json")]
        public async Task JsonRoundTripAsyncTest(string resourceSubdir, string filename)
        {
            var resourceFilename = Path.Join("Resources", resourceSubdir, filename);
            using (var jsonBomStream = File.OpenRead(resourceFilename))
            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                var bom = await Serializer.DeserializeAsync(jsonBomStream).ConfigureAwait(false);
                await Serializer.SerializeAsync(bom, ms).ConfigureAwait(false);
                ms.Position = 0;
                Snapshot.Match(sr.ReadToEnd(), SnapshotNameExtension.Create(filename));
            }
        }
    }
}
