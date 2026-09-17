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
namespace CycloneDX.Models
{
    /// <summary>
    /// Selectable algorithm for resolving two <c>Equivalent</c>-but-not-equal
    /// <c>Component</c>s during a merge. Kept as its own enum (rather than
    /// boolean toggles) specifically so new algorithms can be added as new
    /// cases without reshaping <see cref="MergeStrategy"/> or any of the
    /// merge call sites that read it.
    /// </summary>
    public enum ComponentConflictResolution
    {
        /// <summary>Leave both components as separate, unrelated entries.</summary>
        KeepSeparate,

        /// <summary>
        /// Merge reconcilable fields into one entry. If <c>Scope</c> differs,
        /// prefer keeping it unset/optional over silently widening it -- see
        /// <see cref="ComponentConflictResolution.SquashUpgradeScope"/> for
        /// the alternative.
        /// </summary>
        Squash,

        /// <summary>
        /// Same as <see cref="Squash"/>, but when <c>Scope</c> differs
        /// between the two, the more permissive value wins (e.g. "required"
        /// beats "optional").
        /// </summary>
        SquashUpgradeScope,

        /// <summary>
        /// Reserved for a future strategy: when two equivalent components
        /// differ only by <c>Scope</c>, keep both as distinct entries by
        /// renaming one's (or both's) <c>bom-ref</c> with a scope-derived
        /// suffix (and rewriting back-references accordingly) instead of
        /// squashing or discarding either. Not yet implemented -- selecting
        /// this value currently behaves like <see cref="KeepSeparate"/>.
        /// </summary>
        RenameByScope,
    }

    /// <summary>
    /// Configuration steering how <c>CycloneDXUtils.FlatMerge</c>/
    /// <c>HierarchicalMerge</c> reconcile two or more BOM documents.
    /// </summary>
    public class MergeStrategy
    {
        /// <summary>
        /// When <c>true</c>, list merging considers calling each item's
        /// <see cref="IMergeable{T}.MergeWith"/> to reconcile
        /// equivalent-but-not-equal entries; when <c>false</c>, list merging
        /// only deduplicates exactly-equal entries (fast, but may leave
        /// near-duplicate entries that a smarter merge could have combined).
        /// </summary>
        public bool UseEntityMerge { get; set; }

        /// <summary>
        /// When merging BOM documents that include <c>Equivalent</c>
        /// components with differing values of <c>Scope</c> (required/null
        /// vs. optional vs. excluded), rename the conflicting components'
        /// <c>bom-ref</c> (and their back-references) instead of silently
        /// conflating them under one identity.
        /// </summary>
        public bool RenameConflictingComponents { get; set; }

        /// <summary>
        /// Selects the algorithm used when two <c>Equivalent</c> components
        /// are not fully equal. See <see cref="ComponentConflictResolution"/>.
        /// </summary>
        public ComponentConflictResolution ComponentConflictResolution { get; set; }

        /// <summary>
        /// CycloneDX spec says dependency graphs "MUST" declare components
        /// with no dependencies as empty elements, which in practice does
        /// not always hold across BOM documents describing overlapping
        /// codebases (e.g. a Maven module built standalone vs. as part of a
        /// parent build). When <c>true</c>, differing (non-conflicting,
        /// subset-of-each-other) direct-dependency lists for the same
        /// component are merged (grown) rather than treated as a conflict.
        /// </summary>
        public bool MergeSubsetDependencies { get; set; }

        /// <summary>
        /// Treat back-references from <c>Dependencies</c> as an internal
        /// detail owned by the referenced component/service (as the spec
        /// implies), rather than an independent top-level concern. See also
        /// https://github.com/CycloneDX/specification/discussions/320
        /// </summary>
        public bool TreatDependencyAsExtraProperty { get; set; }

        /// <summary>
        /// Refresh the merged BOM's own metadata (serial number, timestamp,
        /// tool reference) after merging.
        /// </summary>
        public bool DoBomMetadataUpdate { get; set; }

        /// <summary>See <see cref="DoBomMetadataUpdate"/>.</summary>
        public bool DoBomMetadataUpdateNewSerialNumber { get; set; }

        /// <summary>See <see cref="DoBomMetadataUpdate"/>.</summary>
        public bool DoBomMetadataUpdateReferThisToolkit { get; set; }

        /// <summary>
        /// Reasonable default strategy settings, matching the behavior this
        /// port aims to reproduce. Callers can tune the returned instance
        /// further.
        /// </summary>
        public static MergeStrategy Default()
        {
            return new MergeStrategy
            {
                UseEntityMerge = true,
                RenameConflictingComponents = true,
                MergeSubsetDependencies = true,
                TreatDependencyAsExtraProperty = true,
                ComponentConflictResolution = ComponentConflictResolution.Squash,
                DoBomMetadataUpdate = false,
                DoBomMetadataUpdateNewSerialNumber = false,
                DoBomMetadataUpdateReferThisToolkit = false,
            };
        }
    }
}
#endif
