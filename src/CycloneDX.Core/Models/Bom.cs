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
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlRoot("bom", IsNullable=false)]
    [ProtoContract]
    public class Bom
    {
        [XmlIgnore]
        public string BomFormat => "CycloneDX";

        private SpecificationVersion _specVersion = SpecificationVersionHelpers.CurrentVersion;
        [XmlIgnore]
        [JsonIgnore]
        public SpecificationVersion SpecVersion
        {
            get => _specVersion;
            set
            {
                _specVersion = value;
                // this is horrible, but I can't get the XML serializer to cooperate with me otherwise
                BomUtils.EnumerateAllToolChoices(this, (toolChoice) =>
                {
                    toolChoice.SpecVersion = _specVersion;
                });
                BomUtils.EnumerateAllServices(this, (service) =>
                {
                    service.SpecVersion = _specVersion;
                    if (service.XmlData != null)
                    {
                        service.XmlData.SpecVersion = _specVersion;
                    }
                });
                BomUtils.EnumerateAllDatasetChoices(this, (DatasetChoices) =>
                {
                    DatasetChoices.SpecVersion = _specVersion;
                });
            }
        }

        // For JSON we could use a custom converter
        // but this works nicely for protobuf too
        [XmlIgnore]
        [ProtoMember(1)]
        [JsonPropertyName("specVersion")]
        public string SpecVersionString
        {
            get => SpecificationVersionHelpers.VersionString(SpecVersion);
            set
            {
                switch (value)
                {
                    case "1.0":
                        SpecVersion = SpecificationVersion.v1_0;
                        break;
                    case "1.1":
                        SpecVersion = SpecificationVersion.v1_1;
                        break;
                    case "1.2":
                        SpecVersion = SpecificationVersion.v1_2;
                        break;
                    case "1.3":
                        SpecVersion = SpecificationVersion.v1_3;
                        break;
                    case "1.4":
                        SpecVersion = SpecificationVersion.v1_4;
                        break;
                    case "1.5":
                        SpecVersion = SpecificationVersion.v1_5;
                        break;
                    case "1.6":
                        SpecVersion = SpecificationVersion.v1_6;
                        break;
                    case "1.7":
                        SpecVersion = SpecificationVersion.v1_7;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported specification version: {value}");
                }
            }
        }

        [XmlAttribute("serialNumber")]
        [ProtoMember(3)]
        public string SerialNumber { get; set; }

        [XmlIgnore]
        [ProtoMember(2)]
        public int? Version { get; set; }
        [XmlAttribute("version")]
        [JsonIgnore]
        public int NonNullableVersion
        {
            get
            {
                return Version.Value;
            }
            set
            {
                Version = value;
            }
        }
        public bool ShouldSerializeNonNullableVersion() { return Version.HasValue; }

        [XmlElement("metadata")]
        [ProtoMember(4)]
        public Metadata Metadata { get; set; }

        [XmlArray("components")]
        [XmlArrayItem("component")]
        [ProtoMember(5)]
        public List<Component> Components { get; set; }

        [XmlArray("services")]
        [XmlArrayItem("service")]
        [ProtoMember(6)]
        public List<Service> Services { get; set; }
        public bool ShouldSerializeServices() { return Services?.Count > 0; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(7)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }

        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        [ProtoMember(8)]
        public List<Dependency> Dependencies { get; set; }
        public bool ShouldSerializeDependencies() { return Dependencies?.Count > 0; }

        [XmlArray("compositions")]
        [XmlArrayItem("composition")]
        [ProtoMember(9)]
        public List<Composition> Compositions { get; set; }

        [XmlArray("vulnerabilities")]
        [XmlArrayItem("vulnerability")]
        [ProtoMember(10)]
        public List<Vulnerabilities.Vulnerability> Vulnerabilities { get; set; }
        public bool ShouldSerializeVulnerabilities() { return Vulnerabilities?.Count > 0; }
        
        [XmlArray("annotations")]
        [XmlArrayItem("annotation")]
        [ProtoMember(11)]
        public List<Annotation> Annotations { get; set; }
        public bool ShouldSerializeAnnotations() { return Annotations?.Count > 0; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(12)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        
        [XmlArray("formulation")]
        [XmlArrayItem("formula")]
        [ProtoMember(13)]
        public List<Formula> Formulation { get; set; }
        public bool ShouldSerializeFormulation() { return Formulation?.Count > 0; }


        [XmlElement("declarations")]
        [ProtoMember(14)]
        public Declarations Declarations { get; set; }
        public bool ShouldSerializeDeclarations() { return Declarations != null; }

        [XmlElement("definitions")]
        [ProtoMember(15)]
        public Definitions Definitions { get; set; }

        [XmlArray("citations")]
        [XmlArrayItem("citation")]
        [ProtoMember(16)]
        public List<Citation> Citations { get; set; }
        public bool ShouldSerializeCitations() { return Citations?.Count > 0; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }
        [XmlIgnore]
        public SignatureChoice Signature { get; set; }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Rename a "bom-ref" identifier and every back-reference to it
        /// throughout this document (Dependency.Ref, Composition
        /// assemblies/dependencies, Vulnerability.Affects[].Ref, annotation
        /// subjects, ...), via <see cref="BomRefWalker"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="oldRef"/> was found and rewritten
        /// somewhere in the document; <c>false</c> if it was not present
        /// (a non-fatal no-op) or the arguments were invalid.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="newRef"/> is already used as a bom-ref identifier
        /// or back-reference somewhere else in this document. Renaming
        /// anyway would silently make two different entities share one
        /// bom-ref (or repoint an existing back-reference at the wrong
        /// entity) -- refused rather than done.
        /// </exception>
        public bool RenameRef(string oldRef, string newRef)
        {
            if (string.IsNullOrEmpty(oldRef) || string.IsNullOrEmpty(newRef) || oldRef == newRef)
            {
                return false;
            }

            // Read-only pass (rewrite function returns its input unchanged,
            // just records it) to check for a collision before touching
            // anything -- reuses the same traversal RewriteRefs uses for
            // the real rewrite, so "what counts as a ref" can't drift
            // between the check and the actual rename.
            var existingRefs = new HashSet<string>();
            BomRefWalker.RewriteRefs(this, r =>
            {
                if (!string.IsNullOrEmpty(r))
                {
                    existingRefs.Add(r);
                }
                return r;
            });

            if (!existingRefs.Contains(oldRef))
            {
                return false;
            }
            if (existingRefs.Contains(newRef))
            {
                throw new InvalidOperationException($"Cannot rename \"{oldRef}\" to \"{newRef}\": \"{newRef}\" is already used as a bom-ref (or a reference to one) elsewhere in this document.");
            }

            bool found = false;
            BomRefWalker.RewriteRefs(this, r =>
            {
                if (r == oldRef)
                {
                    found = true;
                    return newRef;
                }
                return r;
            });
            return found;
        }

        /// <summary>
        /// Add a reference to this running build of cyclonedx-dotnet-library
        /// (and, if different, the entry assembly -- typically a consuming
        /// tool like cyclonedx-cli) into this document's Metadata/Tools.
        /// Intended for use after processing that creates or modifies a
        /// document, so any bugs in the processing are traceable to the
        /// tool/library versions that produced the result. Avoids adding
        /// exact-duplicate entries.
        /// </summary>
        public void BomMetadataReferThisToolkit()
        {
#pragma warning disable 618
            var toolThisLibrary = new Tool
            {
                Vendor = "OWASP Foundation",
                Name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
#pragma warning restore 618

            if (Metadata is null)
            {
                Metadata = new Metadata();
            }

            if (Metadata.Tools is null || Metadata.Tools.Tools is null)
            {
#pragma warning disable 618
                Metadata.Tools = new ToolChoices
                {
                    Tools = new List<Tool>(new[] { toolThisLibrary }),
                };
#pragma warning restore 618
            }
            else if (!Metadata.Tools.Tools.Contains(toolThisLibrary))
            {
                Metadata.Tools.Tools.Add(toolThisLibrary);
            }

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var toolThisScriptName = entryAssembly?.GetName()?.Name;
            if (!string.IsNullOrEmpty(toolThisScriptName) && toolThisScriptName != toolThisLibrary.Name)
            {
#pragma warning disable 618
                var toolThisScript = new Tool
                {
                    Name = toolThisScriptName,
                    Vendor = toolThisScriptName.ToLowerInvariant().StartsWith("cyclonedx", StringComparison.Ordinal) ? "OWASP Foundation" : null,
                    Version = entryAssembly.GetName().Version.ToString()
                };
#pragma warning restore 618

                if (!Metadata.Tools.Tools.Contains(toolThisScript))
                {
                    Metadata.Tools.Tools.Add(toolThisScript);
                }
            }
        }

        /// <summary>
        /// Refresh this document's own identity: Version/SerialNumber and
        /// Metadata/Timestamp. Typically called after content
        /// manipulations such as a merge or rename. Callers usually also
        /// want <see cref="BomMetadataReferThisToolkit"/> separately.
        /// </summary>
        public void BomMetadataUpdate(bool generateNewSerialNumber)
        {
            if (Version is null || Version < 1 || string.IsNullOrEmpty(SerialNumber))
            {
                generateNewSerialNumber = true;
            }

            if (generateNewSerialNumber)
            {
                Version = 1;
                SerialNumber = "urn:uuid:" + Guid.NewGuid().ToString();
            }
            else
            {
                Version++;
            }

            if (Metadata is null)
            {
                Metadata = new Metadata();
            }
            Metadata.Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Find every top-level Component (in Metadata.Component and
        /// Components) that no dependsOn edge anywhere in the document's
        /// Dependencies tree ever targets. These are structurally valid
        /// per the JSON/XML schema, but invisible to any consumer that
        /// walks the dependency graph from the document's subject rather
        /// than scanning the flat Components list (e.g. Dependency-Track).
        /// A flat merge of many independently-generated documents can
        /// easily leave some of them like this: FlatMerge unions each
        /// input's own Dependencies, but never guarantees the union stays
        /// one connected graph -- that depends entirely on dependsOn edges
        /// the inputs already had.
        ///
        /// Buckets whatever is found by Scope (Excluded, Optional,
        /// Required, unspecified) and gives each non-empty bucket its own
        /// synthetic "attachment" Component, wired into the dependency
        /// graph as a child of the existing dependency-list entry named by
        /// <paramref name="attachmentRef"/> (or this document's own
        /// subject, Metadata.Component.BomRef, if
        /// <paramref name="attachmentRef"/> is null or doesn't identify an
        /// existing entry).
        /// </summary>
        /// <returns>
        /// The bom-refs of newly-created attachment Components, each
        /// mapped to the dangling bom-refs attached under it. Empty if
        /// none were found, or if there was no subject to attach under.
        /// </returns>
        public Dictionary<string, List<string>> AttachDanglingComponents(string attachmentRef = null)
        {
            var known = new Dictionary<string, Component>();
            if (Metadata?.Component != null && !string.IsNullOrEmpty(Metadata.Component.BomRef))
            {
                known[Metadata.Component.BomRef] = Metadata.Component;
            }
            if (Components != null)
            {
                foreach (var c in Components)
                {
                    if (!string.IsNullOrEmpty(c.BomRef))
                    {
                        known[c.BomRef] = c;
                    }
                }
            }

            var referenced = new HashSet<string>();
            void WalkReferenced(List<Dependency> deps, bool isTopLevel)
            {
                if (deps is null)
                {
                    return;
                }
                foreach (var d in deps)
                {
                    if (!isTopLevel && !string.IsNullOrEmpty(d.Ref))
                    {
                        referenced.Add(d.Ref);
                    }
                    WalkReferenced(d.Dependencies, false);
                }
            }
            WalkReferenced(Dependencies, true);

            var rootRef = Metadata?.Component?.BomRef;
            var dangling = new Dictionary<string, List<string>>();
            foreach (var kvp in known)
            {
                if (kvp.Key == rootRef || referenced.Contains(kvp.Key))
                {
                    continue;
                }
                var scopeLabel = kvp.Value.Scope?.ToString() ?? "Unspecified";
                if (!dangling.TryGetValue(scopeLabel, out var list))
                {
                    list = new List<string>();
                    dangling[scopeLabel] = list;
                }
                list.Add(kvp.Key);
            }

            var result = new Dictionary<string, List<string>>();
            if (dangling.Count == 0)
            {
                return result;
            }

            if (Dependencies is null)
            {
                Dependencies = new List<Dependency>();
            }

            var attachmentEntry = string.IsNullOrEmpty(attachmentRef)
                ? null
                : Dependencies.FirstOrDefault(d => d.Ref == attachmentRef);
            if (attachmentEntry is null)
            {
                attachmentEntry = string.IsNullOrEmpty(rootRef)
                    ? null
                    : Dependencies.FirstOrDefault(d => d.Ref == rootRef);
                if (attachmentEntry is null && !string.IsNullOrEmpty(rootRef))
                {
                    attachmentEntry = new Dependency { Ref = rootRef };
                    Dependencies.Add(attachmentEntry);
                }
            }
            if (attachmentEntry is null)
            {
                // No subject at all to attach under -- nothing safe to do.
                return result;
            }
            attachmentEntry.Dependencies ??= new List<Dependency>();

            if (Components is null)
            {
                Components = new List<Component>();
            }

            var usedRefs = new HashSet<string>(known.Keys);
            foreach (var kvp in dangling)
            {
                var scopeLabel = kvp.Key;
                var baseRef = $"unreferenced-components:scope={scopeLabel}";
                var newRef = baseRef;
                var suffix = 2;
                while (usedRefs.Contains(newRef))
                {
                    newRef = $"{baseRef}:{suffix}";
                    suffix++;
                }
                usedRefs.Add(newRef);

                Components.Add(new Component
                {
                    BomRef = newRef,
                    Type = Component.Classification.Application,
                    Name = $"unreferenced-components (scope={scopeLabel})",
                });
                Dependencies.Add(new Dependency
                {
                    Ref = newRef,
                    Dependencies = kvp.Value.Select(r => new Dependency { Ref = r }).ToList(),
                });
                attachmentEntry.Dependencies.Add(new Dependency { Ref = newRef });

                result[newRef] = kvp.Value;
            }

            return result;
        }
#endif
    }
}