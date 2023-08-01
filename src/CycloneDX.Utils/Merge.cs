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
            if (list1 is null) return list2;
            if (list2 is null) return list1;

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
            var result = new Bom();

            #pragma warning disable 618
            var toolsMerger = new ListMergeHelper<Tool>();
            #pragma warning restore 618
            var tools = toolsMerger.Merge(bom1.Metadata?.Tools?.Tools, bom2.Metadata?.Tools?.Tools);
            if (tools != null)
            {
                result.Metadata = new Metadata
                {
                    Tools = new ToolChoices
                    {
                        Tools = tools,
                    }
                };
            }

            var componentsMerger = new ListMergeHelper<Component>();
            result.Components = componentsMerger.Merge(bom1.Components, bom2.Components);

            //Add main component if missing
            if (result.Components != null && !(bom2.Metadata?.Component is null) && !result.Components.Contains(bom2.Metadata.Component)) 
            {
                result.Components.Add(bom2.Metadata.Component);
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
            
            foreach (var bom in boms)
            {
                result = FlatMerge(result, bom);
            }

            if (bomSubject != null)
            {
                // use the params provided if possible
                result.Metadata.Component = bomSubject;
                result.Metadata.Component.BomRef = ComponentBomRefNamespace(result.Metadata.Component);

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
            if (bomSubject != null)
            {
                if (bomSubject.BomRef is null) bomSubject.BomRef = ComponentBomRefNamespace(bomSubject);
                result.Metadata = new Metadata
                {
                    Component = bomSubject,
                    #pragma warning disable 618
                    Tools = new ToolChoices
                    {
                        Tools = new List<Tool>(),
                    }
                    #pragma warning restore 618
                };
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

                if (bom.Metadata?.Tools?.Tools?.Count > 0)
                {
                    result.Metadata.Tools.Tools.AddRange(bom.Metadata.Tools.Tools);
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

            // cleanup empty top level elements
            if (result.Metadata.Tools.Tools.Count == 0) result.Metadata.Tools.Tools = null;
            if (result.Components.Count == 0) result.Components = null;
            if (result.Services.Count == 0) result.Services = null;
            if (result.ExternalReferences.Count == 0) result.ExternalReferences = null;
            if (result.Dependencies.Count == 0) result.Dependencies = null;
            if (result.Compositions.Count == 0) result.Compositions = null;
            if (result.Vulnerabilities.Count == 0) result.Vulnerabilities = null;

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
