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
using System.Collections.Generic;
using Xunit;
using CycloneDX;
using CycloneDX.Models.v1_3;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class ComponentVersionDiffTests
    {
        [Fact]
        public void ComponentRemovedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string>());

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Removed);
        }

        [Fact]
        public void ComponentAddedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string>());
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Added);
        }

        [Fact]
        public void ComponentUnchangedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Unchanged);
        }

        [Fact]
        public void ComponentModifiedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@2" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Added);
            Assert.Single(result["component"].Removed);
        }
    }
}
