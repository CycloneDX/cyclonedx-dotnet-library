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
using Xunit;

namespace CycloneDX.Core.Tests
{
    public class MediaTypeTests
    {
        [Theory]
        [InlineData(Format.Xml, null, "application/vnd.cyclonedx+xml")]
        [InlineData(Format.Xml, SchemaVersion.v1_3, "application/vnd.cyclonedx+xml; version=1.3")]
        [InlineData(Format.Xml, SchemaVersion.v1_2, "application/vnd.cyclonedx+xml; version=1.2")]
        [InlineData(Format.Xml, SchemaVersion.v1_1, "application/vnd.cyclonedx+xml; version=1.1")]
        [InlineData(Format.Xml, SchemaVersion.v1_0, "application/vnd.cyclonedx+xml; version=1.0")]
        [InlineData(Format.Json, null, "application/vnd.cyclonedx+json")]
        [InlineData(Format.Json, SchemaVersion.v1_3, "application/vnd.cyclonedx+json; version=1.3")]
        [InlineData(Format.Json, SchemaVersion.v1_2, "application/vnd.cyclonedx+json; version=1.2")]
        [InlineData(Format.Protobuf, null, "application/x.vnd.cyclonedx+protobuf")]
        [InlineData(Format.Protobuf, SchemaVersion.v1_3, "application/x.vnd.cyclonedx+protobuf; version=1.3")]
        public void MediaTypeIsCorrect(Format format, SchemaVersion? schemaVersion, string expected)
        {
            Assert.Equal(expected, MediaTypes.GetMediaType(format, schemaVersion));
        }
    }
}
