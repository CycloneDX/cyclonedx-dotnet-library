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

// Default interface methods require the target to support .NET Standard 2.1 /
// .NET Core 3.0 or later ABI features. This library also targets netstandard2.0
// (for older .NET Framework consumers), which cannot support default bodies here,
// so the whole capability is only compiled in for the modern targets. Consumers
// on netstandard2.0 keep today's behavior (plain FlatMerge/HierarchicalMerge,
// no per-entity merge strategy) unchanged.
#if NET8_0_OR_GREATER
using System;

namespace CycloneDX.Models
{
    /// <summary>
    /// Marker interface for model types that participate in generic,
    /// strategy-driven BOM-tree operations (equivalence checks, merging).
    /// Deliberately empty: it exists only so <see cref="IMergeable{T}"/> and
    /// <see cref="IEquivalent{T}"/> share a common, checkable root, without
    /// requiring a shared base class.
    /// </summary>
    public interface IBomEntity
    {
    }

    /// <summary>
    /// Declares that two instances of <typeparamref name="T"/> can attempt to
    /// combine into one during a BOM merge.
    /// </summary>
    /// <remarks>
    /// The default implementation is intentionally trivial: it merges only if
    /// the two instances are already exactly equal per the type's own
    /// <see cref="IEquatable{T}"/> implementation (the same equality every
    /// model class in this library already provides). Most model types need
    /// nothing more than this default and simply declare
    /// <c>: IMergeable&lt;Foo&gt;</c> with no method body. Types with real
    /// field-by-field reconciliation rules (for example <c>Component</c>,
    /// where two BOM documents may describe the same component with
    /// different scopes, hashes, or external references) override this
    /// method with actual logic instead of relying on the default.
    ///
    /// This is the C# idiom this design deliberately favors over a shared
    /// base class with reflection-driven dispatch: every implementing type
    /// gets a uniform, compiler-checked contract for free, and only pays for
    /// custom behavior where it actually has some.
    /// </remarks>
    public interface IMergeable<T> : IBomEntity
    {
        /// <summary>
        /// Attempt to merge <paramref name="other"/> into <c>this</c>.
        /// </summary>
        /// <param name="other">The other instance to merge in.</param>
        /// <param name="strategy">Merge configuration/strategy toggles.</param>
        /// <returns>
        /// <c>true</c> if <c>this</c> now represents the union of both
        /// instances and <paramref name="other"/> can be dropped from the
        /// containing list; <c>false</c> if the two instances could not be
        /// merged (they should be kept as separate list entries).
        /// </returns>
        bool MergeWith(T other, MergeStrategy strategy) =>
            this is IEquatable<T> equatable && other != null && equatable.Equals(other);
    }

    /// <summary>
    /// Declares that two instances of <typeparamref name="T"/> can be
    /// evaluated for "close enough to be worth attempting a merge" -- a
    /// weaker, type-specific relation than exact equality.
    /// </summary>
    /// <remarks>
    /// The default implementation defers to exact <see cref="IEquatable{T}"/>
    /// equality; see <see cref="IMergeable{T}"/> for why this is the
    /// deliberate default rather than an abstract requirement.
    /// </remarks>
    public interface IEquivalent<T> : IBomEntity
    {
        /// <summary>
        /// Cheap pre-check: are these two instances plausibly the same
        /// real-world entity (and thus worth attempting
        /// <see cref="IMergeable{T}.MergeWith"/> on), even if not exactly
        /// equal?
        /// </summary>
        bool Equivalent(T other, MergeStrategy strategy) =>
            this is IEquatable<T> equatable && other != null && equatable.Equals(other);
    }
}
#endif
