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

using System.Collections.Generic;
using CycloneDX.Models;
using Xunit;

namespace CycloneDX.Utils.Tests
{
    public class NestedComponentVersionDiffTests
    {
        [Theory]
        [InlineData(0, "1", "2")]
        [InlineData(0, null, "2")]
        [InlineData(0, "1", null)]
        [InlineData(0, "1", "1")]
        [InlineData(1, "1", "2")]
        [InlineData(1, null, "2")]
        [InlineData(1, "1", null)]
        [InlineData(1, "1", "1")]
        [InlineData(2, "1", "2")]
        [InlineData(2, null, "2")]
        [InlineData(2, "1", null)]
        [InlineData(2, "1", "1")]
        public void ComponentVersionsAtEachDepthAreCompared(int depth, string fromVersion, string toVersion)
        {
            var fromBom = CreateBom(depth, fromVersion);
            var toBom = CreateBom(depth, toVersion);

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.True(result.ContainsKey("library"));
            var diff = result["library"];
            Assert.Equal(fromVersion != null && fromVersion != toVersion ? 1 : 0, diff.Removed.Count);
            Assert.Equal(toVersion != null && fromVersion != toVersion ? 1 : 0, diff.Added.Count);
            Assert.Equal(fromVersion == toVersion ? 1 : 0, diff.Unchanged.Count);
            if (diff.Removed.Count > 0) { Assert.Equal(fromVersion, diff.Removed[0].Version); }
            if (diff.Added.Count > 0) { Assert.Equal(toVersion, diff.Added[0].Version); }
            for (var i = 0; i < depth; i++)
            {
                Assert.Single(result[$"assembly-{i}"].Unchanged);
                Assert.Empty(result[$"assembly-{i}"].Added);
                Assert.Empty(result[$"assembly-{i}"].Removed);
            }
        }

        [Fact]
        public void MovingAComponentIntoAnAssemblyKeepsItsVersionUnchanged()
        {
            var result = CycloneDXUtils.ComponentVersionDiff(CreateBom(0, "1"), CreateBom(2, "1"));

            Assert.True(result.ContainsKey("library"));
            Assert.Single(result["library"].Unchanged);
            Assert.Empty(result["library"].Added);
            Assert.Empty(result["library"].Removed);
            Assert.Single(result["assembly-0"].Added);
            Assert.Single(result["assembly-1"].Added);
        }

        [Fact]
        public void PedigreeComponentsAreNotIncludedInTheVersionDiff()
        {
            var fromBom = CreateBom(0, "1");
            var toBom = CreateBom(0, "1");
            toBom.Components[0].Pedigree = new Pedigree
            {
                Ancestors = new List<Component>
                {
                    new Component { Type = Component.Classification.Library, Name = "ancestor", Version = "0" }
                }
            };

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result);
            Assert.Single(result["library"].Unchanged);
        }

        private static Bom CreateBom(int depth, string version)
        {
            var components = new List<Component>();
            if (version != null)
            {
                components.Add(new Component
                {
                    Type = Component.Classification.Library,
                    Name = "library",
                    Version = version
                });
            }

            for (var i = 0; i < depth; i++)
            {
                components = new List<Component>
                {
                    new Component
                    {
                        Type = Component.Classification.Application,
                        Name = $"assembly-{i}",
                        Version = "1",
                        Components = components
                    }
                };
            }

            return new Bom { Components = components };
        }
    }
}
