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

namespace CycloneDX.Core.Tests.Xml.v1_0
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-bom-1.0.xml")]
        [InlineData("valid-component-hashes-1.0.xml")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.0", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
        
        [Theory]
        [InlineData("valid-bom-1.0.xml")]
        [InlineData("valid-component-hashes-1.0.xml")]
        public void XmlRoundTripStreamTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.0", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Serializer.Deserialize(xmlBom);
            using var ms = new MemoryStream();
            Serializer.Serialize(bom, ms);

            Snapshot.Match(Encoding.UTF8.GetString(ms.ToArray()), SnapshotNameExtension.Create(filename));
        }
    }
}
