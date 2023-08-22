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
using System.Reflection;
using System.Text.RegularExpressions;
using CycloneDX.Models;

namespace CycloneDX
{
    /// <summary>
    /// Allows to merge generic lists with items of specified types
    /// (by default essentially adding entries which are not present
    /// yet according to List.Contains() method), and calls special
    /// logic for lists of BomEntry types.<br/>
    ///
    /// Used in CycloneDX.Utils various Merge implementations as well
    /// as in CycloneDX.Core BomEntity-derived classes' MergeWith().<br/>
    ///
    /// Does not modify original lists and returns a new instance
    /// with merged data. One exception is if one of the inputs is
    /// null or empty - then the other object is returned.
    /// </summary>
    /// <typeparam name="T">Type of listed entries</typeparam>
    public class ListMergeHelper<T>
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            return Merge(list1, list2, BomEntityListMergeHelperStrategy.Default());
        }

        public List<T> Merge(List<T> list1, List<T> list2, BomEntityListMergeHelperStrategy listMergeHelperStrategy)
        {
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
            {
                iDebugLevel = 0;
            }

            // Rule out utterly empty inputs
            if ((list1 is null || list1.Count < 1) && (list2 is null || list2.Count < 1))
            {
                if (!(list1 is null))
                {
                    return list1;
                }
                if (!(list2 is null))
                {
                    return list2;
                }
                return new List<T>();
            }

            // At least one of these entries exists, per above sanity check
            if (typeof(BomEntity).IsInstanceOfType((!(list1 is null) && list1.Count > 0) ? list1[0] : list2[0]))
            {
                MethodInfo methodMerge = null;
                Object helper;
                // Use cached info where available
                if (BomEntity.KnownBomEntityListMergeHelpers.TryGetValue(typeof(T), out BomEntityListMergeHelperReflection refInfo))
                {
                    methodMerge = refInfo.methodMerge;
                    helper = refInfo.helperInstance;
                }
                else
                {
                    // Inspired by https://stackoverflow.com/a/4661237/4715872
                    // to craft a List<SpecificType> "result" at run-time:
                    Type listHelperType = typeof(BomEntityListMergeHelper<>);
                    var constructedListHelperType = listHelperType.MakeGenericType(typeof(T));
                    helper = Activator.CreateInstance(constructedListHelperType);
                    // Gotta use reflection for run-time evaluated type methods:
                    methodMerge = constructedListHelperType.GetMethod("Merge", 0, new [] { typeof(List<T>), typeof(List<T>), typeof(BomEntityListMergeHelperStrategy) });
                }

                if (methodMerge != null)
                {
                    return (List<T>)methodMerge.Invoke(helper, new object[] {list1, list2, listMergeHelperStrategy});
                }
                else
                {
                    // Should not get here, but if we do - log and fall through
                    if (iDebugLevel >= 1)
                    {
                        Console.WriteLine($"Warning: List-Merge for BomEntity failed to find a Merge() helper method: {list1?.GetType()?.ToString()} and {list2?.GetType()?.ToString()}");
                    }
                }
            }

            // Lists of legacy types (for BomEntity we use BomEntityListMergeHelper<T> class)
            if (iDebugLevel >= 1)
            {
                Console.WriteLine($"List-Merge for legacy types: {list1?.GetType()?.ToString()} and {list2?.GetType()?.ToString()}");
            }

            if (list1 is null || list1.Count < 1)
            {
                return list2;
            }
            if (list2 is null || list2.Count < 1)
            {
                return list1;
            }

            var result = new List<T>(list1);

            foreach (var item in list2)
            {
                if (!(result.Contains(item)))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        // Adapted from https://stackoverflow.com/a/76523292/4715872
        public void SortByAscending(List<T> list)
        {
            SortByImpl<Object>(true, false, list, null, null);
        }

        public void SortByAscending(List<T> list, bool recursive)
        {
            SortByImpl<Object>(true, recursive, list, null, null);
        }

        public void SortByAscending<TKey>(List<T> list, Func<T, TKey> selector)
        {
            SortByImpl<TKey>(true, false, list, selector, null);
        }

        public void SortByAscending<TKey>(List<T> list, Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            SortByImpl<TKey>(true, false, list, selector, comparer);
        }

        public void SortByDescending(List<T> list)
        {
            SortByImpl<Object>(false, false, list, null, null);
        }

        public void SortByDescending(List<T> list, bool recursive)
        {
            SortByImpl<Object>(false, recursive, list, null, null);
        }

        public void SortByDescending<TKey>(List<T> list, Func<T, TKey> selector)
        {
            SortByImpl<TKey>(false, false, list, selector, null);
        }

        public void SortByDescending<TKey>(List<T> list, Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            SortByImpl<TKey>(false, false, list, selector, comparer);
        }

        /// <summary>
        /// Implementation of the sort algorithm.
        /// Special handling for BomEntity-derived objects, including
        /// optional recursion to have them sort their list-of-something
        /// properties.
        /// </summary>
        /// <typeparam name="TKey">ValueTuple of function parameters returned by selector lambda</typeparam>
        /// <param name="ascending">Ascending (true) or Descending (false)</param>
        /// <param name="recursive">Passed to BomEntity.NormalizeList() (effective if recursing), not handled right here</param>
        /// <param name="list">List<SomeType> to sort</param>
        /// <param name="selector">lambda to select a tuple of properties to sort by</param>
        /// <param name="comparer">null for default, or a custom comparer</param>
        public void SortByImpl<TKey>(bool ascending, bool recursive, List<T> list, Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            if (list is null || list.Count < 2)
            {
                // No-op quickly for null, empty or single-item lists
                return;
            }

            // Ordering proposed in those NormalizeList() implementations
            // is an educated guess. Main purpose for this is to have
            // consistently ordered serialized BomEntity-derived type
            // lists for the purposes of comparison and compression.
            if (selector is null && typeof(BomEntity).IsInstanceOfType(list[0]))
            {
                // This should really be offloaded as lambdas into the
                // BomEntity-derived classes themselves, but I've struggled
                // to cast the right magic spells at C# to please its gods.
                // In particular, the ValueTuple used in selector signature is
                // both generic for the values' types (e.g. <string, bool, int>),
                // and for their amount in the tuple (0, 1, 2, ... explicitly
                // stated). So this is the next best thing...

/*
                switch (typeof(TKey)) {
                    case Tool:
                        SortByImpl<Tool>(ascending, list,
                            o => (o?.Vendor, o?.Name, o?.Version),
                            comparer);
                        return;

                    case typeof(Component):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.BomRef, o?.Type, o?.Group, o?.Name, o?.Version),
                            comparer);
                        return;

                    case typeof(Service):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.BomRef, o?.Group, o?.Name, o?.Version),
                            comparer);
                        return;

                    case typeof(ExternalReference):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.Url, o?.Type),
                            comparer);
                        return;

                    case typeof(Dependency):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.Ref),
                            comparer);
                        return;

                    case typeof(Composition):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.Aggregate, o?.Assemblies, o?.Dependencies),
                            comparer);
                        return;

                    case typeof(Vulnerability):
                        SortByImpl<TKey>(ascending, list,
                            o => (o?.BomRef, o?.Id, o?.Created, o?.Updated),
                            comparer);
                        return;

                    default:
                        // Expensive but reliable (modulo differently sorted lists
                        // of identical item sets inside the otherwise identical
                        // objects); classes are welcome to implement theirs eventually
                        // or switch cases above currently.
                        SortByImplBomEntity<TKey>(ascending, list,
                            o => (o?.SerializeEntity()),
                            comparer);
                        return;
                }
*/

                // Alas, C# won't let us just call
                // BomEntity.NormalizeList(ascending, recursive, (List<BomEntity>)list) or
                // something as simple, so here it goes - some more reflection:
                var methodNormalizeList = typeof(BomEntity).GetMethod("NormalizeList",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                        new [] { typeof(bool), typeof(bool), typeof(List<IBomEntity>) });

                if (methodNormalizeList != null)
                {
                    if (BomEntity.KnownEntityTypeLists.TryGetValue(typeof(IBomEntity), out BomEntityListReflection refInfoListInterface))
                    {
                        if (BomEntity.KnownEntityTypeLists.TryGetValue(list[0].GetType(), out BomEntityListReflection refInfoListType))
                        {
                            // Gotta make ugly cast copies there and back:
                            List<IBomEntity> helper = (List<IBomEntity>)Activator.CreateInstance(refInfoListInterface.genericType);
                            refInfoListInterface.methodAddRange.Invoke(helper, new object[] {list});

                            methodNormalizeList.Invoke(null, new object[] {ascending, recursive, helper});

                            // Populate back the original list object:
                            list.Clear();
                            foreach (var item in helper)
                            {
                                refInfoListType.methodAdd.Invoke(list, new object[] {item});
                            }
                        }
                    }
                } // else keep it as was? no good cause for an exception?..

                return;
            }

            if (comparer is null)
            {
                comparer = Comparer<TKey>.Default;
            }

            if (ascending)
            {
                list.Sort((a, b) => comparer.Compare(selector(a), selector(b)));
            }
            else
            {
                list.Sort((a, b) => comparer.Compare(selector(b), selector(a)));
            }
        }
    }
}
