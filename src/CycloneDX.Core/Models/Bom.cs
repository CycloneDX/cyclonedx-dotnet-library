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
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlRoot("bom", IsNullable=false)]
    [ProtoContract]
    public class Bom : BomEntity
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

        // TODO: MergeWith() might be reasonable but is currently handled
        // by several strategy implementations in CycloneDX.Utils Merge.cs
        // so maybe there should be sub-classes or strategy arguments or
        // properties to select one of those implementations at run-time?..

        /// <summary>
        /// Add reference to this currently running build of cyclonedx-cli
        /// (likely) and this cyclonedx-dotnet-library into the Metadata/Tools
        /// of this Bom document. Intended for use after processing which
        /// creates or modifies the document. After all - any bugs appearing
        /// due to library routines are our own and should be trackable...
        ///
        /// NOTE: Tries to not add identical duplicate entries.
        /// </summary>
        public void BomMetadataReferThisToolkit()
        {
            // Per https://stackoverflow.com/a/36351902/4715872 :
            // Use System.Reflection.Assembly.GetExecutingAssembly()
            // to get the assembly (that this line of code is in), or
            // use System.Reflection.Assembly.GetEntryAssembly() to
            // get the assembly your project started with (most likely
            // this is your app). In multi-project solutions this is
            // something to keep in mind!
            #pragma warning disable 618
            Tool toolThisLibrary = new Tool
            {
                Vendor = "OWASP Foundation",
                Name = Assembly.GetExecutingAssembly().GetName().Name, // "cyclonedx-dotnet-library"
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            #pragma warning restore 618

            if (this.Metadata is null)
            {
                this.Metadata = new Metadata();
            }

            if (this.Metadata.Tools is null || this.Metadata.Tools.Tools is null)
            {
                #pragma warning disable 618
                this.Metadata.Tools = new ToolChoices
                {
                    Tools = new List<Tool>(new [] {toolThisLibrary}),
                };
                #pragma warning restore 618
            }
            else
            {
                if (!this.Metadata.Tools.Tools.Contains(toolThisLibrary))
                {
                    this.Metadata.Tools.Tools.Add(toolThisLibrary);
                }
            }

            // At worst, these would dedup away?..
            string toolThisScriptName = Assembly.GetEntryAssembly().GetName().Name; // "cyclonedx-cli" or similar
            if (toolThisScriptName != toolThisLibrary.Name)
            {
                #pragma warning disable 618
                Tool toolThisScript = new Tool
                {
                    Name = toolThisScriptName,
                    Vendor = (toolThisScriptName.ToLowerInvariant().StartsWith("cyclonedx") ? "OWASP Foundation" : null),
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString()
                };
                #pragma warning restore 618

                if (!this.Metadata.Tools.Tools.Contains(toolThisScript))
                {
                    this.Metadata.Tools.Tools.Add(toolThisScript);
                }
            }
        }

        /// <summary>
        /// Update the Metadata/Timestamp of this Bom document
        /// (after content manipulations such as a merge)
        /// using DateTime.Now.
        ///
        /// NOTE: Creates a new Metadata object to populate
        /// the property, if one was missing in this Bom object.
        /// </summary>
        public void BomMetadataUpdateTimestamp()
        {
            if (this.Metadata is null)
            {
                this.Metadata = new Metadata();
            }

            this.Metadata.Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Update the SerialNumber and optionally bump the Version
        /// of a Bom document issued with such serial number (both
        /// not in the Metadata structure, but still are "meta data")
        /// of this Bom document, either using a new random UUID as
        /// the SerialNumber and assigning a Version=1, or bumping
        /// the Version -- usually done after content manipulations
        /// such as a merge, depending on their caller-defined impact.
        /// </summary>
        public void BomMetadataUpdateSerialNumberVersion(bool generateNewSerialNumber)
        {
            bool doGenerateNewSerialNumber = generateNewSerialNumber;
            if (this.Version is null || this.Version < 1 || this.SerialNumber is null || this.SerialNumber == "")
            {
                doGenerateNewSerialNumber = true;
            }

            if (doGenerateNewSerialNumber)
            {
                this.Version = 1;
                this.SerialNumber = "urn:uuid:" + System.Guid.NewGuid().ToString();
            }
            else
            {
                this.Version++;
            }
        }

        /// <summary>
        /// Set up (default or update) meta data of this Bom document,
        /// covering the Version, SerialNumber and Metadata/Timestamp
        /// in one shot. Typically useful to brush up a `new Bom()` or
        /// to ensure a new identity for a modified Bom document.
        ///
        /// NOTE: caller may want to BomMetadataReferThisToolkit()
        /// separately, to add the Metadata/Tools[] entries about this
        /// CycloneDX library and its consumer (e.g. the "cyclonedx-cli"
        /// program).
        /// </summary>
        public void BomMetadataUpdate(bool generateNewSerialNumber)
        {
            this.BomMetadataUpdateSerialNumberVersion(generateNewSerialNumber);
            this.BomMetadataUpdateTimestamp();
        }

        /// <summary>
        /// Prepare a BomWalkResult discovery report starting from
        /// this Bom document. Callers can cache it to re-use for
        /// repetitive operations.
        /// </summary>
        /// <returns></returns>
        public BomWalkResult WalkThis()
        {
            BomWalkResult res = new BomWalkResult();
            res.reset(this);

            // Note: passing "container=null" should be safe here, as
            // long as this Bom type does not have a BomRef property.
            res.SerializeBomEntity_BomRefs(this, null);

            return res;
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
        public Dictionary<BomEntity, List<BomEntity>> GetBomRefsInContainers(BomWalkResult res)
        {
            if (res.bomRoot != this)
            {
                // throw?
                return null;
            }
            return res.dictRefsInContainers;
        }

        /// <summary>
        /// This is a run-once method to get a dictionary.
        /// See GetBomRefsInContainers(BomWalkResult) for one using a cache
        /// prepared by WalkThis() for mass manipulations.
        /// </summary>
        /// <returns></returns>
        public Dictionary<BomEntity, List<BomEntity>> GetBomRefsInContainers()
        {
            BomWalkResult res = WalkThis();
            return GetBomRefsInContainers(res);
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
        public Dictionary<BomEntity, BomEntity> GetBomRefsWithContainer(BomWalkResult res)
        {
            if (res.bomRoot != this)
            {
                // throw?
                return null;
            }
            return res.GetBomRefsWithContainer();
        }

        /// <summary>
        /// This is a run-once method to get a dictionary.
        /// See GetBomRefsWithContainer(BomWalkResult) for one using a cache
        /// prepared by WalkThis() for mass manipulations.
        /// </summary>
        /// <returns></returns>
        public Dictionary<BomEntity, BomEntity> GetBomRefsWithContainer()
        {
            BomWalkResult res = WalkThis();
            return res.GetBomRefsWithContainer();
        }

        /// <summary>
        /// Rename all occurrences of the "BomRef" (its value definition
        /// to name an entity, if present in this Bom document, and the
        /// references to it from other entities).
        ///
        /// This version of the method considers a cache of information
        /// about current BomEntity relationships in this document, as
        /// prepared by an earlier call to GetBomRefsWithContainer() and
        /// cached by caller (may speed up the loops in case of massive
        /// processing).
        /// </summary>
        /// <param name="oldRef">Old value of BomRef</param>
        /// <param name="newRef">New value of BomRef</param>
        /// <param name="dict">Cached output of earlier GetBomRefsWithContainer();
        ///     contents of the cache can change due to successful renaming
        ///     to keep reflecting BomEntity relations in this document.
        /// </param>
        /// <returns>
        ///     False if had no hits, had collisions, etc.;
        ///     True if renamed something without any errors.
        ///
        ///     TODO: throw Exceptions instead of False,
        ///     to help callers discern the error cases?
        /// </returns>
        public bool RenameBomRef(string oldRef, string newRef, BomWalkResult res)
        {
            return false;
        }

        /// <summary>
        /// See related method
        ///     RenameBomRef(string oldRef, string newRef, Dictionary<BomEntity, BomEntity> dict)
        /// for details.
        ///
        /// This version of the method prepares and discards the helper
        /// dictionary with mapping of cross-referencing entities, and
        /// is easier to use in code for single-use cases but is less
        /// efficient for massive processing loops.
        /// </summary>
        /// <param name="oldRef">Old value of BomRef</param>
        /// <param name="newRef">New value of BomRef</param>
        /// <returns>False if had no hits; True if renamed something without any errors</returns>
        public bool RenameBomRef(string oldRef, string newRef)
        {
            BomWalkResult res = WalkThis();
            return this.RenameBomRef(oldRef, newRef, res);
        }
    }
}
