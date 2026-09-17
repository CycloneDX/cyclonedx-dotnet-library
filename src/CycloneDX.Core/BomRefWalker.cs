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
using CycloneDX.Models.Vulnerabilities;

namespace CycloneDX
{
    /// <summary>
    /// Rewrites every "bom-ref"-shaped value in a <see cref="Bom"/> document
    /// -- both identifiers (Component/Service/Vulnerability/Annotation
    /// BomRef) and back-references to them (Dependency.Ref, Composition's
    /// Assemblies/Dependencies string lists, Vulnerability.Affects[].Ref,
    /// Annotation subjects) -- through a caller-supplied function.
    /// </summary>
    /// <remarks>
    /// This generalizes traversal code CycloneDX.Utils.CycloneDXUtils.
    /// HierarchicalMerge already hand-rolls for bom-ref namespacing (see
    /// its private NamespaceComponentBomRefs/NamespaceDependencyBomRefs/
    /// NamespaceCompositions/NamespaceVulnerabilitiesRefs/
    /// NamespaceAnnotationsBomRefs methods) into one reusable entry point,
    /// parameterized on an arbitrary rewrite function instead of always
    /// prefixing a namespace. Namespacing becomes
    /// <c>RewriteRefs(bom, r => $"{ns}:{r}")</c>; a manual single-ref
    /// rename (as the CLI's <c>rename-entity</c> command needs) becomes
    /// <c>RewriteRefs(bom, r => r == oldRef ? newRef : r)</c>.
    ///
    /// Scope note: covers Metadata.Component, Components, Services,
    /// Dependencies, Compositions, Vulnerabilities, and Annotations -- it
    /// does not yet walk the newer (CycloneDX 1.6) Declarations/Definitions
    /// sections. Extending it there is a mechanical follow-up, not an
    /// architectural one: add another block below following the same
    /// pattern.
    /// </remarks>
    public static class BomRefWalker
    {
        public static void RewriteRefs(Bom bom, Func<string, string> rewrite)
        {
            if (bom is null || rewrite is null)
            {
                return;
            }

            if (bom.Metadata?.Component != null)
            {
                RewriteComponentTree(bom.Metadata.Component, rewrite);
            }
            if (bom.Metadata?.Tools?.Components != null)
            {
                foreach (var component in bom.Metadata.Tools.Components)
                {
                    RewriteComponentTree(component, rewrite);
                }
            }
            if (bom.Metadata?.Tools?.Services != null)
            {
                foreach (var service in bom.Metadata.Tools.Services)
                {
                    service.BomRef = rewrite(service.BomRef);
                }
            }

            if (bom.Components != null)
            {
                foreach (var component in bom.Components)
                {
                    RewriteComponentTree(component, rewrite);
                }
            }

            if (bom.Services != null)
            {
                foreach (var service in bom.Services)
                {
                    service.BomRef = rewrite(service.BomRef);
                }
            }

            if (bom.Dependencies != null)
            {
                RewriteDependencyTree(bom.Dependencies, rewrite);
            }

            if (bom.Compositions != null)
            {
                foreach (var composition in bom.Compositions)
                {
                    RewriteStringListInPlace(composition.Assemblies, rewrite);
                    RewriteStringListInPlace(composition.Dependencies, rewrite);
                }
            }

            if (bom.Vulnerabilities != null)
            {
                foreach (var vulnerability in bom.Vulnerabilities)
                {
                    vulnerability.BomRef = rewrite(vulnerability.BomRef);
                    if (vulnerability.Affects != null)
                    {
                        foreach (var affect in vulnerability.Affects)
                        {
                            affect.Ref = rewrite(affect.Ref);
                        }
                    }
                }
            }

            if (bom.Annotations != null)
            {
                foreach (var annotation in bom.Annotations)
                {
                    annotation.BomRef = rewrite(annotation.BomRef);
                    if (annotation.XmlSubjects != null)
                    {
                        for (var i = 0; i < annotation.XmlSubjects.Count; i++)
                        {
                            annotation.XmlSubjects[i].Ref = rewrite(annotation.XmlSubjects[i].Ref);
                        }
                    }
                    if (annotation.Annotator?.Component != null)
                    {
                        RewriteComponentTree(annotation.Annotator.Component, rewrite);
                    }
                    if (annotation.Annotator?.Individual != null)
                    {
                        annotation.Annotator.Individual.BomRef = rewrite(annotation.Annotator.Individual.BomRef);
                    }
                    if (annotation.Annotator?.Organization != null)
                    {
                        annotation.Annotator.Organization.BomRef = rewrite(annotation.Annotator.Organization.BomRef);
                    }
                    if (annotation.Annotator?.Service != null)
                    {
                        annotation.Annotator.Service.BomRef = rewrite(annotation.Annotator.Service.BomRef);
                    }
                }
            }
        }

        private static void RewriteComponentTree(Component topComponent, Func<string, string> rewrite)
        {
            var pending = new Stack<Component>();
            pending.Push(topComponent);
            while (pending.Count > 0)
            {
                var component = pending.Pop();
                if (component.Components != null)
                {
                    foreach (var sub in component.Components)
                    {
                        pending.Push(sub);
                    }
                }
                component.BomRef = rewrite(component.BomRef);
            }
        }

        private static void RewriteDependencyTree(List<Dependency> dependencies, Func<string, string> rewrite)
        {
            var pending = new Stack<Dependency>(dependencies);
            while (pending.Count > 0)
            {
                var dependency = pending.Pop();
                if (dependency.Dependencies != null)
                {
                    foreach (var sub in dependency.Dependencies)
                    {
                        pending.Push(sub);
                    }
                }
                dependency.Ref = rewrite(dependency.Ref);
            }
        }

        private static void RewriteStringListInPlace(List<string> refs, Func<string, string> rewrite)
        {
            if (refs is null)
            {
                return;
            }
            for (var i = 0; i < refs.Count; i++)
            {
                refs[i] = rewrite(refs[i]);
            }
        }
    }
}
#endif
