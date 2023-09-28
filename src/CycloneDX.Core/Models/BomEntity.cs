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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml;

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
        /// When merging whole Bom documents which include
        /// Equivalent() Components (and probably references
        /// back to them in respective Dependencies[] lists)
        /// with differing values of Scope (required or null
        /// vs. optional vs. excluded), do not conflate them
        /// but instead rename the two siblings' values of
        /// "bom-ref", suffixing the ":scope" - including
        /// the back-references from locations known by spec.
        /// Also consider equality of non-null Dependencies
        /// pointing back to their same BomRef value in the
        /// two original Bom documents (notably honouring the
        /// explicitly empty "dependsOn" lists -- NOT NULL).
        ///
        /// This is partially orthogonal to useBomEntityMerge
        /// setting which would allow to populate missing
        /// data points using an incoming Component object:
        /// * "partially" being that when two Components would
        ///   be inspected by MergeWith(), the possibiliy of
        ///   such suffix would be considered among equality
        ///   criteria (not exact equality of BomRef props).
        /// * "orthogonal" relating to the fact that this conflict
        ///   inspection aims to be a quick pre-processing stage
        ///   similar to quick merge (useBomEntityMerge==false)
        ///   and modifies the incoming list of Bom documents
        ///   before that quick merge, with a targeted solution
        ///   cheaper than a full MergeWith() iteration.
        ///
        /// This is a bit costlier in processing, but safer in
        /// pedantic approach, than the known alternatives:
        /// * Just following "useBomEntityMerge" to the letter,
        ///   comparing for exact equality of serialization of
        ///   the two objects -- two or more copies of the same
        ///   BomRef value assigned to different but related
        ///   "real-life" entities can appear (e.g. when "scope"
        ///   differs, like for production and testing modules)
        ///   AND different Dependencies[] entries can exist
        ///   (e.g. different Maven resolutions when building
        ///   a Java ecosystem library vs. an app using it,
        ///   with different dependencyManagement preferences).
        ///   Due to this, we can not quickly conflate "purely
        ///   equal" entities as the first pass when such
        ///   nuanced inequalities can arise.
        /// * Brutely conflating the Components with different
        ///   Scopes ("optional" becomes "required" if something
        ///   else in the overall merged product did require it)
        ///   can backfire if the merged document describes an
        ///   end-user bundle of a number of products: their
        ///   separate programs (or even containers) do still have
        ///   their separate dependency trees, so "app A" requiring
        ///   a library does not mean that "app B" which had it as
        ///   optional suddenly requires it now -- and maybe gets
        ///   false-positive vulnerabilities reported due to that.
        ///   For merged Bom documents describing a single linker
        ///   namespace such conflation may in fact be valid however.
        /// </summary>
        public bool renameConflictingComponents { get; set; }

        /// <summary>
        /// CycloneDX spec version.
        /// </summary>
        public SpecificationVersion specificationVersion { get; set; }

        /// <summary>
        /// Used by interim Merge.FlatMerge(bom1, bom2) in a loop
        /// context -- defaulting to `false` to reduce compute
        /// load for results we would discard. Can be set to `true`
        /// by some other use-cases that would invoke that method.
        /// Does not impact the Merge.FlatMerge(Iterable<Bom>) variant.
        ///
        /// See also: doBomMetadataUpdateNewSerialNumber,
        /// doBomMetadataUpdateReferThisToolkit
        /// </summary>
        public bool doBomMetadataUpdate { get; set; }
        /// <summary>See doBomMetadataUpdate description.</summary>
        public bool doBomMetadataUpdateNewSerialNumber { get; set; }
        /// <summary>See doBomMetadataUpdate description.</summary>
        public bool doBomMetadataUpdateReferThisToolkit { get; set; }

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
                renameConflictingComponents = true,
                doBomMetadataUpdate = false,
                doBomMetadataUpdateNewSerialNumber = false,
                doBomMetadataUpdateReferThisToolkit = false,
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
                            resMerge = (bool)methodMergeWith.Invoke(item1, new object[] {item0, listMergeHelperStrategy});
                        }
                        else
                        {
                            resMerge = item1.MergeWith(item0, listMergeHelperStrategy);
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
                            resMerge = (bool)methodMergeWith.Invoke(item1, new object[] {item2, listMergeHelperStrategy});
                        }
                        else
                        {
                            resMerge = item1.MergeWith(item2, listMergeHelperStrategy);
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
    /// Just a baseline interface for the big BomEntity
    /// family to formally implement. In practice all
    /// those classes are derived from BomEntity so it
    /// can dispatch calls into them when used as a
    /// generic base class, or serve default method
    /// implementations.
    /// </summary>
    public interface IBomEntity : IEquatable<IBomEntity>
    {
        public string SerializeEntity();
    }

    /// <summary>
    /// Interface assigned to BomEntity derived classes
    /// which have a property generally conforming to
    /// CycloneDX schema definition of "bom:refType"
    /// (per XML schema) or "#/definitions/refType"
    /// (per JSON schema).
    /// Such a property is usually called "bom-ref"
    /// in text representations of Bom documents and
    /// is a C# string; however some more complex type
    /// may be used in the future to multi-plex all the
    /// different referencing use-cases.
    ///
    /// For specific practical hints, see also:
    ///    IBomEntityWithRefType_String_BomRef
    /// </summary>
    public interface IBomEntityWithRefType : IBomEntity
    {
    }

    /// <summary>
    /// Interface assigned to BomEntity derived classes
    /// which have a property with a CycloneDX Bom schema
    /// "refType" attribute specifically named "BomRef"
    /// and typed as a "string" in C#. It helps to know
    /// where we can call GetBomRef() safely...
    /// </summary>
    public interface IBomEntityWithRefType_String_BomRef : IBomEntityWithRefType
    {
    }

    /// <summary>
    /// Interface assigned to BomEntity derived classes
    /// which have a property generally conforming to
    /// CycloneDX schema definition of
    /// "bom:refLinkType" (per XML schema) or
    /// "#/definitions/refLinkType" (per JSON schema).
    /// Such a property is usually called "ref"
    /// in text representations of Bom documents,
    /// but can be items in certain lists as well.
    ///
    /// Technically it follows same schema definition
    /// as a "refType" but is intended (since CDX 1.5)
    /// to specify links pointing to someone else's
    /// "bom-ref" values.
    ///
    /// For specific practical hints, see also:
    ///    IBomEntityWithRefLinkType_String_Ref
    ///    IBomEntityWithRefLinkType_StringList
    /// </summary>
    public interface IBomEntityWithRefLinkType : IBomEntity
    {
        /// <summary>
        /// For each property in this class which can
        /// convey a Bom "refLinkType" (single values
        /// like a "ref" or lists full of references),
        /// clarify which classes are expected to be
        /// on the other end of the reference -- with
        /// one of their instances having the "bom-ref"
        /// identification value specified in this "ref".
        /// The CycloneDX spec details that some refs
        /// only point to a "component", others also
        /// to a "service", some to a "componentData",
        /// and some do not constrain.
        ///
        /// Note that there may be no hits in the
        /// current Bom document, and not all items
        /// with a "bom-ref" attribute would have
        /// such back-links to them defined in the
        /// same Bom document.
        /// </summary>
        /// <returns></returns>
        // FIXME: Would a C# annotation serve this cause
        //  better? Would it be faster in processing
        //  (with reflection) e.g. to *find* which
        //  properties to look at?
        public Dictionary<PropertyInfo, List<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion);
    }

    /// <summary>
    /// Interface assigned to BomEntity derived classes
    /// which have a property with a CycloneDX Bom schema
    /// "refLinkType" attribute specifically named "Ref"
    /// and typed as a "string" in C#. It helps to know
    /// where we can call GetRef() safely...
    /// </summary>
    public interface IBomEntityWithRefLinkType_String_Ref : IBomEntityWithRefLinkType
    {
    }

    /// <summary>
    /// Interface assigned to BomEntity derived classes
    /// which have one or more properties which are lists,
    /// whose items conform to CycloneDX Bom schema for
    /// "refLinkType", and are typed as a "List<string>"
    /// in C#. It helps to know where we can iterate
    /// those safely... See also GetRefLinkConstraints().
    /// </summary>
    public interface IBomEntityWithRefLinkType_StringList : IBomEntityWithRefLinkType
    {
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
    public class BomEntity : IBomEntity
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
                List<Type> KnownEntityTypesPlus = new List<Type>(KnownEntityTypes);
                KnownEntityTypesPlus.Add(typeof(BomEntity));
                foreach (var type in KnownEntityTypesPlus)
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
                        new [] { type, typeof(BomEntityListMergeHelperStrategy) });
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
                    MethodInfo method = null;

                    if (BomEntity.KnownEntityTypeLists.TryGetValue(type, out BomEntityListReflection refInfoListType))
                    {
                        method = type.GetMethod("NormalizeList",
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                            new [] { typeof(bool), typeof(bool), refInfoListType.genericType });
                        if (method != null)
                        {
                            dict[type] = method;
                            continue;
                        }
                    }

                    // Try class default
                    method = type.GetMethod("NormalizeList",
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
        public string SerializeEntity()
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
        public bool Equals(IBomEntity obj)
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
        /// stated). So this is the next best thing... even if highly
        /// inefficient to copy lists from one type to another as a
        /// fake cast. At least it works!..
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
                            }
                            catch (System.Reflection.TargetInvocationException)
                            {
                                // property.GetValue(obj) failed
                            }
                        }
                    }
                }
            }

            if (KnownTypeNormalizeList.TryGetValue(thisType, out var methodNormalizeList))
            {
                if (BomEntity.KnownEntityTypeLists.TryGetValue(thisType, out BomEntityListReflection refInfoListType))
                {
                    if (BomEntity.KnownEntityTypeLists.TryGetValue(typeof(BomEntity), out BomEntityListReflection refInfoListInterface))
                    {
                        // Note we do not check for null/type of "obj" at this point
                        // since the derived classes define the logic of equivalence
                        // (possibly to other entity subtypes as well).
                        //    methodNormalizeList.Invoke(null, new object[] {ascending, recursive, list}) does
                        // not work, alas

                        // Gotta make ugly cast copies there and back:
                        var helper = Activator.CreateInstance(refInfoListType.genericType);
                        foreach (var item in list)
                        {
                            refInfoListType.methodAdd.Invoke(helper, new object[] {item});
                        }

                        methodNormalizeList.Invoke(null, new object[] {ascending, recursive, helper});

                        // Populate back the original list object:
                        list.Clear();
                        refInfoListInterface.methodAddRange.Invoke(list, new [] {helper});
                        return;
                    }
                }
            }

            // Expensive but reliable default implementation (modulo differently
            // sorted lists of identical item sets inside the otherwise identical
            // objects -- but currently spec seems to mean ordered collections),
            // classes are welcome to implement theirs eventually or switch cases
            // above currently.
            var sortHelper = new ListMergeHelper<BomEntity>();
            sortHelper.SortByImpl(ascending, recursive, list,
                o => (o?.SerializeEntity()),
                null);
        }

        /// <summary>
        /// See NormalizeList(); this variant defaults "ascending=true"
        /// </summary>
        /// <param name="recursive">Should this recurse into child-object properties which are sub-lists?</param>
        /// <param name="list"></param>
        public static void NormalizeList(bool recursive, List<BomEntity> list)
        {
            NormalizeList(true, recursive, list);
        }

        /// <summary>
        /// <summary>
        /// See NormalizeList(); this variant defaults "ascending=true"
        /// and "recursive=false" to only normalize the given list itself.
        /// </summary>
        /// <param name="list"></param>
        public static void NormalizeList(List<BomEntity> list)
        {
            NormalizeList(true, false, list);
        }

        /// Default implementation just "agrees" that Equals()==true objects
        /// are already merged (returns true), and that Equivalent()==false
        /// objects are not (returns false), and for others (equivalent but
        /// not equal, or different types) raises an exception.
        /// Treats a null "other" object as a success (it is effectively a
        /// no-op merge, which keeps "this" object as is).
        /// </summary>
        /// <param name="obj">Another object of same type whose additional
        /// non-conflicting data we try to squash into this object.</param>
        /// <param name="listMergeHelperStrategy">A BomEntityListMergeHelperStrategy
        /// instance which relays nuances about desired merging activity.</param>
        /// <returns>True if merge was successful, False if it these objects
        /// are not equivalent, or throws if merge can not be done (including
        /// lack of merge logic or unresolvable conflicts in data points).
        /// </returns>
        /// <exception cref="BomEntityConflictException">Source data problem: two entities with conflicting information</exception>
        /// <exception cref="BomEntityIncompatibleException">Caller error: somehow merging different entity types</exception>
        public bool MergeWith(BomEntity obj, BomEntityListMergeHelperStrategy listMergeHelperStrategy)
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

    /// <summary>
    /// Helper class for Bom.GetBomRefsInContainers() et al discovery tracking.
    /// </summary>
    public class BomWalkResult
    {
        /// <summary>
        /// The BomEntity (normally a whole Bom document)
        /// which was walked and reported here.
        /// </summary>
        public BomEntity bomRoot = null;

        /// <summary>
        /// Populated by GetBomRefsInContainers(),
        /// keys are "container" entities and values
        /// are lists of "contained" entities which
        /// have a BomRef or equivalent property.
        /// </summary>
        readonly public Dictionary<BomEntity, List<BomEntity>> dictRefsInContainers = new Dictionary<BomEntity, List<BomEntity>>();

        /// <summary>
        /// Populated by GetBomRefsInContainers(),
        /// keys are "Ref" or equivalent string values
        /// which link back to a "BomRef" hopefully
        /// defined somewhere in the same Bom document
        /// (but may be dangling, or sometimes co-opted
        /// with external links to other Bom documents!),
        /// and values are lists of entities which use
        /// this same "ref" value.
        /// </summary>
        readonly public Dictionary<String, List<BomEntity>> dictBackrefs = new Dictionary<String, List<BomEntity>>();

        // Callers can enable performance monitoring
        // (and printing in ToString() method) to help
        // debug the data-walk overheads. Accounting
        // does have a cost (~5% for a larger 20s run).
        public bool debugPerformance = false;

        // Helpers for performance accounting - how hard
        // was it to discover the information in this
        // BomWalkResult object?
        private int sbeCountMethodEnter { get; set; }
        private int sbeCountMethodQuickExit { get; set; }
        private int sbeCountPropInfoEnter { get; set; }
        private int sbeCountPropInfoQuickExit { get; set; }
        private int sbeCountPropInfoQuickExit2 { get; set; }
        private int sbeCountPropInfo { get; set; }
        private int sbeCountPropInfo_EvalIsBomref { get; set; }
        private int sbeCountPropInfo_EvalIsNotStringBomref { get; set; }
        private int sbeCountPropInfo_EvalIsStringNotNamedBomref { get; set; }
        private int sbeCountPropInfo_EvalIsStringNotNamedRef { get; set; }
        private int sbeCountPropInfo_EvalXMLAttr { get; set; }
        private int sbeCountPropInfo_EvalJSONAttr { get; set; }
        private int sbeCountPropInfo_EvalList { get; set; }
        private int sbeCountPropInfo_EvalListQuickExit { get; set; }
        private int sbeCountPropInfo_EvalListWalk { get; set; }
        private int sbeCountNewBomRefCheckDict { get; set; }
        private int sbeCountNewBomRef { get; set; }

        // This one is null, outermost loop makes a new instance, starts and stops it:
        private Stopwatch stopWatchWalkTotal = null;
        private Stopwatch stopWatchEvalAttr = new Stopwatch();
        private Stopwatch stopWatchNewBomref = new Stopwatch();
        private Stopwatch stopWatchNewBomrefCheck = new Stopwatch();
        private Stopwatch stopWatchNewBomrefNewListSpawn = new Stopwatch();
        private Stopwatch stopWatchNewBomrefNewListInDict = new Stopwatch();
        private Stopwatch stopWatchNewBomrefListAdd = new Stopwatch();
        private Stopwatch stopWatchNewRefLink = new Stopwatch();
        private Stopwatch stopWatchGetValue = new Stopwatch();

        public void reset()
        {
            dictRefsInContainers.Clear();
            dictBackrefs.Clear();

            sbeCountMethodEnter = 0;
            sbeCountMethodQuickExit = 0;
            sbeCountPropInfoEnter = 0;
            sbeCountPropInfoQuickExit = 0;
            sbeCountPropInfoQuickExit2 = 0;
            sbeCountPropInfo = 0;
            sbeCountPropInfo_EvalIsBomref = 0;
            sbeCountPropInfo_EvalIsNotStringBomref = 0;
            sbeCountPropInfo_EvalIsStringNotNamedBomref = 0;
            sbeCountPropInfo_EvalIsStringNotNamedRef = 0;
            sbeCountPropInfo_EvalXMLAttr = 0;
            sbeCountPropInfo_EvalJSONAttr = 0;
            sbeCountPropInfo_EvalList = 0;
            sbeCountPropInfo_EvalListQuickExit = 0;
            sbeCountPropInfo_EvalListWalk = 0;
            sbeCountNewBomRefCheckDict = 0;
            sbeCountNewBomRef = 0;

            bomRoot = null;
            stopWatchWalkTotal = null;
            stopWatchEvalAttr = new Stopwatch();
            stopWatchNewBomref = new Stopwatch();
            stopWatchNewBomrefCheck = new Stopwatch();
            stopWatchNewBomrefNewListSpawn = new Stopwatch();
            stopWatchNewBomrefNewListInDict = new Stopwatch();
            stopWatchNewBomrefListAdd = new Stopwatch();
            stopWatchNewRefLink = new Stopwatch();
            stopWatchGetValue = new Stopwatch();
        }

        public void reset(BomEntity newRoot)
        {
            this.reset();
            this.bomRoot = newRoot;
        }

        private static string StopWatchToString(Stopwatch stopwatch)
        {
            string elapsed = "N/A";
            if (stopwatch != null)
            {
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopwatch.Elapsed;
                elapsed = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
            }
            return elapsed;
        }

        public override string ToString()
        {
            return "BomWalkResult: " + (debugPerformance ?
                $"Timing.WalkTotal={StopWatchToString(stopWatchWalkTotal)} " +
                $"sbeCountMethodEnter={sbeCountMethodEnter} " +
                $"sbeCountMethodQuickExit={sbeCountMethodQuickExit} " +
                $"sbeCountPropInfoEnter={sbeCountPropInfoEnter} " +
                $"sbeCountPropInfoQuickExit={sbeCountPropInfoQuickExit} " +
                $"Timing.GetValue={StopWatchToString(stopWatchGetValue)} " +
                $"sbeCountPropInfo_EvalIsBomref={sbeCountPropInfo_EvalIsBomref} " +
                $"sbeCountPropInfo_EvalIsNotStringBomref={sbeCountPropInfo_EvalIsNotStringBomref} " +
                $"sbeCountPropInfo_EvalIsStringNotNamedBomref={sbeCountPropInfo_EvalIsStringNotNamedBomref} " +
                $"sbeCountPropInfo_EvalIsStringNotNamedRef={sbeCountPropInfo_EvalIsStringNotNamedRef} " +
                $"Timing.EvalAttr={StopWatchToString(stopWatchEvalAttr)} " +
                $"sbeCountPropInfo_EvalXMLAttr={sbeCountPropInfo_EvalXMLAttr} " +
                $"sbeCountPropInfo_EvalJSONAttr={sbeCountPropInfo_EvalJSONAttr} " +
                $"Timing.NewBomRef={StopWatchToString(stopWatchNewBomref)} (" +
                $"Timing.NewBomRefCheck={StopWatchToString(stopWatchNewBomrefCheck)} " +
                $"Timing.NewBomRefNewListSpawn={StopWatchToString(stopWatchNewBomrefNewListSpawn)} " +
                $"Timing.NewBomRefNewListInDict={StopWatchToString(stopWatchNewBomrefNewListInDict)} " +
                $"Timing.NewBomRefListAdd={StopWatchToString(stopWatchNewBomrefListAdd)}) " +
                $"sbeCountNewBomRefCheckDict={sbeCountNewBomRefCheckDict} " +
                $"sbeCountNewBomRef={sbeCountNewBomRef} " +
                $"Timing.NewRefLink={StopWatchToString(stopWatchNewRefLink)} (" +
                $"sbeCountPropInfo_EvalList={sbeCountPropInfo_EvalList} " +
                $"sbeCountPropInfoQuickExit2={sbeCountPropInfoQuickExit2} " +
                $"sbeCountPropInfo_EvalListQuickExit={sbeCountPropInfo_EvalListQuickExit} " +
                $"sbeCountPropInfo_EvalListWalk={sbeCountPropInfo_EvalListWalk} " +
                $"sbeCountPropInfo={sbeCountPropInfo} "
                : "" ) +
                $"dictRefsInContainers.Count={dictRefsInContainers.Count} " +
                $"dictBackrefs.Count={dictBackrefs.Count}";
        }

        /// <summary>
        /// Helper for Bom.GetBomRefsInContainers().
        /// </summary>
        /// <param name="obj">A BomEntity instance currently being investigated</param>
        /// <param name="container">A BomEntity instance whose attribute
        ///    (or member of a List<> attribute) is currently being
        ///    investigated. May be null when starting iteration
        ///    from this.GetBomRefsInContainers() method.
        /// </param>
        public void SerializeBomEntity_BomRefs(BomEntity obj, BomEntity container)
        {
            // With CycloneDX spec 1.4 or older it might be feasible to
            // walk specific properties of the Bom instance to look into
            // their contents by known class types. As seen by excerpt
            // from the spec below, just to list the locations where a
            // "bom-ref" value can be set to identify an entity or where
            // such value can be used to refer back to that entity, such
            // approach is nearly infeasible starting with CDX 1.5 -- so
            // use of reflection below is a more sustainable choice.

            // TL:DR further details:
            //
            // Looking in schema definitions search for items that should
            // be bom-refs (whether the attributes of certain entry types,
            // or back-references from whoever uses them):
            // * in "*.schema.json" search for "#/definitions/refType", or
            // * in "*.xsd" search for "bom:refType" and its super-set for
            //   certain use-cases "bom:bomReferenceType"
            // Since CDX spec 1.5 note there is also a "refLinkType" with
            // same formal syntax as "refType" but different purpose --
            // to specify back-references (as separate from identifiers
            // of new unique entries).  Also do not confuse with bomLink,
            // bomLinkDocumentType, and bomLinkElementType which refer to
            // entities in OTHER Bom documents (or those Boms themselves).
            //
            // As of CDX spec 1.4+, a "bom-ref" attribute can be specified in:
            // * (1.4, 1.5) component/"bom-ref"
            // * (1.4, 1.5) service/"bom-ref"
            // * (1.4, 1.5) vulnerability/"bom-ref"
            // * (1.5) organizationalEntity/"bom-ref"
            // * (1.5) organizationalContact/"bom-ref"
            // * (1.5) license/"bom-ref"
            // * (1.5) license/licenseChoice/...expression.../"bom-ref"
            // * (1.5) componentEvidence/occurrences[]/"bom-ref"
            // * (1.5) compositions/"bom-ref"
            // * (1.5) annotations/"bom-ref"
            // * (1.5) modelCard/"bom-ref"
            // * (1.5) componentData/"bom-ref"
            // * (1.5) formula/"bom-ref"
            // * (1.5) workflow/"bom-ref"
            // * (1.5) task/"bom-ref"
            // * (1.5) workspace/"bom-ref"
            // * (1.5) trigger/"bom-ref"
            // and referred from:
            // * dependency/"ref" => only "component" (1.4), or
            //   "component or service" (since 1.5)
            // * dependency/"dependsOn[]" => only "component" (1.4),
            //   or "component or service" (since 1.5)
            // * (1.4, 1.5) compositions/"assemblies[]" => "component or service"
            // * (1.4, 1.5) compositions/"dependencies[]" => "component or service"
            //   ** NOTE: As of this writing, Composition.cs file
            //      defines assemblies[] and dependencies[] as lists
            //      of strings, each treated as a "ref" in class
            //      instance (de-)serializations
            // * (1.4, 1.5) vulnerability/affects/items/"ref" => "component or service"
            // * (1.5) componentEvidence/identity/tools[] => any, see spec
            //   ** NOTE: As of this writing, EvidenceTools.cs is
            //      defined as a list of strings, each treated as
            //      a "ref" in class instance (de-)serializations
            // * (1.5) annotations/subjects[] => any
            // * (1.5) modelCard/modelParameters/datasets[]/"ref" => "data component" (see "#/definitions/componentData")
            // * (1.5) resourceReferenceChoice/"ref" => any
            //
            // Notably, CDX 1.5 also introduces resourceReferenceChoice
            // which generalizes internal or external references, used in:
            // * (1.5) workflow/resourceReferences[]
            // * (1.5) task/resourceReferences[]
            // * (1.5) workspace/resourceReferences[]
            // * (1.5) trigger/resourceReferences[]
            // * (1.5) event/{source,target}
            // * (1.5) {inputType,outputType}/{source,target,resource}
            // The CDX 1.5 tasks, workflows etc. also can reference each other.
            //
            // In particular, "component" instances (e.g. per JSON
            // "#/definitions/component" spec search) can be direct
            // properties (or property arrays) in:
            // * (1.4, 1.5) component/pedigree/{ancestors,descendants,variants}
            // * (1.4, 1.5) component/components[] -- structural hierarchy (not dependency tree)
            // * (1.4, 1.5) bom/components[]
            // * (1.4, 1.5) bom/metadata/component -- 0 or 1 item about the Bom itself
            // * (1.5) bom/metadata/tools/components[] -- SW and HW tools used to create the Bom
            // * (1.5) vulnerability/tools/components[] -- SW and HW tools used to describe the vuln
            // * (1.5) formula/components[]
            //
            // Note that there may be potentially any level of nesting of
            // components in components, and compositions, among other things.
            //
            // And "service" instances (per JSON "#/definitions/service"):
            // * (1.4, 1.5) service/services[]
            // * (1.4, 1.5) bom/services[]
            // * (1.5) bom/metadata/tools/services[] -- services as tools used to create the Bom
            // * (1.5) vulnerability/tools/services[] -- services as tools used to describe the vuln
            // * (1.5) formula/services[]
            //
            // The CDX spec 1.5 also introduces "annotation" which can refer to
            // such bom-ref carriers as service, component, organizationalEntity,
            // organizationalContact.
            if (debugPerformance)
            {
                sbeCountMethodEnter++;
            }

            if (obj is null)
            {
                if (debugPerformance)
                {
                    sbeCountMethodQuickExit++;
                }
                return;
            }

            Type objType = obj.GetType();

            // Sanity-check: we do not recurse into non-BomEntity types.
            // Hopefully the compiler or runtime would not have let other obj's in...
            if (objType is null || (!(typeof(BomEntity).IsAssignableFrom(objType))))
            {
                if (debugPerformance)
                {
                    sbeCountMethodQuickExit++;
                }
                return;
            }

            bool isTimeAccounter = (stopWatchWalkTotal is null);
            if (isTimeAccounter && debugPerformance)
            {
                stopWatchWalkTotal = new Stopwatch();
                stopWatchWalkTotal.Start();
            }

            // Looking up (comparing) keys in dictRefsInContainers[] is prohibitively
            // expensive (may have to do with serialization into a string to implement
            // GetHashCode() method), so we minimize interactions with that codepath.
            // General assumption that we only look at same container once, but the
            // code should cope with more visits (possibly at a cost).
            List<BomEntity> containerList = null;

            // TODO: Prepare a similar cache with only a subset of
            // properties of interest for bom-ref search, to avoid
            // looking into known dead ends in a loop.
            PropertyInfo[] objProperties = BomEntity.KnownEntityTypeProperties[objType];
            if (objProperties.Length < 1)
            {
                objProperties = objType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
            foreach (PropertyInfo propInfo in objProperties)
            {
                if (debugPerformance)
                {
                    sbeCountPropInfoEnter++;
                }

                // We do not recurse into non-BomEntity types
                if (propInfo is null)
                {
                    // Is this expected? Maybe throw?
                    if (debugPerformance)
                    {
                        sbeCountPropInfoQuickExit++;
                    }
                    continue;
                }

                Type propType = propInfo.PropertyType;
                if (debugPerformance)
                {
                    stopWatchGetValue.Start();
                }
                if (propInfo.Name.StartsWith("NonNullable")) {
                    // It is a getter/setter-wrapped facade
                    // of a Nullable<T> for some T - skip,
                    // we would inspect the raw item instead
                    // (factual nulls would cause an exception
                    // and require a try/catch overhead here).
                    // FIXME: Is there an attribute for this,
                    // to avoid a string comparison in a loop?
                    if (debugPerformance)
                    {
                        sbeCountPropInfoQuickExit++;
                        stopWatchGetValue.Stop();
                    }
                    continue;
                }
                var propVal = propInfo.GetValue(obj, null);
                if (debugPerformance)
                {
                    stopWatchGetValue.Stop();
                }

                if (propVal is null)
                {
                    if (debugPerformance)
                    {
                        sbeCountPropInfoQuickExit++;
                    }
                    continue;
                }

                // If the type of current "obj" contains a "bom-ref", or
                // has annotations like [JsonPropertyName("bom-ref")] and
                // [XmlAttribute("bom-ref")], save it into the dictionary.
                //
                // TODO: Pedantically it would be better to either parse
                // and consult corresponding CycloneDX spec, somehow, for
                // properties which have needed schema-defined type (see
                // detailed comments in GetBomRefsInContainers() method).
                if (debugPerformance)
                {
                    sbeCountPropInfo_EvalIsBomref++;
                }
                bool propIsBomRef = false;
                bool propIsRefLink = false;
                if (propType.GetTypeInfo().IsAssignableFrom(typeof(string)))
                {
                    // NOTE: Current CycloneDX spec (1.5 and those before it)
                    // explicitly specify reference fields as a string type.
                    // Wondering if this would change in the future (more so
                    // with higher-level grouping types like "refLinkType" or
                    // "bomLink", or generic "link to somewhere" such as
                    // "anyOf refLinkType or bomLinkElementType") which are
                    // a frequent occurrence starting from CDX spec 1.5...
                    propIsBomRef = (propInfo.Name == "BomRef");
                    if (!propIsBomRef)
                    {
                        if (debugPerformance)
                        {
                            sbeCountPropInfo_EvalIsStringNotNamedBomref++;
                        }
                        propIsRefLink = (propInfo.Name == "Ref");
                    }
                    if (!propIsRefLink)
                    {
                        if (debugPerformance)
                        {
                            sbeCountPropInfo_EvalIsStringNotNamedRef++;
                        }
                        if (!propIsBomRef)
                        {
                            if (debugPerformance)
                            {
                                sbeCountPropInfo_EvalXMLAttr++;
                                stopWatchEvalAttr.Start();
                            }
                            object[] attrs = propInfo.GetCustomAttributes(typeof(XmlAttribute), false);
                            if (attrs.Length > 0)
                            {
                                propIsBomRef = (Array.Find(attrs, x => ((XmlAttribute)x).Name == "bom-ref") != null);
                            }
                            if (debugPerformance)
                            {
                                stopWatchEvalAttr.Stop();
                            }
                        }
                        if (!propIsBomRef)
                        {
                            if (debugPerformance)
                            {
                                sbeCountPropInfo_EvalJSONAttr++;
                                stopWatchEvalAttr.Start();
                            }
                            object[] attrs = propInfo.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false);
                            if (attrs.Length > 0)
                            {
                                propIsBomRef = (Array.Find(attrs, x => ((JsonPropertyNameAttribute)x).Name == "bom-ref") != null);
                            }
                            if (debugPerformance)
                            {
                                stopWatchEvalAttr.Stop();
                            }
                        }
                    }
                }
                else
                {
                    if (debugPerformance)
                    {
                        sbeCountPropInfo_EvalIsNotStringBomref++;
                    }
                }

                if (propIsBomRef)
                {
                    // Save current object into tracking, and be done with this prop!
                    if (debugPerformance)
                    {
                        stopWatchNewBomref.Start();
                    }
                    if (containerList is null)
                    {
                        if (debugPerformance)
                        {
                            sbeCountNewBomRefCheckDict++;
                            stopWatchNewBomrefCheck.Start();
                        }
                        // "proper" dict key lookup probably goes via hashes
                        // which go via serialization for BomEntity classes,
                        // and so walking a Bom with a hundred Components
                        // takes a second with "apparent" loop like:
                        //    if (dictRefsInContainers.TryGetValue(container, out List<BomEntity> list))
                        // but takes miniscule fractions as it should, when
                        // we avoid hashing like this (and also maintain
                        // consistent references if original objects get
                        // modified - so serialization and hash changes;
                        // this should not happen in this loop, and the
                        // intention is to keep tabs on references to all
                        // original objects so we can rename what we need):
                        foreach (var (cont, list) in dictRefsInContainers)
                        {
                            if (Object.ReferenceEquals(container, cont))
                            {
                                containerList = list;
                                break;
                            }
                        }
                        if (debugPerformance)
                        {
                            stopWatchNewBomrefCheck.Stop();
                        }

                        if (containerList is null)
                        {
                            if (debugPerformance)
                            {
                                stopWatchNewBomrefNewListSpawn.Start();
                            }
                            containerList = new List<BomEntity>();
                            if (debugPerformance)
                            {
                                stopWatchNewBomrefNewListSpawn.Stop();
                                stopWatchNewBomrefNewListInDict.Start();
                            }
                            dictRefsInContainers[container] = containerList;
                            if (debugPerformance)
                            {
                                stopWatchNewBomrefNewListInDict.Stop();
                            }
                        }
                    }

                    if (debugPerformance)
                    {
                        sbeCountNewBomRef++;
                        stopWatchNewBomrefListAdd.Start();
                    }
                    containerList.Add((BomEntity)obj);
                    if (debugPerformance)
                    {
                        stopWatchNewBomrefListAdd.Stop();
                        stopWatchNewBomref.Stop();
                    }

                    // Done with this (string) property, look at next
                    continue;
                }

                if (propIsRefLink)
                {
                    // Save current object into "back-reference" tracking,
                    // and be done with this prop!
                    // Note: this approach covers only string "ref" properties,
                    // but not those few with a "List<string>" at the moment!
                    // Note: It is currently somewhat up to the consumer
                    // of these results to guess (or find) which "obj"
                    // property is the reference (currently tends to be
                    // called "Ref", but...). For the greater purposes of
                    // entities' "bom-ref" renaming this could surely be
                    // optimized.
                    if (debugPerformance)
                    {
                        stopWatchNewRefLink.Start();
                    }

                    string sPropVal = (string)propVal;
                    // nullness ruled out above
                    if (sPropVal.Trim() == "")
                    {
                        continue;
                    }

                    if (!(dictBackrefs.TryGetValue(sPropVal, out List<BomEntity> listBackrefs)))
                    {
                        listBackrefs = new List<BomEntity>();
                        dictBackrefs[sPropVal] = listBackrefs;
                    }
                    listBackrefs.Add(obj);

                    if (debugPerformance)
                    {
                        stopWatchNewRefLink.Stop();
                    }

                    // Done with this (string) property, look at next
                    continue;
                }

                // We do not recurse into non-BomEntity types
                if (debugPerformance)
                {
                    sbeCountPropInfo_EvalList++;
                }
                bool propIsListBomEntity = (
                    (propType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(System.Collections.IList)))
                    && (Array.Find(propType.GetTypeInfo().GenericTypeArguments,
                        x => typeof(BomEntity).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo())) != null)
                );

                if (!(
                    propIsListBomEntity
                    || (typeof(BomEntity).GetTypeInfo().IsAssignableFrom(propType.GetTypeInfo()))
                ))
                {
                    // Not a BomEntity or (potentially) a List of those
                    if (debugPerformance)
                    {
                        sbeCountPropInfoQuickExit2++;
                    }
                    continue;
                }

                if (propIsListBomEntity)
                {
                    // Use cached info where available
                    PropertyInfo listPropCount = null;
                    MethodInfo listMethodGetItem = null;
                    MethodInfo listMethodAdd = null;
                    if (BomEntity.KnownEntityTypeLists.TryGetValue(propType, out BomEntityListReflection refInfo))
                    {
                        listPropCount = refInfo.propCount;
                        listMethodGetItem = refInfo.methodGetItem;
                        listMethodAdd = refInfo.methodAdd;
                    }
                    else
                    {
                        // No cached info about BomEntityListReflection[{propType}
                        listPropCount = propType.GetProperty("Count");
                        listMethodGetItem = propType.GetMethod("get_Item");
                        listMethodAdd = propType.GetMethod("Add");
                    }

                    if (listMethodGetItem == null || listPropCount == null || listMethodAdd == null)
                    {
                        // Should not have happened, but...
                        if (debugPerformance)
                        {
                            sbeCountPropInfo_EvalListQuickExit++;
                        }
                        continue;
                    }

                    int propValCount = (int)listPropCount.GetValue(propVal, null);
                    if (propValCount < 1)
                    {
                        // Empty list
                        if (debugPerformance)
                        {
                            sbeCountPropInfo_EvalListQuickExit++;
                        }
                        continue;
                    }

                    if (debugPerformance)
                    {
                        sbeCountPropInfo_EvalListWalk++;
                    }
                    for (int o = 0; o < propValCount; o++)
                    {
                        var listVal = listMethodGetItem.Invoke(propVal, new object[] { o });
                        if (listVal is null)
                        {
                            continue;
                        }

                        if (!(listVal is BomEntity))
                        {
                            break;
                        }

                        SerializeBomEntity_BomRefs((BomEntity)listVal, obj);
                    }

                    // End of list, or a break per above
                    continue;
                }

                if (debugPerformance)
                {
                    sbeCountPropInfo++;
                }
                SerializeBomEntity_BomRefs((BomEntity)propVal, obj);
            }

            if (isTimeAccounter && debugPerformance)
            {
                stopWatchWalkTotal.Stop();
            }
        }

        /// <summary>
        /// Provide a Dictionary whose keys are container BomEntities
        /// and values are lists of one or more directly contained
        /// entities with a BomRef attribute, e.g. the Bom itself and
        /// the Components in it; or the Metadata and the Component
        /// description in it; or certain Components or Tools with a
        /// set of further "structural" components.
        ///
        /// The assumption per CycloneDX spec, not directly challenged
        /// in this method, is that each such listed "contained entity"
        /// (likely Component instances) has an unique BomRef value across
        /// the whole single Bom document. Other Bom documents may however
        /// have the same BomRef value (trivially "1", "2", ...) which
        /// is attached to description of an unrelated entity. This can
        /// impact such operations as a FlatMerge() of different Boms.
        ///
        /// See also: GetBomRefsWithContainer() with transposed returns.
        /// </summary>
        /// <returns></returns>
        public Dictionary<BomEntity, List<BomEntity>> GetBomRefsInContainers()
        {
            return dictRefsInContainers;
        }

        /// <summary>
        /// Provide a Dictionary whose keys are "contained" entities
        /// with a BomRef attribute and values are their direct
        /// container BomEntities, e.g. each Bom.Components[] list
        /// entry referring the Bom itself; or the Metadata.Component
        /// entry referring the Metadata; or further "structural"
        /// components in certain Component or Tool entities.
        ///
        /// The assumption per CycloneDX spec, not directly challenged
        /// in this method, is that each such listed "contained entity"
        /// (likely Component instances) has an unique BomRef value across
        /// the whole single Bom document. Other Bom documents may however
        /// have the same BomRef value (trivially "1", "2", ...) which
        /// is attached to description of an unrelated entity. This can
        /// impact such operations as a FlatMerge() of different Boms.
        ///
        /// See also: GetBomRefsInContainers() with transposed returns.
        /// </summary>
        /// <returns></returns>
        public Dictionary<BomEntity, BomEntity> GetBomRefsWithContainer()
        {
            Dictionary<BomEntity, BomEntity> dictWithC = new Dictionary<BomEntity, BomEntity>();

            foreach (var (container, listItems) in dictRefsInContainers)
            {
                if (listItems is null || container is null || listItems.Count < 1) {
                    continue;
                }

                foreach (var item in listItems) {
                    dictWithC[item] = container;
                }
            }

            return dictWithC;
        }
    }
}
