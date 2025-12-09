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
using System.Linq;
using System.Reflection;
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;
using CycloneDX.Utils.Exceptions;
using Json.Schema;

namespace CycloneDX.Utils
{
    class ListMergeHelper<T> where T : IEquatable<T>
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            if (list1 is null) return list2;
            if (list2 is null) return list1;

            var result = new List<T>(list1);
            // We want to avoid the costly computation of the hashes if possible.
            // Therefore, we use a nullable type.
            var resultHashes = new List<int?>(list1.Count);
            for (int i = 0; i < list1.Count; i++)
            {
                resultHashes.Add(null);
            }

            foreach (var item in list2)
            {
                int hash = item.GetHashCode();
                bool found = false;
                for (int i = 0; i < result.Count; i++)
                {
                    var resultItem = result[i];
                    if (resultHashes[i] == null)
                    {
                        resultHashes[i] = resultItem.GetHashCode();
                    }
                    int resultHash = resultHashes[i].Value;
                    if (hash == resultHash && item.Equals(resultItem))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    result.Add(item);
                    resultHashes.Add(hash);
                }
            }

            return result;
        }
    }

    public static class ListExtensions
    {
        public static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> items)
        {
            if (items != null)
            {
                list.AddRange(items);
            }
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
            var toolsComponentsMerger = new ListMergeHelper<Component>();
            var toolsComponents = toolsComponentsMerger.Merge(bom1.Metadata?.Tools?.Components, bom2.Metadata?.Tools?.Components);
            var toolsServicesMerger = new ListMergeHelper<Service>();
            var toolsServices = toolsServicesMerger.Merge(bom1.Metadata?.Tools?.Services, bom2.Metadata?.Tools?.Services);
            if (tools != null || toolsComponents != null || toolsServices != null)
            {
                result.Metadata = new Metadata
                {
                    Tools = new ToolChoices
                    {
                        Tools = tools,
                        Components = toolsComponents,
                        Services = toolsServices,
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

            if (bom1.Definitions != null && bom2.Definitions != null)
            {
                //this will not take a signature, but it probably makes sense to empty those after a merge anyways. 
                result.Definitions = new Definitions();
                var standardMerger = new ListMergeHelper<Standard>();
                result.Definitions.Standards = standardMerger.Merge(bom1.Definitions.Standards, bom2.Definitions.Standards);
            }

            if (bom1.Declarations != null && bom2.Declarations != null)
            {
                //dont merge higher level signatures or the affirmation. The previously signed/affirmed data likely is changed.
                result.Declarations = new Declarations();
                var AssesorMerger = new ListMergeHelper<Assessor>();
                result.Declarations.Assessors = AssesorMerger.Merge(bom1.Declarations.Assessors, bom2.Declarations.Assessors);
                var attestationMerger = new ListMergeHelper<Attestation>();
                result.Declarations.Attestations = attestationMerger.Merge(bom1.Declarations.Attestations, bom2.Declarations.Attestations);
                var claimmerger = new ListMergeHelper<Claim>();
                result.Declarations.Claims = claimmerger.Merge(bom1.Declarations.Claims, bom2.Declarations.Claims);

                if (bom1.Declarations?.Targets != null && bom2.Declarations?.Targets != null)
                {
                    result.Declarations.Targets.Organizations = new ListMergeHelper<OrganizationalEntity>().Merge(bom1.Declarations.Targets.Organizations, bom2.Declarations.Targets.Organizations);
                    result.Declarations.Targets.Components = new ListMergeHelper<Component>().Merge(bom1.Declarations.Targets.Components, bom2.Declarations.Targets.Components);
                    result.Declarations.Targets.Services = new ListMergeHelper<Service>().Merge(bom1.Declarations.Targets.Services, bom2.Declarations.Targets.Services);
                }
            }

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
                        var dep = new Dependency();
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

            result.Declarations = new Declarations
            {
                Assessors = new List<Assessor>(),
                Attestations = new List<Attestation>(),
                Claims = new List<Claim>(),
                Evidence = new List<DeclarationsEvidence>(),
                Targets = new Targets
                {
                    Components = new List<Component>(),
                    Organizations = new List<OrganizationalEntity>(),
                    Services = new List<Service>()
                }
            };

            result.Definitions = new Definitions
            {
                Standards = new List<Standard>()
            };

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
                if (bom.Metadata?.Tools?.Components?.Count > 0)
                {
                    if (result.Metadata.Tools.Components == null)
                    {
                        result.Metadata.Tools.Components = new List<Component>();
                    }
                    foreach (var component in bom.Metadata.Tools.Components)
                    {
                        NamespaceComponentBomRefs(ComponentBomRefNamespace(bom.Metadata.Component), component);
                        if (!result.Metadata.Tools.Components.Contains(component))
                        {
                            result.Metadata.Tools.Components.Add(component);
                        }
                    }
                }
                if (bom.Metadata?.Tools?.Services?.Count > 0)
                {
                    if (result.Metadata.Tools.Services == null)
                    {
                        result.Metadata.Tools.Services = new List<Service>();
                    }
                    foreach (var service in bom.Metadata.Tools.Services)
                    {
                        service.BomRef = NamespacedBomRef(bom.Metadata.Component, service.BomRef);
                        if (!result.Metadata.Tools.Services.Contains(service))
                        {
                            result.Metadata.Tools.Services.Add(service);
                        }
                    }
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
                    NamespaceVulnerabilitiesRefs(ComponentBomRefNamespace(bom.Metadata.Component), bom.Vulnerabilities);
                    result.Vulnerabilities.AddRange(bom.Vulnerabilities);
                }

                void NamespaceBomRefs(IEnumerable<IHasBomRef> refs) => CycloneDXUtils.NamespaceBomRefs(thisComponent, refs);
                void NamespaceReference(IEnumerable<object> refs, string name) => CycloneDXUtils.NamespaceProperty(thisComponent, refs, name);
                
                //Definitions
                if (bom.Definitions?.Standards != null)
                {
                    //Namespace all references
                    NamespaceBomRefs(bom.Definitions.Standards);
                    foreach (var standard in bom.Definitions.Standards)
                    {

                        NamespaceBomRefs(standard.Requirements);
                        NamespaceBomRefs(standard.Levels);
                        NamespaceReference(standard.Levels, nameof(Level.Requirements));
                    }
                    result.Definitions.Standards.AddRange(bom.Definitions.Standards);
                }

                //Assesors
                NamespaceBomRefs(bom.Declarations?.Assessors);
                result.Declarations.Assessors.AddRangeIfNotNull(bom.Declarations?.Assessors);

                //Attestation
                NamespaceReference(bom.Declarations?.Attestations, nameof(Attestation.Assessor));
                bom.Declarations?.Attestations?.ForEach(attestation =>
                {
                    NamespaceReference(attestation.Map, nameof(Map.Claims));
                    NamespaceReference(attestation.Map, nameof(Map.CounterClaims));
                    NamespaceReference(attestation.Map, nameof(Map.Requirement));                    
                    result.Declarations.Attestations.AddRangeIfNotNull(bom.Declarations?.Attestations);
                    NamespaceReference(attestation.Map?.Select(map => map.Conformance), nameof(Conformance.MitigationStrategies));
                });

                //Claims
                NamespaceBomRefs(bom.Declarations?.Claims);
                NamespaceReference(bom.Declarations?.Claims, nameof(Claim.Evidence));
                NamespaceReference(bom.Declarations?.Claims, nameof(Claim.CounterEvidence));
                NamespaceReference(bom.Declarations?.Claims, nameof(Claim.Target));
                result.Declarations.Claims.AddRangeIfNotNull(bom.Declarations?.Claims);

                //Evidence
                NamespaceBomRefs(bom.Declarations?.Evidence);
                result.Declarations.Evidence.AddRangeIfNotNull(bom.Declarations?.Evidence);

                //Targets
                NamespaceBomRefs(result.Declarations?.Targets?.Organizations);
                NamespaceBomRefs(result.Declarations?.Targets?.Components);
                NamespaceBomRefs(result.Declarations?.Targets?.Services);
                result.Declarations.Targets.Organizations.AddRangeIfNotNull(bom.Declarations?.Targets?.Organizations);
                result.Declarations.Targets.Components.AddRangeIfNotNull(bom.Declarations?.Targets?.Components);
                result.Declarations.Targets.Services.AddRangeIfNotNull(bom.Declarations?.Targets?.Services);

            }

            if (bomSubject != null)
            {
                result.Dependencies.Add(new Dependency
                {
                    Ref = result.Metadata.Component.BomRef,
                    Dependencies = bomSubjectDependencies
                });
            }

            // cleanup empty top level elements
            if (result.Metadata.Tools.Tools.Count == 0) { result.Metadata.Tools.Tools = null; }
            if (result.Components.Count == 0) { result.Components = null; }
            if (result.Services.Count == 0) { result.Services = null; }
            if (result.ExternalReferences.Count == 0) { result.ExternalReferences = null; }
            if (result.Dependencies.Count == 0) { result.Dependencies = null; }
            if (result.Compositions.Count == 0) { result.Compositions = null; }
            if (result.Vulnerabilities.Count == 0) { result.Vulnerabilities = null; }

            return result;
        }

        private static void NamespaceBomRefs(Component bomSubject, IEnumerable<IHasBomRef> references)
        {
            if (references == null)
            {
                return;
            }
            foreach (IHasBomRef item in references)
            {
                item.BomRef = NamespacedBomRef(bomSubject, item.BomRef);
            }
        }

        /// <summary>
        /// Applies a namespace transformation to a specified property on a collection of objects.
        /// This method can handle properties of type <see cref="string"/> or <see cref="List{T}"/> where T is <see cref="string"/>.
        /// </summary>
        /// <param name="bomSubject">The component used in the namespace transformation.</param>
        /// <param name="references">The collection of objects whose property values will be transformed.</param>
        /// <param name="property">
        /// The name of the property to be transformed. 
        /// The property can be of type <see cref="string"/> or <see cref="List{T}"/> where T is <see cref="string"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="property"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified <paramref name="property"/> is not found on the objects in <paramref name="references"/>,
        /// or when the property's type is neither <see cref="string"/> nor <see cref="List{T}"/> where T is <see cref="string"/>.
        /// </exception>
        /// <remarks>
        /// The method iterates over each object in the <paramref name="references"/> collection. If the specified property is of type 
        /// <see cref="string"/>, the method applies the <see cref="NamespacedBomRef"/> function to the property value and updates it.
        /// If the property is of type <see cref="List{T}"/> where T is <see cref="string"/>, the method applies the <see cref="NamespacedBomRef"/> 
        /// function to each item in the list, replaces the list with a new one containing the transformed values, and updates the property.
        /// </remarks>
        private static void NamespaceProperty(Component bomSubject, IEnumerable<object> references, string property)
        {
            if (references == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException(nameof(property), "Property name cannot be null or empty.");
            }

            PropertyInfo propertyInfo = null;

            foreach (var item in references)
            {
                if (propertyInfo == null)
                {
                    var type = item.GetType();
                    propertyInfo = type.GetProperty(property);

                    if (propertyInfo == null)
                    {
                        throw new ArgumentException($"Property '{property}' not found on type '{type.FullName}'");
                    }
                }

                // Check if the property is a string
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var currentValue = (string)propertyInfo.GetValue(item);
                    var newValue = NamespacedBomRef(bomSubject, currentValue);
                    propertyInfo.SetValue(item, newValue);
                }
                // Check if the property is a List<string>
                else if (propertyInfo.PropertyType == typeof(List<string>))
                {
                    var currentList = (List<string>)propertyInfo.GetValue(item);

                    if (currentList == null)
                    {
                        currentList = new List<string>();
                    }

                    var updatedList = new List<string>();
                    foreach (var value in currentList)
                    {
                        updatedList.Add(NamespacedBomRef(bomSubject, value));
                    }

                    propertyInfo.SetValue(item, updatedList);
                }
                else
                {
                    throw new ArgumentException($"Property '{property}' on type '{propertyInfo.DeclaringType.FullName}' is neither of type string nor List<string>.");
                }
            }
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
            NamespaceComponentBomRefs(ComponentBomRefNamespace(topComponent), topComponent);
        }

        private static void NamespaceComponentBomRefs(string bomRefNamespace, Component topComponent)
        {
            var components = new Stack<Component>();
            components.Push(topComponent);

            while (components.Count > 0)
            {
                var currentComponent = components.Pop();

                if (currentComponent.Components != null)
                {
                    foreach (var subComponent in currentComponent.Components)
                    {
                        components.Push(subComponent);
                    }
                }

                currentComponent.BomRef = NamespacedBomRef(bomRefNamespace, currentComponent.BomRef);
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
                        affect.Ref = NamespacedBomRef(bomRefNamespace, affect.Ref);
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
                {
                    for (var i = 0; i < composition.Assemblies.Count; i++)
                    {
                        composition.Assemblies[i] = NamespacedBomRef(bomRefNamespace, composition.Assemblies[i]);
                    }
                }

                if (composition.Dependencies != null)
                {
                    for (var i = 0; i < composition.Dependencies.Count; i++)
                    {
                        composition.Dependencies[i] = NamespacedBomRef(bomRefNamespace, composition.Dependencies[i]);
                    }
                }
            }
        }
    }
}
