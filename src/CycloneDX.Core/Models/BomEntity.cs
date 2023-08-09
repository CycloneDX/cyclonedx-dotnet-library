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
            : base(String.Format("Unresolvable conflict in Bom entities"))
        { }

        public BomEntityConflictException(Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}", type.ToString()))
        { }

        public BomEntityConflictException(string msg)
            : base(String.Format("Unresolvable conflict in Bom entities: {0}", msg))
        { }

        public BomEntityConflictException(string msg, Type type)
            : base(String.Format("Unresolvable conflict in Bom entities of type {0}: {1}", type.ToString(), msg))
        { }
    }

    [Serializable]
    public class BomEntityIncompatibleException : Exception
    {
        public BomEntityIncompatibleException()
            : base(String.Format("Comparing incompatible Bom entities"))
        { }

        public BomEntityIncompatibleException(Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}", type1.ToString(), type2.ToString()))
        { }

        public BomEntityIncompatibleException(string msg)
            : base(String.Format("Comparing incompatible Bom entities: {0}", msg))
        { }

        public BomEntityIncompatibleException(string msg, Type type1, Type type2)
            : base(String.Format("Comparing incompatible Bom entities of types {0} and {1}: {2}", type1.ToString(), type2.ToString(), msg))
        { }
    }

    public class BomEntityListMergeHelper<T> where T : BomEntity
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            //return BomUtils.MergeBomEntityLists(list1, list2);
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
                iDebugLevel = 0;

            if (list1 is null || list1.Count < 1) return list2;
            if (list2 is null || list2.Count < 1) return list1;

            if (iDebugLevel >= 1)
                Console.WriteLine($"List-Merge for BomEntity derivatives: {list1.GetType().ToString()}");

            List<T> result = new List<T>(list1);
            Type TType = list1[0].GetType();
            if (!BomEntity.KnownTypeMergeWith.TryGetValue(TType, out var methodMergeWith))
            {
                methodMergeWith = null;
            }

            foreach (var item2 in list2)
            {
                bool isContained = false;
                if (iDebugLevel >= 3)
                    Console.WriteLine($"result<{TType.ToString()}> now contains {result.Count} entries");

                for (int i=0; i < result.Count; i++)
                {
                    if (iDebugLevel >= 3)
                        Console.WriteLine($"result<{TType.ToString()}>: checking entry #{i}");
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
                        Console.WriteLine($"ALREADY THERE: {item2.ToString()}");
                }
                else
                {
                    // Add new entry "as is" (new-ness is subject to
                    // equality checks of respective classes):
                    if (iDebugLevel >= 2)
                        Console.WriteLine($"WILL ADD: {item2.ToString()}");
                    result.Add(item2);
                }
            }

            return result;
        }
    }

    public class BomEntityListReflection
    {
        public Type genericType;
        public PropertyInfo propCount;
        public MethodInfo methodAdd;
        public MethodInfo methodAddRange;
        public MethodInfo methodGetItem;
    }

    public class BomEntityListMergeHelperReflection
    {
        public Type genericType;
        public MethodInfo methodMerge;
        public Object helperInstance;
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
        public static List<Type> KnownEntityTypes =
            new Func<List<Type>>(() =>
            {
                List<Type> derived_types = new List<Type>();
                foreach (var domain_assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assembly_types = domain_assembly.GetTypes()
                        .Where(type => type.IsSubclassOf(typeof(BomEntity)) && !type.IsAbstract);

                    derived_types.AddRange(assembly_types);
                }
                return derived_types;
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equals() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static Dictionary<Type, System.Reflection.PropertyInfo[]> KnownEntityTypeProperties =
            new Func<Dictionary<Type, System.Reflection.PropertyInfo[]>>(() =>
            {
                Dictionary<Type, System.Reflection.PropertyInfo[]> dict = new Dictionary<Type, System.Reflection.PropertyInfo[]>();
                foreach (var type in KnownEntityTypes)
                {
                    dict[type] = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance); // BindingFlags.DeclaredOnly
                }
                return dict;
            }) ();

        public static Dictionary<Type, BomEntityListReflection> KnownEntityTypeLists =
            new Func<Dictionary<Type, BomEntityListReflection>>(() =>
            {
                Dictionary<Type, BomEntityListReflection> dict = new Dictionary<Type, BomEntityListReflection>();
                foreach (var type in KnownEntityTypes)
                {
                    // Inspired by https://stackoverflow.com/a/4661237/4715872
                    // to craft a List<SpecificType> "result" at run-time:
                    Type listType = typeof(List<>);
                    Type constructedListType = listType.MakeGenericType(type);
                    // Needed? var helper = Activator.CreateInstance(constructedListType);

                    dict[type] = new BomEntityListReflection();
                    dict[type].genericType = constructedListType;

                    // Gotta use reflection for run-time evaluated type methods:
                    dict[type].propCount = constructedListType.GetProperty("Count");
                    dict[type].methodGetItem = constructedListType.GetMethod("get_Item");
                    dict[type].methodAdd = constructedListType.GetMethod("Add", 0, new Type[] { type });
                    dict[type].methodAddRange = constructedListType.GetMethod("AddRange", 0, new Type[] { constructedListType });
                }
                return dict;
            }) ();

        public static Dictionary<Type, BomEntityListMergeHelperReflection> KnownBomEntityListMergeHelpers =
            new Func<Dictionary<Type, BomEntityListMergeHelperReflection>>(() =>
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
                        var methodMerge = constructedListHelperType.GetMethod("Merge", 0, new Type[] { LType, LType });
                        if (methodMerge != null)
                        {
                            dict[type] = new BomEntityListMergeHelperReflection();
                            dict[type].genericType = constructedListHelperType;
                            dict[type].methodMerge = methodMerge;
                            dict[type].helperInstance = helper;
                            // Callers would return (List<T>)methodMerge.Invoke(helper, new object[] {list1, list2});
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
                return dict;
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about custom CycloneDX.Json.Serializer.Serialize()
        /// implementations (if present), prepared startically at start time.
        /// </summary>
        public static Dictionary<Type, System.Reflection.MethodInfo> KnownTypeSerializers =
            new Func<Dictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                var jserClassType = typeof(CycloneDX.Json.Serializer);
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = jserClassType.GetMethod("Serialize",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
                        new Type[] { type });
                    if (method != null)
                        dict[type] = method;
                }
                return dict;
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equals() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static Dictionary<Type, System.Reflection.MethodInfo> KnownTypeEquals =
            new Func<Dictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equals",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new Type[] { type });
                    if (method != null)
                        dict[type] = method;
                }
                return dict;
            }) ();

        // Our loops check for some non-BomEntity typed value equalities,
        // so cache their methods if present. Note that this one retains
        // the "null" results to mark that we do not need to look further.
        public static Dictionary<Type, System.Reflection.MethodInfo> KnownOtherTypeEquals =
            new Func<Dictionary<Type, System.Reflection.MethodInfo>>(() =>
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
                        new Type[] { type });
                    dict[type] = method;
                }
                return dict;
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom Equivalent() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static Dictionary<Type, System.Reflection.MethodInfo> KnownTypeEquivalent =
            new Func<Dictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("Equivalent",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new Type[] { type });
                    if (method != null)
                        dict[type] = method;
                }
                return dict;
            }) ();

        /// <summary>
        /// Dictionary mapping classes derived from BomEntity to reflection
        /// MethodInfo about their custom MergeWith() method implementations
        /// (if present), prepared startically at start time.
        /// </summary>
        public static Dictionary<Type, System.Reflection.MethodInfo> KnownTypeMergeWith =
            new Func<Dictionary<Type, System.Reflection.MethodInfo>>(() =>
            {
                Dictionary<Type, System.Reflection.MethodInfo> dict = new Dictionary<Type, System.Reflection.MethodInfo>();
                foreach (var type in KnownEntityTypes)
                {
                    var method = type.GetMethod("MergeWith",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        new Type[] { type });
                    if (method != null)
                        dict[type] = method;
                }
                return dict;
            }) ();

        protected BomEntity()
        {
            // a bad alternative to private is to: throw new NotImplementedException("The BomEntity class directly should not be instantiated");
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

            var res = CycloneDX.Json.Serializer.Serialize(this);
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
        /// <param name="other">Another BomEntity-derived object of same type</param>
        /// <returns>True if two objects are deemed equal</returns>
        public bool Equals(BomEntity other)
        {
            Type thisType = this.GetType();
            if (KnownTypeEquals.TryGetValue(thisType, out var methodEquals))
            {
                return (bool)methodEquals.Invoke(this, new object[] {other});
            }

            if (other is null || thisType != other.GetType()) return false;
            return this.SerializeEntity() == other.SerializeEntity();
        }
    
        public override int GetHashCode()
        {
            return this.SerializeEntity().GetHashCode();
        }

        /// <summary>
        /// Do this and other objects describe the same real-life entity?
        /// "Override" this in sub-classes that have a more detailed definition of
        /// equivalence (e.g. that certain fields are equal even if whole contents
        /// are not) by defining an implementation tailored to that derived type
        /// as the argument, or keep this default where equiality is equivalence.
        /// </summary>
        /// <param name="other">Another object of same type</param>
        /// <returns>True if two data objects are considered to represent
        /// the same real-life entity, False otherwise.</returns>
        public bool Equivalent(BomEntity other)
        {
            Type thisType = this.GetType();
            if (KnownTypeEquivalent.TryGetValue(thisType, out var methodEquivalent))
            {
                // Note we do not check for null/type of "other" at this point
                // since the derived classes define the logic of equivalence
                // (possibly to other entity subtypes as well).
                return (bool)methodEquivalent.Invoke(this, new object[] {other});
            }

            // Note that here a default Equivalent() may call into custom Equals(),
            // so the similar null/type sanity shecks are still relevant.
            return (!(other is null) && (thisType == other.GetType()) && this.Equals(other));
        }

        /// <summary>
        /// Default implementation just "agrees" that Equals()==true objects
        /// are already merged (returns true), and that Equivalent()==false
        /// objects are not (returns false), and for others (equivalent but
        /// not equal, or different types) raises an exception.
        /// Treats a null "other" object as a success (it is effectively a
        /// no-op merge, which keeps "this" object as is).
        /// </summary>
        /// <param name="other">Another object of same type whose additional
        /// non-conflicting data we try to squash into this object.</param>
        /// <returns>True if merge was successful, False if it these objects
        /// are not equivalent, or throws if merge can not be done (including
        /// lack of merge logic or unresolvable conflicts in data points).
        /// </returns>
        /// <exception cref="BomEntityConflictException">Source data problem: two entities with conflicting information</exception>
        /// <exception cref="BomEntityIncompatibleException">Caller error: somehow merging different entity types</exception>
        public bool MergeWith(BomEntity other)
        {
            if (other is null) return true;
            if (this.GetType() != other.GetType())
            {
                // Note: potentially descendent classes can catch this
                // to adapt their behavior... if some two different
                // classes would ever describe something comparable
                // in real life.
                throw new BomEntityIncompatibleException(this.GetType(), other.GetType());
            }

            if (this.Equals(other)) return true;
            // Avoid calling Equals => serializer twice for no gain
            // (default equivalence is equality):
            if (KnownTypeEquivalent.TryGetValue(this.GetType(), out var methodEquivalent))
            {
                if (!this.Equivalent(other)) return false;
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
