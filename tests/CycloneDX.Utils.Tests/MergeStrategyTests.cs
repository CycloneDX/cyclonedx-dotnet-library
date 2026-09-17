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
        public void FlatMerge_WithStrategy_ReconcilesOwnMetadataComponentWithAnAlreadyMergedThinEntry()
        {
            // moduleA depends on moduleB, but only knows enough about it to
            // list it as a thin, dependency-only Components entry.
            var thinB = new Component { Name = "moduleB", Version = "1.0.0", Group = "example", Purl = "pkg:maven/example/moduleB@1.0.0" };
            var moduleABom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "moduleA", Version = "1.0.0" } },
                Components = new List<Component> { thinB },
            };

            // moduleB's own bom describes itself richly via Metadata.Component,
            // with no Components list of its own.
            var richB = new Component { Name = "moduleB", Version = "1.0.0", Group = "example", Purl = "pkg:maven/example/moduleB@1.0.0", Description = "the real thing" };
            var moduleBBom = new Bom
            {
                Metadata = new Metadata { Component = richB },
            };

            var result = CycloneDXUtils.FlatMerge(new List<Bom> { moduleABom, moduleBBom }, MergeStrategy.Default());

            // Previously this produced two Components entries sharing the
            // same real-world identity (and, once bom-refs were assigned,
            // the same bom-ref) -- one thin, one rich -- instead of folding
            // moduleB's own self-description into the already-accumulated
            // thin stub.
            Assert.Equal(2, result.Components.Count);
            var mergedB = Assert.Single(result.Components, c => c.Name == "moduleB");
            Assert.Equal("the real thing", mergedB.Description);
        }

        [Fact]
        public void FlatMerge_WithStrategy_DoesNotRenameEquivalentComponentsAboutToBeSquashed()
        {
            // Two source BOMs each depend on the same real-world package,
            // sharing its bom-ref, but recorded slightly different details
            // for it (no Scope conflict) -- moduleA's copy has hashes,
            // moduleB's has a description. RenameConflictingComponents must
            // not rename either copy here: the generic Components merge is
            // going to fold them into one entry anyway (Equivalent, no
            // conflict under the default resolution), and renaming first
            // would leave moduleB's own dependsOn entry pointing at a
            // bom-ref no component ends up carrying.
            const string jackRef = "pkg:maven/example/jackson@1.0.0";
            var moduleABom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "moduleA" } },
                Components = new List<Component>
                {
                    new Component { Name = "jackson", Version = "1.0.0", Purl = jackRef, BomRef = jackRef, Hashes = new List<Hash> { new Hash { Alg = Hash.HashAlgorithm.SHA_256, Content = "aaaa" } } }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "moduleA", Dependencies = new List<Dependency> { new Dependency { Ref = jackRef } } }
                },
            };
            var moduleBBom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "moduleB" } },
                Components = new List<Component>
                {
                    new Component { Name = "jackson", Version = "1.0.0", Purl = jackRef, BomRef = jackRef, Description = "Jackson integration helpers" }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "moduleB", Dependencies = new List<Dependency> { new Dependency { Ref = jackRef } } }
                },
            };

            var result = CycloneDXUtils.FlatMerge(new List<Bom> { moduleABom, moduleBBom }, MergeStrategy.Default());

            var componentRefs = new HashSet<string>();
            foreach (var c in result.Components) componentRefs.Add(c.BomRef);

            // jackson (merged into one) + moduleA's and moduleB's own
            // Metadata.Component self-descriptions.
            Assert.Equal(3, result.Components.Count);
            var jackson = Assert.Single(result.Components, c => c.Name == "jackson");
            Assert.Single(jackson.Hashes);
            Assert.Equal("Jackson integration helpers", jackson.Description);

            void AssertNoDangling(IEnumerable<Dependency> deps)
            {
                foreach (var d in deps)
                {
                    Assert.True(componentRefs.Contains(d.Ref) || d.Ref == "moduleA" || d.Ref == "moduleB",
                        $"dependency ref '{d.Ref}' has no matching component");
                    if (d.Dependencies != null) AssertNoDangling(d.Dependencies);
                }
            }
            AssertNoDangling(result.Dependencies);
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

        [Fact]
        public void BomRenameRef_Throws_WhenNewRefAlreadyInUse()
        {
            var bom = new Bom
            {
                Components = new List<Component>
                {
                    new Component { Name = "left-pad", BomRef = "old-ref" },
                    new Component { Name = "right-pad", BomRef = "already-taken" }
                }
            };

            Assert.Throws<System.InvalidOperationException>(() => bom.RenameRef("old-ref", "already-taken"));
            // Refused, so nothing should have been touched.
            Assert.Equal("old-ref", bom.Components[0].BomRef);
        }

        [Fact]
        public void Default_Strategy_UpgradesConflictingScope_ToRequired()
        {
            // Locks in the flipped default: a Required-vs-Optional conflict
            // must not silently downgrade to Optional.
            Assert.Equal(ComponentConflictResolution.Squash_UpgradeScope, MergeStrategy.Default().ComponentConflictResolution);

            var a = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Required };
            var b = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional };

            Assert.True(a.MergeWith(b, MergeStrategy.Default()));
            Assert.Equal(Component.ComponentScope.Required, a.Scope);
        }

        [Fact]
        public void Squash_DowngradeScope_PrefersOptionalOverRequired()
        {
            var strategy = MergeStrategy.Default();
            strategy.ComponentConflictResolution = ComponentConflictResolution.Squash_DowngradeScope;

            var a = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Required };
            var b = new Component { Name = "left-pad", Version = "1.0.0", Scope = Component.ComponentScope.Optional };

            Assert.True(a.MergeWith(b, strategy));
            Assert.Equal(Component.ComponentScope.Optional, a.Scope);
        }

        [Fact]
        public void Dependency_MergeWith_UnionsSubsetDependsOnLists()
        {
            var a = new Dependency { Ref = "app", Dependencies = new List<Dependency> { new Dependency { Ref = "lib-a" } } };
            var b = new Dependency { Ref = "app", Dependencies = new List<Dependency> { new Dependency { Ref = "lib-b" } } };

            Assert.True(a.MergeWith(b, MergeStrategy.Default()));
            Assert.Equal(2, a.Dependencies.Count);
            Assert.Contains(a.Dependencies, d => d.Ref == "lib-a");
            Assert.Contains(a.Dependencies, d => d.Ref == "lib-b");
        }

        [Fact]
        public void Dependency_MergeWith_RefusesDiffering_WhenSubsetMergeDisabled()
        {
            var strategy = MergeStrategy.Default();
            strategy.MergeSubsetDependencies = false;

            var a = new Dependency { Ref = "app", Dependencies = new List<Dependency> { new Dependency { Ref = "lib-a" } } };
            var b = new Dependency { Ref = "app", Dependencies = new List<Dependency> { new Dependency { Ref = "lib-b" } } };

            Assert.False(a.MergeWith(b, strategy));
        }

        [Fact]
        public void FlatMerge_RenameByScope_SplitsConflictingComponentsAndFixesUpBackReferences()
        {
            var strategy = MergeStrategy.Default();
            strategy.ComponentConflictResolution = ComponentConflictResolution.Squash_RenameByScope;

            var bom1 = new Bom
            {
                Components = new List<Component>
                {
                    new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Required }
                },
                Dependencies = new List<Dependency> { new Dependency { Ref = "lp" } }
            };
            var bom2 = new Bom
            {
                Components = new List<Component>
                {
                    new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Excluded }
                },
                Dependencies = new List<Dependency> { new Dependency { Ref = "lp" } }
            };

            var result = CycloneDXUtils.FlatMerge(bom1, bom2, strategy);

            Assert.Equal(2, result.Components.Count);
            var required = Assert.Single(result.Components, c => c.Scope == Component.ComponentScope.Required);
            var excluded = Assert.Single(result.Components, c => c.Scope == Component.ComponentScope.Excluded);
            Assert.Equal("lp:scope=Required", required.BomRef);
            Assert.Equal("lp:scope=Excluded", excluded.BomRef);

            Assert.Equal(2, result.Dependencies.Count);
            Assert.Contains(result.Dependencies, d => d.Ref == "lp:scope=Required");
            Assert.Contains(result.Dependencies, d => d.Ref == "lp:scope=Excluded");
        }

        [Fact]
        public void FlatMerge_RenameByScope_DoesNotSuffixWhenAllSourcesAgree()
        {
            var strategy = MergeStrategy.Default();
            strategy.ComponentConflictResolution = ComponentConflictResolution.Squash_RenameByScope;

            var bom1 = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Required } }
            };
            var bom2 = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Required, Description = "pads strings" } }
            };

            var result = CycloneDXUtils.FlatMerge(bom1, bom2, strategy);

            var merged = Assert.Single(result.Components);
            Assert.Equal("lp", merged.BomRef);
            Assert.Equal("pads strings", merged.Description);
        }

        [Fact]
        public void FlatMerge_RenameByScope_ThirdCopySquashesIntoExistingPartition()
        {
            var strategy = MergeStrategy.Default();
            strategy.ComponentConflictResolution = ComponentConflictResolution.Squash_RenameByScope;

            var bom1 = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Required } }
            };
            var bom2 = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Excluded } }
            };
            var bom3 = new Bom
            {
                Components = new List<Component> { new Component { Name = "left-pad", Version = "1.0.0", BomRef = "lp", Scope = Component.ComponentScope.Required, Copyright = "2024 Acme" } }
            };

            var result = CycloneDXUtils.FlatMerge(new[] { bom1, bom2, bom3 }, strategy);

            Assert.Equal(2, result.Components.Count);
            var required = Assert.Single(result.Components, c => c.Scope == Component.ComponentScope.Required);
            Assert.Equal("2024 Acme", required.Copyright);
        }

        [Fact]
        public void CleanupMetadataComponent_RemovesDuplicateAndMergesFields()
        {
            var subject = new Component { Name = "app", Version = "1", BomRef = "app-ref" };
            var duplicate = new Component { Name = "app", Version = "1", BomRef = "app-ref", Description = "the app" };
            var other = new Component { Name = "lib", Version = "1", BomRef = "lib-ref" };
            var bom = new Bom
            {
                Metadata = new Metadata { Component = subject },
                Components = new List<Component> { duplicate, other }
            };

            var result = CycloneDXUtils.CleanupMetadataComponent(bom);

            Assert.Single(result.Components);
            Assert.Same(other, result.Components[0]);
            Assert.Equal("the app", result.Metadata.Component.Description);
        }

        [Fact]
        public void CleanupMetadataComponent_NoOp_WhenNoDuplicate()
        {
            var subject = new Component { Name = "app", Version = "1", BomRef = "app-ref" };
            var other = new Component { Name = "lib", Version = "1", BomRef = "lib-ref" };
            var bom = new Bom
            {
                Metadata = new Metadata { Component = subject },
                Components = new List<Component> { other }
            };

            var result = CycloneDXUtils.CleanupMetadataComponent(bom);

            Assert.Single(result.Components);
            Assert.Same(other, result.Components[0]);
        }

        [Fact]
        public void CleanupEmptyLists_ReplacesEmptyListsWithNull()
        {
            var bom = new Bom
            {
                Components = new List<Component>(),
                Services = new List<Service> { new Service { Name = "svc" } },
                Dependencies = new List<Dependency>(),
            };

            var result = CycloneDXUtils.CleanupEmptyLists(bom);

            Assert.Null(result.Components);
            Assert.Null(result.Dependencies);
            Assert.NotNull(result.Services);
            Assert.Single(result.Services);
        }

        [Fact]
        public void AttachDanglingComponents_BucketsByScopeAndAttachesUnderRoot()
        {
            var bom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "root-app", BomRef = "root" } },
                Components = new List<Component>
                {
                    new Component { Name = "req-lib", BomRef = "req-lib", Scope = Component.ComponentScope.Required },
                    new Component { Name = "opt-lib", BomRef = "opt-lib", Scope = Component.ComponentScope.Optional },
                    new Component { Name = "linked-lib", BomRef = "linked-lib", Scope = Component.ComponentScope.Required },
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "root", Dependencies = new List<Dependency> { new Dependency { Ref = "linked-lib" } } },
                },
            };

            var attached = bom.AttachDanglingComponents();

            Assert.Equal(2, attached.Count);
            var requiredBucket = Assert.Single(attached, kv => kv.Key.Contains("scope=Required")).Value;
            Assert.Equal(new List<string> { "req-lib" }, requiredBucket);
            var optionalBucket = Assert.Single(attached, kv => kv.Key.Contains("scope=Optional")).Value;
            Assert.Equal(new List<string> { "opt-lib" }, optionalBucket);

            // linked-lib was already reachable -- not touched.
            var rootEntry = Assert.Single(bom.Dependencies, d => d.Ref == "root");
            Assert.Equal(3, rootEntry.Dependencies.Count); // linked-lib + 2 new buckets
            Assert.Contains(rootEntry.Dependencies, d => d.Ref == "linked-lib");
        }

        [Fact]
        public void AttachDanglingComponents_AttachesUnderExplicitRefWhenPresent()
        {
            var bom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "root-app", BomRef = "root" } },
                Components = new List<Component>
                {
                    new Component { Name = "orphan", BomRef = "orphan", Scope = Component.ComponentScope.Required },
                    new Component { Name = "grouping", BomRef = "grouping" },
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "root", Dependencies = new List<Dependency> { new Dependency { Ref = "grouping" } } },
                    new Dependency { Ref = "grouping", Dependencies = new List<Dependency>() },
                },
            };

            bom.AttachDanglingComponents("grouping");

            var groupingEntry = Assert.Single(bom.Dependencies, d => d.Ref == "grouping");
            Assert.Single(groupingEntry.Dependencies);
            var rootEntry = Assert.Single(bom.Dependencies, d => d.Ref == "root");
            Assert.Single(rootEntry.Dependencies); // unchanged -- attachment went under "grouping"
        }

        [Fact]
        public void AttachDanglingComponents_FallsBackToRoot_WhenExplicitRefNotFound()
        {
            var bom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "root-app", BomRef = "root" } },
                Components = new List<Component> { new Component { Name = "orphan", BomRef = "orphan" } },
                Dependencies = new List<Dependency> { new Dependency { Ref = "root" } },
            };

            bom.AttachDanglingComponents("does-not-exist");

            var rootEntry = Assert.Single(bom.Dependencies, d => d.Ref == "root");
            Assert.Single(rootEntry.Dependencies);
        }

        [Fact]
        public void AttachDanglingComponents_NoOp_WhenNothingIsDangling()
        {
            var bom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "root-app", BomRef = "root" } },
                Components = new List<Component> { new Component { Name = "linked-lib", BomRef = "linked-lib" } },
                Dependencies = new List<Dependency>
                {
                    new Dependency { Ref = "root", Dependencies = new List<Dependency> { new Dependency { Ref = "linked-lib" } } },
                },
            };

            var attached = bom.AttachDanglingComponents();

            Assert.Empty(attached);
            Assert.Single(bom.Components);
        }

        [Fact]
        public void AttachDanglingComponents_IsIdempotent()
        {
            var bom = new Bom
            {
                Metadata = new Metadata { Component = new Component { Name = "root-app", BomRef = "root" } },
                Components = new List<Component> { new Component { Name = "orphan", BomRef = "orphan" } },
                Dependencies = new List<Dependency> { new Dependency { Ref = "root" } },
            };

            var firstPass = bom.AttachDanglingComponents();
            var secondPass = bom.AttachDanglingComponents();

            Assert.Single(firstPass);
            Assert.Empty(secondPass);
        }
    }
}
#endif
