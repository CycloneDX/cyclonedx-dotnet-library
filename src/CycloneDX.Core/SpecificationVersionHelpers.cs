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
using System.Text.RegularExpressions;
using CycloneDX.Models;

namespace CycloneDX
{
    public static class SpecificationVersionHelpers
    {
        public static readonly SpecificationVersion CurrentVersion = SpecificationVersion.v1_3;

        public static readonly string XmlNamespaceRegexString = @"http:\/\/cyclonedx\.org\/schema\/bom\/(?<SpecificationVersionString>1\.[0-3])";

        public static readonly Regex XmlNamespaceRegex = new Regex(XmlNamespaceRegexString);

        public static bool IsValidXmlNamespace(string xmlns)
        {
            var match = XmlNamespaceRegex.Match(xmlns);
            return match.Success;
        }

        public static string XmlNamespaceSpecificationVersion(string xmlns)
        {
            var match = XmlNamespaceRegex.Match(xmlns);
            if (match.Success)
            {
                return match.Groups["SpecificationVersionString"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static string XmlNamespace(SpecificationVersion specificationVersion) {
            return $"http://cyclonedx.org/schema/bom/{VersionString(specificationVersion)}";
        }

        public static string VersionString(SpecificationVersion specificationVersion)
        {
            switch (specificationVersion)
            {
                case SpecificationVersion.v1_3:
                    return "1.3";
                case SpecificationVersion.v1_2:
                    return "1.2";
                case SpecificationVersion.v1_1:
                    return "1.1";
                case SpecificationVersion.v1_0:
                    return "1.0";
                default:
                    throw new ArgumentOutOfRangeException($"Unhandled specification version value: {specificationVersion}");
            }
        }

        public static Bom GetBomForSerialization(Bom bom)
        {
            if (bom.SpecVersion == SpecificationVersionHelpers.CurrentVersion)
            {
                return bom;
            }
            else
            {
                var downgradedBom = SpecificationVersionHelpers.CopyBomAndDowngrade(bom);
                return downgradedBom;
            }
        }

        internal static Bom CopyBomAndDowngrade(Bom bom)
        {
            var bomCopy = BomDeepCopy(bom);

            // we downgrade stuff starting with lowest spec first
            // this will remove entire classes of things and will save unnecessary processing further down
            if (bomCopy.SpecVersion < SpecificationVersion.v1_1)
            {
                bomCopy.SerialNumber = null;
                bomCopy.ExternalReferences = null;

                EnumerateAllComponents(bomCopy, (component) => {
                    component.BomRef = null;
                    component.Pedigree = null;
                    component.ExternalReferences = null;
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_2)
            {
                bomCopy.Metadata = null;
                bomCopy.Dependencies = null;
                bomCopy.Services = null;

                EnumerateAllComponents(bomCopy, (component) => {
                    component.Author = null;
                    component.MimeType = null;
                    component.Supplier = null;
                    component.Swid = null;

                    if (component.Pedigree != null)
                    {
                        component.Pedigree.Patches = null;
                    }
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_3)
            {
                bomCopy.Compositions = null;
                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.Licenses = null;
                    bomCopy.Metadata.Properties = null;
                }

                EnumerateAllComponents(bomCopy, (component) => {
                    component.Properties = null;
                    component.Evidence = null;
                    if (component.ExternalReferences != null)
                    {
                        foreach (var extRef in component.ExternalReferences)
                        {
                            extRef.Hashes = null;
                        }
                    }
                });

                if (bomCopy.Services != null)
                {
                    foreach (var service in bomCopy.Services)
                    {
                        service.Properties = null;
                        if (service.ExternalReferences != null)
                        {
                            foreach (var extRef in service.ExternalReferences)
                            {
                                extRef.Hashes = null;
                            }
                        }
                    }
                }
            }

            return bomCopy;
        }

        internal static Bom BomDeepCopy(Bom bom)
        {
            var protoBom = Protobuf.Serializer.SerializeForDeepCopy(bom);
            var bomCopy = Protobuf.Serializer.Deserialize(protoBom);
            return bomCopy;
        }

        internal static void EnumerateAllComponents(Bom bom, Action<Component> callback)
        {
            var q = new Queue<Component>();

            if (bom.Metadata?.Component != null)
            {
                q.Enqueue(bom.Metadata.Component);
            }

            if (bom.Components != null)
            {
                foreach (var component in bom.Components)
                {
                    q.Enqueue(component);
                }
            }

            while (q.Count > 0)
            {
                var currentComponent = q.Dequeue();
                
                callback(currentComponent);

                if (currentComponent.Components != null)
                {
                    foreach (var c in currentComponent.Components)
                    {
                        q.Enqueue(c);
                    }
                }
                if (currentComponent.Pedigree?.Ancestors != null)
                {
                    foreach (var c in currentComponent.Pedigree.Ancestors)
                    {
                        q.Enqueue(c);
                    }
                }
                if (currentComponent.Pedigree?.Descendants != null)
                {
                    foreach (var c in currentComponent.Pedigree.Descendants)
                    {
                        q.Enqueue(c);
                    }
                }
                if (currentComponent.Pedigree?.Variants != null)
                {
                    foreach (var c in currentComponent.Pedigree.Variants)
                    {
                        q.Enqueue(c);
                    }
                }
            }
        }
    }
}