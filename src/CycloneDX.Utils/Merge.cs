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
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;
using CycloneDX.Utils.Exceptions;

namespace CycloneDX.Utils
{
    class ListMergeHelper<T>
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
                iDebugLevel = 0;

            if (list1 is null || list1.Count < 1) return list2;
            if (list2 is null || list2.Count < 1) return list1;

            if (typeof(BomEntity).IsInstanceOfType(list1[0]))
            {
                return BomUtils.MergeBomEntityLists(list1 as List<BomEntity>, list2 as List<BomEntity>) as List<T>;
            }

            // Lists of legacy types
            if (iDebugLevel >= 1)
                Console.WriteLine($"List-Merge for legacy types: {list1.GetType().ToString()} and {list2.GetType().ToString()}");
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

    public static partial class CycloneDXUtils
    {
        // TOTHINK: Now that we have a BomEntity base class, shouldn't
        // this logic relocate to become a Bom.MergeWith() implementation?
        // Notably, sanity checks like CleanupMetadataComponent and making
        // sure that a Bom+Bom merge produces a spec-validatable result
        // should be a concern of that class (same as we coerce other
        // classes to perform a structure-dependent meaningful merge,
        // and same as the types in its source code handle non-nullable
        // properties, etc.) - right?.. Perhaps sub-classes like BomFlat
        // and BomHierarchical and their respective MergeWith() methods
        // could be a way forward for this...

        /// <summary>
        /// Performs a flat merge of two BOMs.
        /// 
        /// Useful for situations like building a consolidated BOM for a web
        /// application. Flat merge can combine the BOM for frontend code
        /// with the BOM for backend code and return a single, combined BOM.
        /// 
        /// For situations where system component hierarchy is required to be
        /// maintained refer to the <c>HierarchicalMerge</c> method.
        /// </summary>
        /// <param name="bom1"></param>
        /// <param name="bom2"></param>
        /// <returns></returns>
        public static Bom FlatMerge(Bom bom1, Bom bom2)
        {
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
                iDebugLevel = 0;

            var result = new Bom();
            result.Metadata = new Metadata
            {
                // Note: we recurse into this method from other FlatMerge() implementations
                // (e.g. mass-merge of a big list of Bom documents), so the resulting
                // document gets a new timestamp every time. It is unique after all.
                // Also note that a merge of "new Bom()" with a real Bom is also different
                // from that original (serialNumber, timestamp, possible entry order, etc.)
                Timestamp = DateTime.Now
            };

            var toolsMerger = new ListMergeHelper<Tool>();
            var tools = toolsMerger.Merge(bom1.Metadata?.Tools, bom2.Metadata?.Tools);
            if (tools != null)
            {
                result.Metadata.Tools = tools;
            }

            var componentsMerger = new ListMergeHelper<Component>();
            result.Components = componentsMerger.Merge(bom1.Components, bom2.Components);

            // Add main component from bom2 as a "yet another component"
            // if missing in that list so far. Note: any more complicated
            // cases should be handled by CleanupMetadataComponent() when
            // called by MergeCommand or similar consumer; however we can
            // not generally rely in a library that only one particular
            // tool calls it - so this method should ensure validity of
            // its own output on every step along the way.
            if (result.Components != null && !(bom2.Metadata?.Component is null) && !result.Components.Contains(bom2.Metadata.Component))
            {
                // Skip such addition if the component in bom2 is same as the
                // existing metadata/component in bom1 (gluing same file together
                // twice should be effectively no-op); try to merge instead:

                if (iDebugLevel >= 1)
                    Console.WriteLine($"FLAT-MERGE: bom1comp='{bom1.Metadata?.Component}' bom-ref1='{bom1.Metadata?.Component?.BomRef}' bom2comp='{bom2.Metadata?.Component}' bom-ref2='{bom2.Metadata?.Component?.BomRef}'");

                if (!(bom1.Metadata?.Component is null) && (bom2.Metadata.Component.Equals(bom1.Metadata.Component)
                || (!(bom1.Metadata?.Component?.BomRef is null) && !(bom2.Metadata?.Component?.BomRef is null) && (bom1.Metadata.Component.BomRef == bom2.Metadata.Component.BomRef))))
                {
                    // bom1's entry is not null and seems equivalent to bom2's:
                if (iDebugLevel >= 1)
                        Console.WriteLine($"FLAT-MERGE: bom1.Metadata.Component is already equivalent to bom2.Metadata.Component: merging");
                    result.Metadata.Component = bom1.Metadata.Component;
                    result.Metadata.Component.MergeWith(bom2.Metadata.Component);
                }
                else
                {
                if (iDebugLevel >= 1)
                        Console.WriteLine($"FLAT-MERGE: bom1.Metadata.Component is missing or not equivalent to bom2.Metadata.Component: adding new entry into components[]");
                    result.Components.Add(bom2.Metadata.Component);
                }
            }

            var servicesMerger = new ListMergeHelper<Service>();
            result.Services = servicesMerger.Merge(bom1.Services, bom2.Services);

            var extRefsMerger = new ListMergeHelper<ExternalReference>();
            result.ExternalReferences = extRefsMerger.Merge(bom1.ExternalReferences, bom2.ExternalReferences);

            var dependenciesMerger = new ListMergeHelper<Dependency>();
            result.Dependencies = dependenciesMerger.Merge(bom1.Dependencies, bom2.Dependencies);

            var compositionsMerger = new ListMergeHelper<Composition>();
            result.Compositions = compositionsMerger.Merge(bom1.Compositions, bom2.Compositions);

            var vulnerabilitiesMerger = new ListMergeHelper<Vulnerability>();
            result.Vulnerabilities = vulnerabilitiesMerger.Merge(bom1.Vulnerabilities, bom2.Vulnerabilities);

            result = CleanupMetadataComponent(result);
            result = CleanupEmptyLists(result);

            return result;
        }


