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

#if NET8_0_OR_GREATER
using System.Collections.Generic;
using Xunit;
using CycloneDX;
using CycloneDX.Models;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class MergeStrategyTests
    {
        [Fact]
        public void Component_Equivalent_MatchesOnTypeAndName_IgnoresBomRef()
        {
            var a = new Component { Name = "left-pad", Version = "1.0.0", BomRef = "ref-a" };
            var b = new Component { Name = "left-pad", Version = "1.0.0", BomRef = "ref-b" };

            Assert.True(a.Equivalent(b, MergeStrategy.Default()));
        }

        [Fact]
        public void Component_Equivalent_False_WhenNameDiffers()
        {
            var a = new Component { Name = "left-pad", Version = "1.0.0" };
            var b = new Component { Name = "right-pad", Version = "1.0.0" };

            Assert.False(a.Equivalent(b, MergeStrategy.Default()));
        }

        [Fact]
        public void Component_MergeWith_SquashesOptionalScopes()
        {
            var a = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional };
            var b = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional, Description = "pads strings" };

            Assert.True(a.MergeWith(b, MergeStrategy.Default()));
            Assert.Equal(Component.ComponentScope.Optional, a.Scope);
            Assert.Equal("pads strings", a.Description);
        }

        [Fact]
        public void Component_MergeWith_RefusesExcludedVsRequiredConflict()
        {
            var a = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Excluded };
            var b = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Required };

            Assert.False(a.MergeWith(b, MergeStrategy.Default()));
        }

        [Fact]
        public void Component_MergeWith_KeepsBothExcluded_EvenWithUnrelatedFieldDifference()
        {
            // Regression check for the bug this port fixes: two components
            // that are both Excluded-scope but differ in some unrelated
            // field must still merge, not be treated as a scope conflict.
            var a = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Excluded };
            var b = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Excluded, Copyright = "2024 Acme" };

            Assert.True(a.MergeWith(b, MergeStrategy.Default()));
            Assert.Equal(Component.ComponentScope.Excluded, a.Scope);
            Assert.Equal("2024 Acme", a.Copyright);
        }

        [Fact]
        public void Hash_MergeWith_FillsInMissingContent()
        {
            var a = new Hash { Alg = Hash.HashAlgorithm.SHA_256, Content = null };
            var b = new Hash { Alg = Hash.HashAlgorithm.SHA_256, Content = "abc123" };

            Assert.True(a.MergeWith(b, MergeStrategy.Default()));
            Assert.Equal("abc123", a.Content);
        }

        [Fact]
        public void Hash_MergeWith_RefusesContentMismatch()
        {
            var a = new Hash { Alg = Hash.HashAlgorithm.SHA_256, Content = "abc123" };
            var b = new Hash { Alg = Hash.HashAlgorithm.SHA_256, Content = "def456" };

            Assert.False(a.MergeWith(b, MergeStrategy.Default()));
        }

        [Fact]
        public void FlatMerge_WithStrategy_SquashesEquivalentComponentsAcrossBoms()
        {
            var bom1 = new Bom
            {
                Components = new List<Component>
                {
                    new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional }
                }
            };
            var bom2 = new Bom
            {
                Components = new List<Component>
                {
                    new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional, Description = "pads strings" }
                }
            };

            var result = CycloneDXUtils.FlatMerge(bom1, bom2, MergeStrategy.Default());

            Assert.Single(result.Components);
            Assert.Equal("pads strings", result.Components[0].Description);
        }

        [Fact]
        public void BomRenameRef_RewritesIdentifierAndBackReferences()
        {
            var bom = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", BomRef = "old-ref" } },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "root", Dependencies = new List<Dependency> { new Dependency { Ref = "old-ref" } } }
                }
            };

            Assert.True(bom.RenameRef("old-ref", "new-ref"));
            Assert.Equal("new-ref", bom.Components[0].BomRef);
            Assert.Equal("new-ref", bom.Dependencies[0].Dependencies[0].Ref);
        }

        [Fact]
        public void BomRenameRef_ReturnsFalse_WhenRefNotPresent()
        {
            var bom = new Bom { Components = new List<Component> { new Component { Name = "left-pad", BomRef = "some-ref" } } };

            Assert.False(bom.RenameRef("missing-ref", "new-ref"));
        }
    }
}
#endif
