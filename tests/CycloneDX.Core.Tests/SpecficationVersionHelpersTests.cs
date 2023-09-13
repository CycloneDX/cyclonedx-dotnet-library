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
    public class SpecficationVersionHelpersTests
    {
        [Theory]
        [InlineData("http://cyclonedx.org/schema/bom/1.0", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.1", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.2", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.3", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.4", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.5", true)]
        [InlineData("http://cyclonedx.org/schema/bom/1.6", false)]
        [InlineData("http://cyclonedx.org/schema/bom/", false)]
        public void IsValidXmlNamespaceTest(string xmlns, bool valid)
        {
            Assert.Equal(valid, SpecificationVersionHelpers.IsValidXmlNamespace(xmlns));
        }

        [Theory]
        [InlineData("http://cyclonedx.org/schema/bom/1.0", "1.0")]
        [InlineData("http://cyclonedx.org/schema/bom/1.1", "1.1")]
        [InlineData("http://cyclonedx.org/schema/bom/1.2", "1.2")]
        [InlineData("http://cyclonedx.org/schema/bom/1.3", "1.3")]
        [InlineData("http://cyclonedx.org/schema/bom/1.4", "1.4")]
        [InlineData("http://cyclonedx.org/schema/bom/1.5", "1.5")]
        [InlineData("http://cyclonedx.org/schema/bom/1.6", null)]
        [InlineData("http://cyclonedx.org/schema/bom/", null)]
        public void XmlNamespaceSpecificationVersionTest(string xmlns, string specVersionString)
        {
            Assert.Equal(specVersionString, SpecificationVersionHelpers.XmlNamespaceSpecificationVersion(xmlns));
        }
    }
}
