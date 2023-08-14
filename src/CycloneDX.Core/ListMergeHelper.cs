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
                        Console.WriteLine($"Warning: List-Merge for BomEntity failed to find a Merge() helper method: {list1.GetType().ToString()} and {list2.GetType().ToString()}");
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
    }
}