        /// <summary>
        /// Performs a flat merge of multiple BOMs.
        /// 
        /// Useful for situations like building a consolidated BOM for a web
        /// application. Flat merge can combine the BOM for frontend code
        /// with the BOM for backend code and return a single, combined BOM.
        /// 
        /// For situations where system component hierarchy is required to be
        /// maintained refer to the <c>HierarchicalMerge</c> method.
        /// </summary>
        /// <param name="bom1"></param>
        /// <param name="bom2"></param>
        /// <returns></returns>
        public static Bom FlatMerge(IEnumerable<Bom> boms)
        {
            return FlatMerge(boms, null);
        }

        /// <summary>
        /// Performs a flat merge of multiple BOMs.
        /// 
        /// Useful for situations like building a consolidated BOM for a web
        /// application. Flat merge can combine the BOM for frontend code
        /// with the BOM for backend code and return a single, combined BOM.
        /// 
        /// For situations where system component hierarchy is required to be
        /// maintained refer to the <c>HierarchicalMerge</c> method.
        /// </summary>
        /// <param name="bom1"></param>
        /// <param name="bom2"></param>
        /// <returns></returns>
        public static Bom FlatMerge(IEnumerable<Bom> boms, Component bomSubject)
        {
            var result = new Bom();

            // Note: we were asked to "merge" and so we do, per principle of
            // least surprise - even if there is just one entry in boms[] so
            // we might be inclined to skip the loop. Resulting document WILL
            // differ from such single original (serialNumber, timestamp...)
            foreach (var bom in boms)
            {
                result = FlatMerge(result, bom);
            }

            if (bomSubject != null)
            {
                // use the params provided if possible: prepare a new document
                // with desired "metadata/component" and merge differing data
                // from earlier collected result into this structure.
                var resultSubj = new Bom();

                resultSubj.Metadata.Component = bomSubject;
                resultSubj.Metadata.Component.BomRef = ComponentBomRefNamespace(result.Metadata.Component);
                result = FlatMerge(resultSubj, result);

                var mainDependency = new Dependency();
                mainDependency.Ref = result.Metadata.Component.BomRef;
                mainDependency.Dependencies = new List<Dependency>();
                
                foreach (var bom in boms)
                {
                    if (!(bom.Metadata?.Component is null)) 
                    {
                        var dep =  new Dependency();
                        dep.Ref = bom.Metadata.Component.BomRef;

                        mainDependency.Dependencies.Add(dep);
                    }
                }

                result.Dependencies.Add(mainDependency);
            }

            result = CleanupMetadataComponent(result);
            result = CleanupEmptyLists(result);

            return result;
        }

