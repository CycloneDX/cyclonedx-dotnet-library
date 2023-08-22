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
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CycloneDX.Models
{
    [Serializable]
    public class BomEntityConflictException : Exception
    {
        public BomEntityConflictException()
            : base("Unresolvable conflict in Bom entities")
        { }

        public BomEntityConflictException(Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}", type))
        { }

        public BomEntityConflictException(string msg)
            : base(String.Format("Unresolvable conflict in Bom entities: {0}", msg))
        { }

        public BomEntityConflictException(string msg, Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}: {1}", type, msg))
        { }
    }

    [Serializable]
    public class BomEntityIncompatibleException : Exception
    {
        public BomEntityIncompatibleException()
            : base("Comparing incompatible Bom entities")
        { }

        public BomEntityIncompatibleException(Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}", type1, type2))
        { }

        public BomEntityIncompatibleException(string msg)
            : base(String.Format("Comparing incompatible Bom entities: {0}", msg))
        { }

        public BomEntityIncompatibleException(string msg, Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}: {2}", type1, type2, msg))
        { }
    }

    /// <summary>
    /// Global configuration helper for ListMergeHelper<T>,
    /// BomEntityListMergeHelper<T>, Merge.cs implementations
    /// and related codebase.
    /// </summary>
    public class BomEntityListMergeHelperStrategy
    {
        /// <summary>
        /// Cause ListMergeHelper<T> to consider calling
        /// the BomEntityListMergeHelper->Merge which in
        /// turn calls BomEntity->MergeWith() in a loop,
        /// vs. just comparing entities for equality and
        /// deduplicating based on that (goes faster but
        /// may cause data structure not conforming to spec)
        /// </summary>
        public bool useBomEntityMerge { get; set; }

        /// <summary>
        /// CycloneDX spec version.
        /// </summary>
        public SpecificationVersion specificationVersion { get; set; }

        /// <summary>
        /// Return reasonable default strategy settings.
        /// </summary>
        /// <returns>A new ListMergeHelperStrategy instance
        /// which the callers can tune to their liking.</returns>
        public static BomEntityListMergeHelperStrategy Default()
        {
            return new BomEntityListMergeHelperStrategy
            {
                useBomEntityMerge = true,
                specificationVersion = SpecificationVersionHelpers.CurrentVersion
            };
        }
    }

    public class BomEntityListMergeHelper<T> where T : BomEntity
    {
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

            List<T> result = new List<T>();

            // Note: no blind checks for null/empty inputs - part of logic below,
            // in order to surely de-duplicate even single incoming lists.
            if (!listMergeHelperStrategy.useBomEntityMerge)
            {
                // Most BomEntity classes are not individually IEquatable to avoid the
                // copy-paste coding overhead, however they inherit the Equals() and
                // GetHashCode() methods from their base class.
                if (iDebugLevel >= 1)
                {
                    Console.WriteLine($"List-Merge (quick and careless) for BomEntity-derived types: {list1?.GetType()?.ToString()} and {list2?.GetType()?.ToString()}");
                }

                List<int> hashList = new List<int>();
                List<int> hashList2 = new List<int>();

                // Exclude possibly pre-existing identical entries first, then similarly
                // handle data from the second list. Here we have the "benefit" of lack
                // of real content merging, so already saved items (and their hashes)
                // can be treated as immutable.
                if (!(list1 is null) && list1.Count > 0)
                {
                    foreach (T item1 in list1)
                    {
                        if (item1 is null)
                        {
                            continue;
                        }
                        int hash1 = item1.GetHashCode();
                        if (hashList.Contains(hash1))
                        {
                            if (iDebugLevel >= 1)
                            {
                                Console.WriteLine($"LIST-MERGE: hash table claims duplicate data in original list1: ${item1.SerializeEntity()}");
                            }
                            continue;
                        }
                        result.Add(item1);
                        hashList.Add(hash1);
                    }
                }

                if (!(list2 is null) && list2.Count > 0)
                {
                    foreach (T item2 in list2)
                    {
                        if (item2 is null)
                        {
                            continue;
                        }
                        int hash2 = item2.GetHashCode();

                        // For info (track if data is bad or hash is unreliably weak):
                        if (iDebugLevel >= 1)
                        {
                            if (hashList2.Contains(hash2))
                            {
                                Console.WriteLine($"LIST-MERGE: hash table claims duplicate data in original list2: ${item2.SerializeEntity()}");
                            }
                            hashList2.Add(hash2);
                        }

                        if (hashList.Contains(hash2))
                        {
                            continue;
                        }
                        result.Add(item2);
                        hashList.Add(hash2);
                    }
                }

                return result;
            }

            // Here both lists are assumed to possibly have same or equivalent
            // entries, even inside the same original list (e.g. if prepared by
            // quick logic above for de-duplicating the major bulk of content).
            Type TType = ((!(list1 is null) && list1.Count > 0) ? list1[0] : list2[0]).GetType();

            if (iDebugLevel >= 1)
            {
                Console.WriteLine($"List-Merge (careful) for BomEntity derivatives: {TType.ToString()}");
            }

            if (!BomEntity.KnownTypeMergeWith.TryGetValue(TType, out var methodMergeWith))
            {
                methodMergeWith = null;
            }

            // Compact version of loop below; see comments there.
            // In short, we avoid making a plain copy of list1 so
            // we can carefully pass each entry to MergeWith()
            // any suitable other in the same original list.
            if (!(list1 is null) && list1.Count > 0)
            {
                foreach (var item0 in list1)
                {
                    bool resMerge = false;
                    for (int i=0; i < result.Count; i++)
                    {
                        var item1 = result[i];
                        if (methodMergeWith != null)
                        {
                            resMerge = (bool)methodMergeWith.Invoke(item1, new object[] {item0});
                        }
                        else
                        {
                            resMerge = item1.MergeWith(item0);
                        }

                        if (resMerge)
                        {
                            break; // item2 merged into result[item1] or already equal to it
                        }
                    }

                    if (!resMerge)
                    {
                        result.Add(item0);
                    }
                }
            }

            // Similar logic to the pass above, but with optional logging to
            // highlight results of merges of the second list into the first.
            if (!(list2 is null) && list2.Count > 0)
            {
                foreach (var item2 in list2)
                {
                    bool isContained = false;
                    if (iDebugLevel >= 3)
                    {
                        Console.WriteLine($"result<{TType.ToString()}> now contains {result.Count} entries");
                    }

                    for (int i=0; i < result.Count; i++)
                    {
                        if (iDebugLevel >= 3)
                        {
                            Console.WriteLine($"result<{TType.ToString()}>: checking entry #{i}");
                        }
                        var item1 = result[i];

                        // Squash contents of the new entry with an already
                        // existing equivalent (same-ness is subject to
                        // IEquatable<>.Equals() checks defined in respective
                        // classes), if there is a method defined there.
                        // For BomEntity descendant instances we assume that
                        // they have Equals(), Equivalent() and MergeWith()
                        // methods defined or inherited as is suitable for
                        // the particular entity type, hence much less code
                        // and error-checking than there was in the PoC:
                        bool resMerge;
                        if (methodMergeWith != null)
                        {
                            resMerge = (bool)methodMergeWith.Invoke(item1, new object[] {item2});
                        }
                        else
                        {
                            resMerge = item1.MergeWith(item2);
                        }
                        // MergeWith() may throw BomEntityConflictException which we
                        // want to propagate to users - their input data is confusing.
                        // Probably should not throw BomEntityIncompatibleException
                        // unless the lists truly are of mixed types.

                        if (resMerge)
                        {
                            isContained = true;
                            break; // item2 merged into result[item1] or already equal to it
                        }
                    }

                    if (isContained)
                    {
                        if (iDebugLevel >= 2)
                        {
                            Console.WriteLine($"ALREADY THERE: {item2.ToString()}");
                        }
                    }
                    else
                    {
                        // Add new entry "as is" (new-ness is subject to
                        // equality checks of respective classes):
                        if (iDebugLevel >= 2)
                        {
                            Console.WriteLine($"WILL ADD: {item2.ToString()}");
                        }
                        result.Add(item2);
                    }
                }
            }

            return result;
        }
    }

    public class BomEntityListReflection
    {
        public Type genericType { get; set; }
        public PropertyInfo propCount { get; set; }
        public MethodInfo methodAdd { get; set; }
        public MethodInfo methodAddRange { get; set; }
        public MethodInfo methodGetItem { get; set; }
        public MethodInfo methodSort { get; set; }
        public MethodInfo methodReverse { get; set; }
    }

    public class BomEntityListMergeHelperReflection
    {
        public Type genericType { get; set; }
        public MethodInfo methodMerge { get; set; }
        public Object helperInstance { get; set; }
    }

    /// <summary>
    /// BomEntity is intended as a base class for other classes in CycloneDX.Models,
    /// which in turn encapsulate different concepts and data types described by
    /// the specification. It allows them to share certain behaviors such as the
    /// ability to determine "equivalent but not equal" objects (e.g. two instances
    /// of a Component with the same "bom-ref" but different in some properties),
    /// and to define the logic for merge-ability of such objects while coding much
    /// of the logical scaffolding only once.
    /// </summary>
    public class BomEntity : IEquatable<BomEntity>
    {
        // Keep this info initialized once to cut down on overheads of reflection
        // when running in our run-time loops.
        // Thanks to https://stackoverflow.com/a/45896403/4715872 for the Func'y trick
        // and https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
        // TOTHINK: Should these be exposed as public or hidden even more strictly?
        //  Perhaps add getters for a copy?

        /// <summary>
        /// List of classes derived from BomEntity, prepared startically at start time.
        /// </summary>
        public static readonly ImmutableList<Type> KnownEntityTypes =
            new Func<ImmutableList<Type>>(() =>
            {
                List<Type> derived_types = new List<Type>();
                foreach (var domain_assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assembly_types = domain_assembly.GetTypes()
                        .Where(type => type.IsSubclassOf(typeof(BomEntity)) && !type.IsAbstract);

                    derived_types.AddRange(assembly_types);
                }
                return ImmutableList.Create(derived_types.ToArray());
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equals() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.PropertyInfo[]> KnownEntityTypeProperties =
            new Func<ImmutableDictionary<Type, System.Reflection.PropertyInfo[]>>(() =>
            {
                Dictionary<Type, System.Reflection.PropertyInfo[]> dict = new Dictionary<Type, System.Reflection.PropertyInfo[]>();
                foreach (var type in KnownEntityTypes)
                {
                    dict[type] = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance); // BindingFlags.DeclaredOnly
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        public static readonly ImmutableDictionary<Type, BomEntityListReflection> KnownEntityTypeLists =
            new Func<ImmutableDictionary<Type, BomEntityListReflection>>(() =>
            {
                Dictionary<Type, BomEntityListReflection> dict = new Dictionary<Type, BomEntityListReflection>();
                foreach (var type in KnownEntityTypes)
                {
                    // Inspired by https://stackoverflow.com/a/4661237/4715872
                    // to craft a List<SpecificType> "result" at run-time:
                    Type listType = typeof(List<>);
                    Type constructedListType = listType.MakeGenericType(type);
                    // Would we want to stach a pre-created helper instance as Activator.CreateInstance(constructedListType) ?

                    dict[type] = new BomEntityListReflection();
                    dict[type].genericType = constructedListType;

                    // Gotta use reflection for run-time evaluated type methods:
                    dict[type].propCount = constructedListType.GetProperty("Count");
                    dict[type].methodGetItem = constructedListType.GetMethod("get_Item");
                    dict[type].methodAdd = constructedListType.GetMethod("Add", 0, new [] { type });
                    dict[type].methodAddRange = constructedListType.GetMethod("AddRange", 0, new [] { constructedListType });

                    // Use the default no-arg implementations here explicitly,
                    // to avoid an System.Reflection.AmbiguousMatchException:
                    dict[type].methodSort = constructedListType.GetMethod("Sort", 0, new Type[] {});
                    dict[type].methodReverse = constructedListType.GetMethod("Reverse", 0, new Type[] {});

                    // Avoid: No cached info about BomEntityListReflection[System.Collections.Generic.List`1[CycloneDX.Models.ExternalReference]]
                    // TODO: Separate dict?..
                    dict[constructedListType] = dict[type];
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        public static readonly ImmutableDictionary<Type, BomEntityListMergeHelperReflection> KnownBomEntityListMergeHelpers =
            new Func<ImmutableDictionary<Type, BomEntityListMergeHelperReflection>>(() =>
            {
                Dictionary<Type, BomEntityListMergeHelperReflection> dict = new Dictionary<Type, BomEntityListMergeHelperReflection>();
                foreach (var type in KnownEntityTypes)
                {
                    // Inspired by https://stackoverflow.com/a/4661237/4715872
                    // to craft a List<SpecificType> "result" at run-time:
                    Type listHelperType = typeof(BomEntityListMergeHelper<>);
                    Type constructedListHelperType = listHelperType.MakeGenericType(type);
                    var helper = Activator.CreateInstance(constructedListHelperType);
                    Type LType = null;
                    if (KnownEntityTypeLists.TryGetValue(type, out BomEntityListReflection refInfo))
                    {
                        LType = refInfo.genericType;
                    }

                    if (LType != null)
                    {
                        // Gotta use reflection for run-time evaluated type methods:
                        var methodMerge = constructedListHelperType.GetMethod("Merge", 0, new [] { LType, LType, typeof(BomEntityListMergeHelperStrategy) });
                        if (methodMerge != null)
                        {
                            dict[type] = new BomEntityListMergeHelperReflection();
                            dict[type].genericType = constructedListHelperType;
                            dict[type].methodMerge = methodMerge;
                            dict[type].helperInstance = helper;
                            // Callers would return something like (List<T>)methodMerge.Invoke(helper, new object[] {list1, list2})
                        }
                        else
                        {
                            // Should not get here, but if we do - make noise
                            throw new InvalidOperationException($"BomEntityListMergeHelper<{type}> lacks a Merge() helper method");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"BomEntityListMergeHelper<{type}> lacks a List class definition");
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about custom CycloneDX.Json.Serializer.Serialize()
        /// implementations (if present), prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownTypeSerializers =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                var jserClassType = typeof(CycloneDX.Json.Serializer);
                var methodDefault = jserClassType.GetMethod("Serialize",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
                    new [] { typeof(BomEntity) });
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = jserClassType.GetMethod("Serialize",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method != null && method != methodDefault)
                    {
                        dict[type] = method;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equals() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownTypeEquals =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equals",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method != null)
                    {
                        dict[type] = method;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownDefaultEquals =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                var methodDefault = typeof(BomEntity).GetMethod("Equals",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    new [] { typeof(BomEntity) });
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equals",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method == null)
                    {
                        dict[type] = methodDefault;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        // Our loops check for some non-BomEntity typed value equalities,
        // so cache their methods if present. Note that this one retains
        // the "null" results to mark that we do not need to look further.
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownOtherTypeEquals =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                var listMore = new List<Type>();
                listMore.Add(typeof(string));
                listMore.Add(typeof(bool));
                listMore.Add(typeof(int));
                foreach (var type in listMore)
                {
                    var method = type.GetMethod("Equals",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    dict[type] = method;
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equivalent() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownTypeEquivalent =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equivalent",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method != null)
                    {
                        dict[type] = method;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownDefaultEquivalent =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                var methodDefault = typeof(BomEntity).GetMethod("Equivalent",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    new [] { typeof(BomEntity) });
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equivalent",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method == null)
                    {
                        dict[type] = methodDefault;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom MergeWith() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownTypeMergeWith =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("MergeWith",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new [] { type });
                    if (method != null)
                    {
                        dict[type] = method;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom static NormalizeList() method
        /// implementations (if present) for sorting=>normalization of lists
        /// of that BomEntity-derived type, prepared startically at start time.
        /// </summary>
        public static readonly ImmutableDictionary<Type, System.Reflection.MethodInfo> KnownTypeNormalizeList =
            new Func<ImmutableDictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("NormalizeList",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                        new [] { typeof(bool), typeof(bool), typeof(List<BomEntity>) });
                    if (method != null)
                    {
                        dict[type] = method;
                    }
                }
                return ImmutableDictionary.CreateRange(dict);
            }) ();

        protected BomEntity()
        {
            // a bad alternative to private could be to: throw new NotImplementedException("The BomEntity class directly should not be instantiated")
        }

        /// <summary>
        /// Helper for comparisons and getting object hash code.
        /// Calls our standard CycloneDX.Json.Serializer to use
        /// its common options in particular.
        /// </summary>
        internal string SerializeEntity()
        {
            // Do we have a custom serializer defined? Use it!
            // (One for BomEntity tends to serialize this base class
            // so comes up empty, or has to jump through hoops...)
            Type thisType = this.GetType();
            if (KnownTypeSerializers.TryGetValue(thisType, out var methodSerializeThis))
            {
                var res1 = (string)methodSerializeThis.Invoke(null, new object[] {this});
                return res1;
            }

            var res = CycloneDX.Json.Serializer.SerializeCompact(this);
            return res;
        }

        /// <summary>
        /// NOTE: Class methods do not "override" this one because they compare to their type
        /// and not to the base BomEntity type objects. They should also not call this method
        /// to avoid looping - implement everything needed there directly, if ever needed!
        /// Keep in mind that the base implementation calls the SerializeEntity() method which
        /// should be by default aware and capable of ultimately serializing the properties
        /// relevant to each derived class.
        /// </summary>
        /// <param name="obj">Another BomEntity-derived object of same type</param>
        /// <returns>True if two objects are deemed equal</returns>
        public bool Equals(BomEntity obj)
        {
            Type thisType = this.GetType();
            if (KnownTypeEquals.TryGetValue(thisType, out var methodEquals))
            {
                return (bool)methodEquals.Invoke(this, new object[] {obj});
            }

            if (obj is null || thisType != obj.GetType())
            {
                return false;
            }
            return this.SerializeEntity() == obj.SerializeEntity();
        }

        // Needed by IEquatable contract
        public override bool Equals(Object obj)
        {
            if (obj is null || !(obj is BomEntity))
            {
                return false;
            }
            return this.Equals((BomEntity)obj);
        }

        /// <summary>
        /// Returns hash code of the string returned by
        /// `this.SerializeEntity()` (typically a compact
        /// JSON representation) plus the length of this
        /// string to randomize it a bit against hash
        /// collisions. Never saw those, but just in case.
        /// </summary>
        /// <returns>Int hash code</returns>
        public override int GetHashCode()
        {
            string ser = this.SerializeEntity();
            return ser.GetHashCode() + ser.Length;
        }

        /// <summary>
        /// Do this and other objects describe the same real-life entity?
        /// "Override" this in sub-classes that have a more detailed definition of
        /// equivalence (e.g. that certain fields are equal even if whole contents
        /// are not) by defining an implementation tailored to that derived type
        /// as the argument, or keep this default where equiality is equivalence.
        /// </summary>
        /// <param name="obj">Another object of same type</param>
        /// <returns>True if two data objects are considered to represent
        /// the same real-life entity, False otherwise.</returns>
        public bool Equivalent(BomEntity obj)
        {
            Type thisType = this.GetType();
            if (KnownTypeEquivalent.TryGetValue(thisType, out var methodEquivalent))
            {
                // Note we do not check for null/type of "obj" at this point
                // since the derived classes define the logic of equivalence
                // (possibly to other entity subtypes as well).
                return (bool)methodEquivalent.Invoke(this, new object[] {obj});
            }

            // Note that here a default Equivalent() may call into custom Equals(),
            // so the similar null/type sanity shecks are still relevant.
            return (!(obj is null) && (thisType == obj.GetType()) && this.Equals(obj));
        }

        /// <summary>
        /// In-place normalization of a list of BomEntity-derived type.
        /// Derived classes can implement this as a sort by one or more
        /// of specific properties (e.g. name or bom-ref). Note that
        /// handling of the "recursive" option is commonly handled in
        /// the base-class method, via which these should be called.
        /// Being a static method, those in derived classes are not
        /// overrides for the PoV of the language.
        ///
        /// Ordering proposed in these methods is an educated guess.
        /// Main purpose for this is to have some consistently ordered
        /// serialized BomEntity lists for the purposes of comparison
        /// and compression.
        ///
        /// TODO: this should really be offloaded as lambdas into the
        /// BomEntity-derived classes themselves, but I've struggled
        /// to cast the right magic spells at C# to please its gods.
        /// In particular, the ValueTuple used in selector signature is
        /// both generic for the values' types (e.g. <string, bool, int>),
        /// and for their amount in the tuple (0, 1, 2, ... explicitly
        /// stated). So this is the next best thing...
        /// </summary>
        public static void NormalizeList(bool ascending, bool recursive, List<BomEntity> list)
        {
            if (list is null || list.Count < 2)
            {
                // No-op quickly for null, empty or single-item lists
                return;
            }

            Type thisType = list[0].GetType();

            if (recursive)
            {
                // Look into properties of each currently listed BomEntity-derived
                // type instance, so if there are further lists - sort them similarly.
                PropertyInfo[] properties = BomEntity.KnownEntityTypeProperties[thisType];
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(List<Object>) || property.PropertyType.ToString().StartsWith("System.Collections.Generic.List"))
                    {
                        // Re-use these learnings while we iterate all original
                        // list items regarding the specified sub-list property:
                        Type LType = null;
                        Type TType = null;
                        PropertyInfo propCount = null;
                        MethodInfo methodGetItem = null;
                        MethodInfo methodSort = null;
                        MethodInfo methodReverse = null;
                        MethodInfo methodNormalizeSubList = null;
                        bool retryMethodNormalizeSubList = true;

                        foreach(var obj in list)
                        {
                            if (obj is null)
                            {
                                continue;
                            }

                            try
                            {
                                // Use cached info where available for all the
                                // list and list-item types and methods involved.

                                // Get the (list) property of the originally iterated
                                // BomEntity-derived item from our original list:
                                var propValObj = property.GetValue(obj);

                                // Is that sub-list trivial enough to skip?
                                if (propValObj is null)
                                {
                                    continue;
                                }

                                if (LType == null)
                                {
                                    LType = propValObj.GetType();
                                }

                                // Learn how to query that LType sort of lists:
                                if (methodGetItem == null || propCount == null || methodSort == null || methodReverse == null)
                                {
                                    if (BomEntity.KnownEntityTypeLists.TryGetValue(LType, out BomEntityListReflection refInfo))
                                    {
                                        propCount = refInfo.propCount;
                                        methodGetItem = refInfo.methodGetItem;
                                        methodSort = refInfo.methodSort;
                                        methodReverse = refInfo.methodReverse;
                                    }
                                    else
                                    {
                                        propCount = LType.GetProperty("Count");
                                        methodGetItem = LType.GetMethod("get_Item");
                                        methodSort = LType.GetMethod("Sort");
                                        methodReverse = LType.GetMethod("Reverse");
                                    }

                                    if (methodGetItem == null || propCount == null || methodSort == null || methodReverse == null)
                                    {
                                        // is this really a LIST - it lacks a get_Item() or other methods, or a Count property
                                        continue;
                                    }
                                }

                                // Is that sub-list trivial enough to skip?
                                int propValObjCount = (int)propCount.GetValue(propValObj, null);
                                if (propValObjCount < 2)
                                {
                                    continue;
                                }

                                // Type of items in that sub-list:
                                if (TType == null)
                                {
                                    TType = methodGetItem.Invoke(propValObj, new object[] { 0 }).GetType();
                                }

                                // Learn how to sort the sub-list of those item types:
                                if (methodNormalizeSubList == null && retryMethodNormalizeSubList)
                                {
                                    if (!KnownTypeNormalizeList.TryGetValue(TType, out var methodNormalizeSubListTmp))
                                    {
                                        methodNormalizeSubListTmp = null;
                                        retryMethodNormalizeSubList = false;
                                    }
                                    methodNormalizeSubList = methodNormalizeSubListTmp;
                                }

                                if (methodNormalizeSubList != null)
                                {
                                    // call static NormalizeList(..., List<TType> obj.propValObj)
                                    methodNormalizeSubList.Invoke(null, new object[] {ascending, recursive, propValObj});
                                }
                                else
                                {
                                    // Default-sort a common sub-list directly (no recursion)
                                    methodSort.Invoke(propValObj, null);
                                    if (!ascending)
                                    {
                                        methodReverse.Invoke(propValObj, null);
                                    }
                                }
                            }
                            catch (System.InvalidOperationException)
                            {
                                // property.GetValue(obj) failed
                                continue;
                            }
                            catch (System.Reflection.TargetInvocationException)
                            {
                                // property.GetValue(obj) failed
                                continue;
                            }
                        }
                    }
                }
            }

            if (KnownTypeNormalizeList.TryGetValue(thisType, out var methodNormalizeList))
            {
                // Note we do not check for null/type of "obj" at this point
                // since the derived classes define the logic of equivalence
                // (possibly to other entity subtypes as well).
                methodNormalizeList.Invoke(null, new object[] {ascending, recursive, list});
                return;
            }

            // Expensive but reliable default implementation (modulo differently
            // sorted lists of identical item sets inside the otherwise identical
            // objects -- but currently spec seems to mean ordered collections);
            // classes are welcome to implement theirs eventually or switch cases
            // above currently.
            var sortHelper = new ListMergeHelper<BomEntity>();
            sortHelper.SortByImpl(ascending, recursive, list,
                o => (o?.SerializeEntity()),
                null);
        }

        /// <summary>
        /// Default implementation just "agrees" that Equals()==true objects
        /// are already merged (returns true), and that Equivalent()==false
        /// objects are not (returns false), and for others (equivalent but
        /// not equal, or different types) raises an exception.
        /// Treats a null "other" object as a success (it is effectively a
        /// no-op merge, which keeps "this" object as is).
        /// </summary>
        /// <param name="obj">Another object of same type whose additional
        /// non-conflicting data we try to squash into this object.</param>
        /// <returns>True if merge was successful, False if it these objects
        /// are not equivalent, or throws if merge can not be done (including
        /// lack of merge logic or unresolvable conflicts in data points).
        /// </returns>
        /// <exception cref="BomEntityConflictException">Source data problem: two entities with conflicting information</exception>
        /// <exception cref="BomEntityIncompatibleException">Caller error: somehow merging different entity types</exception>
        public bool MergeWith(BomEntity obj)
        {
            if (obj is null)
            {
                return true;
            }
            if (this.GetType() != obj.GetType())
            {
                // Note: potentially descendent classes can catch this
                // to adapt their behavior... if some two different
                // classes would ever describe something comparable
                // in real life.
                throw new BomEntityIncompatibleException(this.GetType(), obj.GetType());
            }

            if (this.Equals(obj))
            {
                return true;
            }
            // Avoid calling Equals => serializer twice for no gain
            // (default equivalence is equality):
            if (KnownTypeEquivalent.TryGetValue(this.GetType(), out var methodEquivalent))
            {
                if (!this.Equivalent(obj))
                {
                    return false;
                }
                // else fall through to exception below
            }
            else
            {
                return false; // known not equal => not equivalent by default => false
            }

            // Normal mode of operation: descendant classes catch this
            // exception to use their custom non-trivial merging logic.
            throw new BomEntityConflictException(
                "Base-method implementation treats equivalent but not equal entities as conflicting",
                this.GetType());
        }
    }
}
