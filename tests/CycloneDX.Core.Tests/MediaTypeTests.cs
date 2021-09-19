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
        [InlineData(Format.Xml, SpecificationVersion.v1_3, "application/vnd.cyclonedx+xml; version=1.3")]
        [InlineData(Format.Xml, SpecificationVersion.v1_2, "application/vnd.cyclonedx+xml; version=1.2")]
        [InlineData(Format.Xml, SpecificationVersion.v1_1, "application/vnd.cyclonedx+xml; version=1.1")]
        [InlineData(Format.Xml, SpecificationVersion.v1_0, "application/vnd.cyclonedx+xml; version=1.0")]
        [InlineData(Format.Json, SpecificationVersion.v1_3, "application/vnd.cyclonedx+json; version=1.3")]
        [InlineData(Format.Json, SpecificationVersion.v1_2, "application/vnd.cyclonedx+json; version=1.2")]
        [InlineData(Format.Protobuf, SpecificationVersion.v1_3, "application/x.vnd.cyclonedx+protobuf; version=1.3")]
        public void MediaTypeAndVersionIsCorrect(Format format, SpecificationVersion schemaVersion, string expected)
        {
            Assert.Equal(expected, MediaTypes.GetMediaType(format, schemaVersion));
        }

        [Theory]
        [InlineData(Format.Xml, "application/vnd.cyclonedx+xml")]
        [InlineData(Format.Json, "application/vnd.cyclonedx+json")]
        [InlineData(Format.Protobuf, "application/x.vnd.cyclonedx+protobuf")]
        public void MediaTypeIsCorrect(Format format, string expected)
        {
            Assert.Equal(expected, MediaTypes.GetMediaType(format));
        }

    }
}