        /// <summary>
        /// Performs a hierarchical merge for multiple BOMs.
        /// 
        /// To retain system component hierarchy, top level BOM metadata
        /// component must be included in each BOM.
        /// </summary>
        /// <param name="boms"></param>
        /// <param name="bomSubject">
        /// The component described by the hierarchical merge being performed.
        /// 
        /// This will be included as the top level BOM metadata component in
        /// the returned BOM.
        /// </param>
        /// <returns></returns>
        public static Bom HierarchicalMerge(IEnumerable<Bom> boms, Component bomSubject)
        {
            var result = new Bom();
            result.Metadata = new Metadata
            {
                Timestamp = DateTime.Now
            };

            if (bomSubject != null)
            {
                if (bomSubject.BomRef is null) bomSubject.BomRef = ComponentBomRefNamespace(bomSubject);
                result.Metadata.Component = bomSubject;
                result.Metadata.Tools = new List<Tool>();
            }

            result.Components = new List<Component>();
            result.Services = new List<Service>();
            result.ExternalReferences = new List<ExternalReference>();
            result.Dependencies = new List<Dependency>();
            result.Compositions = new List<Composition>();
            result.Vulnerabilities = new List<Vulnerability>();

            var bomSubjectDependencies = new List<Dependency>();

            foreach (var bom in boms)
            {
                if (bom.Metadata?.Component is null)
                {
                    throw new MissingMetadataComponentException(
                        bom.SerialNumber is null
                        ? "Required metadata (top level) component is missing from BOM."
                        : $"Required metadata (top level) component is missing from BOM {bom.SerialNumber}.");
                }

                if (bom.Metadata?.Tools?.Count > 0)
                {
                    if (result.Metadata.Tools == null)
                    {
                        result.Metadata.Tools = new List<Tool>();
                    }

                    result.Metadata.Tools.AddRange(bom.Metadata.Tools);
                }

                var thisComponent = bom.Metadata.Component;
                if (thisComponent.Components is null) bom.Metadata.Component.Components = new List<Component>();
                if (!(bom.Components is null))
                {
                    thisComponent.Components.AddRange(bom.Components);
                }

                // add a namespace to existing BOM refs
                NamespaceComponentBomRefs(thisComponent);

                // make sure we have a BOM ref set and add top level dependency reference
                if (thisComponent.BomRef is null) thisComponent.BomRef = ComponentBomRefNamespace(thisComponent);
                bomSubjectDependencies.Add(new Dependency { Ref = thisComponent.BomRef });

                result.Components.Add(thisComponent);


                // services
                if (bom.Services != null)
                foreach (var service in bom.Services)
                {
                    service.BomRef = NamespacedBomRef(bom.Metadata.Component, service.BomRef);
                    result.Services.Add(service);
                }

                // external references
                if (!(bom.ExternalReferences is null)) result.ExternalReferences.AddRange(bom.ExternalReferences);

                // dependencies
                if (bom.Dependencies != null)
                {
                    NamespaceDependencyBomRefs(ComponentBomRefNamespace(thisComponent), bom.Dependencies);
                    result.Dependencies.AddRange(bom.Dependencies);
                }

                // compositions
                if (bom.Compositions != null)
                {
                    NamespaceCompositions(ComponentBomRefNamespace(bom.Metadata.Component), bom.Compositions);
                    result.Compositions.AddRange(bom.Compositions);
                }

                // vulnerabilities
                if (bom.Vulnerabilities != null)
                {
                    NamespaceVulnerabilitiesRefs(ComponentBomRefNamespace(result.Metadata.Component), bom.Vulnerabilities);
                    result.Vulnerabilities.AddRange(bom.Vulnerabilities);
                }
            }

            if (bomSubject != null)
            {
                result.Dependencies.Add( new Dependency
                {
                    Ref = result.Metadata.Component.BomRef,
                    Dependencies = bomSubjectDependencies
                });
            }

            result = CleanupMetadataComponent(result);
            result = CleanupEmptyLists(result);

            return result;
        }

