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
using System;
using System.Collections.Generic;
using CycloneDX.Models;

namespace CycloneDX
{
    /// <summary>
    /// Strategy-aware list merging for any element type implementing
    /// IEquatable/IEquivalent/IMergeable. One generic method merges
    /// List&lt;Hash&gt;, List&lt;Component&gt;, List&lt;OrganizationalContact&gt;,
    /// etc., dispatching through real interface calls rather than a
    /// separate reflection-driven helper per element type. Lives in
    /// CycloneDX.Core (rather than alongside CycloneDX.Utils/Merge.cs)
    /// specifically so model classes like Component can call it directly
    /// from their own MergeWith implementations.
    /// </summary>
    public static class MergeableListHelper
    {
        public static List<T> Merge<T>(List<T> list1, List<T> list2, MergeStrategy strategy)
            where T : IEquatable<T>, IEquivalent<T>, IMergeable<T>
        {
            if (list1 is null) return list2;
            if (list2 is null) return list1;
            if (strategy is null || !strategy.UseEntityMerge)
            {
                return ExactMatchMerge(list1, list2);
            }

            var result = new List<T>(list1);
            foreach (var incoming in list2)
            {
                bool merged = false;
                for (int i = 0; i < result.Count; i++)
                {
                    var existing = result[i];
                    if (existing.Equals(incoming) || existing.Equivalent(incoming, strategy))
                    {
                        if (existing.MergeWith(incoming, strategy))
                        {
                            result[i] = existing;
                            merged = true;
                            break;
                        }
                    }
                }
                if (!merged)
                {
                    result.Add(incoming);
                }
            }
            return result;
        }

        /// <summary>
        /// Cheap fallback: dedupe by exact equality only, no MergeWith
        /// attempts. Mirrors CycloneDX.Utils.ListMergeHelper&lt;T&gt;'s
        /// behavior (kept independently here to avoid a Core-&gt;Utils
        /// dependency, which would invert this project's reference graph).
        /// </summary>
        private static List<T> ExactMatchMerge<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
        {
            var result = new List<T>(list1);
            foreach (var item in list2)
            {
                bool found = false;
                foreach (var existing in result)
                {
                    if (existing.Equals(item))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>Take whichever of two nullable reference values is non-null, preferring <paramref name="a"/>.</summary>
        public static T MergeSingle<T>(T a, T b) where T : class => a ?? b;

        /// <summary>Union two string lists, preserving order, dropping duplicates. Null if both are null.</summary>
        public static List<string> MergeStringList(List<string> a, List<string> b)
        {
            if (a is null) return b;
            if (b is null) return a;
            var result = new List<string>(a);
            foreach (var s in b)
            {
                if (!result.Contains(s))
                {
                    result.Add(s);
                }
            }
            return result;
        }

        /// <summary>Nullable-bool "either says true" merge: null if both unset, else true if either is true.</summary>
        public static bool? MergeNullableBoolOr(bool? a, bool? b)
        {
            if (!a.HasValue && !b.HasValue) return null;
            return (a ?? false) || (b ?? false);
        }
    }
}
#endif
