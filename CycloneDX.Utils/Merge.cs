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
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public class MissingMetadataComponentException : Exception
    {
        public MissingMetadataComponentException(string message) : base(message) {}
    }

    class ListMergeHelper<T>
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            if (list1 is null) return list2;
            if (list2 is null) return list1;

            var result = new List<T>(list1);
            result.AddRange(list2);

            return result;
        }
    }

    public static partial class CycloneDXUtils
    {
        [Obsolete("Merge method is deprecated, please use FlatMerge method instead.")]
        public static Bom Merge(Bom bom1, Bom bom2)
        {
            return FlatMerge(bom1, bom2);
        }

        public static Bom FlatMerge(Bom bom1, Bom bom2)
        {
            var result = new Bom();

            var toolsMerger = new ListMergeHelper<Tool>();
            var tools = toolsMerger.Merge(bom1.Metadata?.Tools, bom2.Metadata?.Tools);
            if (tools != null)
            {
                result.Metadata = new Metadata
                {
                    Tools = tools
                };
            }

            var componentsMerger = new ListMergeHelper<Component>();
            result.Components = componentsMerger.Merge(bom1.Components, bom2.Components);

            var servicesMerger = new ListMergeHelper<Service>();
            result.Services = servicesMerger.Merge(bom1.Services, bom2.Services);

            var extRefsMerger = new ListMergeHelper<ExternalReference>();
            result.ExternalReferences = extRefsMerger.Merge(bom1.ExternalReferences, bom2.ExternalReferences);

            var dependenciesMerger = new ListMergeHelper<Dependency>();
            result.Dependencies = dependenciesMerger.Merge(bom1.Dependencies, bom2.Dependencies);

            var compositionsMerger = new ListMergeHelper<Composition>();
            result.Compositions = compositionsMerger.Merge(bom1.Compositions, bom2.Compositions);

            return result;
        }

        public static Bom FlatMerge(IEnumerable<Bom> boms)
        {
            var result = new Bom();

            foreach (var bom in boms)
            {
                result = FlatMerge(result, bom);
            }

            return result;
        }

        public static Bom HierarchicalMerge(IEnumerable<Bom> boms)
        {
            var result = new Bom();

            result.Components = new List<Component>();
            result.Services = new List<Service>();
            result.ExternalReferences = new List<ExternalReference>();
            result.Dependencies = new List<Dependency>();
            result.Compositions = new List<Composition>();

            foreach (var bom in boms)
            {
                if (bom?.Metadata?.Component is null)
                {
                    throw new MissingMetadataComponentException(
                        bom.SerialNumber is null
                        ? "Required metadata (top level) component is missing from BOM."
                        : $"Required metadata (top level) component is missing from BOM {bom.SerialNumber}.");
                }

                // top level component and sub-components
                if (bom.Metadata.Component.Components is null) bom.Metadata.Component.Components = new List<Component>();
                bom.Metadata.Component.Components.AddRange(bom.Components);
                NamespaceComponentBomRefs(bom.Metadata.Component);
                result.Components.Add(bom.Metadata.Component);

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
                    NamespaceDependencyBomRefs(ComponentBomRefNamespace(bom.Metadata.Component), bom.Dependencies);
                    result.Dependencies.AddRange(bom.Dependencies);
                }

                // compositions
                if (bom.Compositions != null)
                {
                    NamespaceCompositions(ComponentBomRefNamespace(bom.Metadata.Component), bom.Compositions);
                    result.Compositions.AddRange(bom.Compositions);
                }
            }

            // cleanup empty top level elements
            if (result.Components.Count == 0) result.Components = null;
            if (result.Services.Count == 0) result.Services = null;
            if (result.ExternalReferences.Count == 0) result.ExternalReferences = null;
            if (result.Dependencies.Count == 0) result.Dependencies = null;
            if (result.Compositions.Count == 0) result.Compositions = null;

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
            return $"{component.Name}@{component.Version}";
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