        /// <summary>
        /// Merge main "metadata/component" entry with its possible alter-ego
        /// in the components list and evict extra copy from that list: per
        /// spec v1_4 at least, the bom-ref must be unique across the document.
        /// </summary>
        /// <param name="result">A Bom document</param>
        /// <returns>Resulting document (whether modified or not)</returns>
        public static Bom CleanupMetadataComponent(Bom result)
        {
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
                iDebugLevel = 0;

            if (iDebugLevel >= 1)
                Console.WriteLine($"MERGE-CLEANUP: metadata/component/bom-ref='{result.Metadata?.Component?.BomRef}'");
            if (!(result.Metadata.Component is null) && !(result.Components is null) && (result.Components?.Count > 0) && result.Components.Contains(result.Metadata.Component))
            {
                if (iDebugLevel >= 2)
                    Console.WriteLine($"MERGE-CLEANUP: Searching in list");
                foreach (Component component in result.Components)
                {
                    if (iDebugLevel >= 2)
                        Console.WriteLine($"MERGE-CLEANUP: Looking at a bom-ref='{component?.BomRef}'");
                    if (component is null) continue; // should not happen
                    if (component.Equals(result.Components) || component.BomRef.Equals(result.Metadata.Component.BomRef))
                    {
                        if (iDebugLevel >= 1)
                            Console.WriteLine($"MERGE-CLEANUP: Found in list: merging, cleaning...");
                        result.Metadata.Component.MergeWith(component);
                        result.Components.Remove(component);
                        return result;
                    }
                }
            }

            if (iDebugLevel >= 1)
                Console.WriteLine($"MERGE-CLEANUP: NO HITS");
            return result;
        }

        public static Bom CleanupEmptyLists(Bom result)
        {
            // cleanup empty top level elements
            if (result.Metadata?.Tools?.Count == 0) result.Metadata.Tools = null;
            if (result.Components?.Count == 0) result.Components = null;
            if (result.Services?.Count == 0) result.Services = null;
            if (result.ExternalReferences?.Count == 0) result.ExternalReferences = null;
            if (result.Dependencies?.Count == 0) result.Dependencies = null;
            if (result.Compositions?.Count == 0) result.Compositions = null;
            if (result.Vulnerabilities?.Count == 0) result.Vulnerabilities = null;

            return result;
        }

        private static string NamespacedBomRef(Component bomSubject, string bomRef)
        {
            return string.IsNullOrEmpty(bomRef) ? null : NamespacedBomRef(ComponentBomRefNamespace(bomSubject), bomRef);
        }

        private static string NamespacedBomRef(string bomRefNamespace, string bomRef)
        {
            return string.IsNullOrEmpty(bomRef) ? null : $"{bomRefNamespace}:{bomRef}";
        }

        private static string ComponentBomRefNamespace(Component component)
        {
            return component.Group is null
                ? $"{component.Name}@{component.Version}"
                : $"{component.Group}.{component.Name}@{component.Version}";
        }

        private static void NamespaceComponentBomRefs(Component topComponent)
        {
            var components = new Stack<Component>();
            components.Push(topComponent);

            while (components.Count > 0)
            {
                var currentComponent = components.Pop();

                if (currentComponent.Components != null)
                foreach (var subComponent in currentComponent.Components)
                {
                    components.Push(subComponent);
                }

                currentComponent.BomRef = NamespacedBomRef(topComponent, currentComponent.BomRef);
            }
        }

        private static void NamespaceVulnerabilitiesRefs(string bomRefNamespace, List<Vulnerability> vulnerabilities)
        {
            var pendingVulnerabilities = new Stack<Vulnerability>(vulnerabilities);

            while (pendingVulnerabilities.Count > 0)
            {
                var vulnerability = pendingVulnerabilities.Pop();

                vulnerability.BomRef = NamespacedBomRef(bomRefNamespace, vulnerability.BomRef);

                if (vulnerability.Affects != null)
                {
                    foreach (var affect in vulnerability.Affects)
                    {
                        affect.Ref = bomRefNamespace;
                    }
                }
            }
        }

        private static void NamespaceDependencyBomRefs(string bomRefNamespace, List<Dependency> dependencies)
        {
            var pendingDependencies = new Stack<Dependency>(dependencies);

            while (pendingDependencies.Count > 0)
            {
                var dependency = pendingDependencies.Pop();

                if (dependency.Dependencies != null)
                foreach (var subDependency in dependency.Dependencies)
                {
                    pendingDependencies.Push(subDependency);
                }

                dependency.Ref = NamespacedBomRef(bomRefNamespace, dependency.Ref);
            }
        }

        private static void NamespaceCompositions(string bomRefNamespace, List<Composition> compositions)
        {
            foreach (var composition in compositions)
            {
                if (composition.Assemblies != null)
                    for (var i=0; i<composition.Assemblies.Count; i++)
                    {
                        composition.Assemblies[i] = NamespacedBomRef(bomRefNamespace, composition.Assemblies[i]);
                    }

                if (composition.Dependencies != null)
                    for (var i=0; i<composition.Dependencies.Count; i++)
                    {
                        composition.Dependencies[i] = NamespacedBomRef(bomRefNamespace, composition.Dependencies[i]);
                    }
            }
        }
    }
}
