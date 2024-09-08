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
using System.Xml.Serialization;
using CycloneDX.Models.Vulnerabilities;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlRoot("bom", IsNullable=false)]
    [ProtoContract]
    public class Bom : ICloneable
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
        
        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(7)]
        public List<ExternalReference> ExternalReferences { get; set; }
        
        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        [ProtoMember(8)]
        public List<Dependency> Dependencies { get; set; }
        
        [XmlArray("compositions")]
        [XmlArrayItem("composition")]
        [ProtoMember(9)]
        public List<Composition> Compositions { get; set; }

        [XmlArray("vulnerabilities")]
        [XmlArrayItem("vulnerability")]
        [ProtoMember(10)]
        public List<Vulnerabilities.Vulnerability> Vulnerabilities { get; set; }
        
        [XmlArray("annotations")]
        [XmlArrayItem("annotation")]
        [ProtoMember(11)]
        public List<Annotation> Annotations { get; set; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(12)]
        public List<Property> Properties { get; set; }
        
        [XmlArray("formulation")]
        [XmlArrayItem("formula")]
        [ProtoMember(13)]
        public List<Formula> Formulation { get; set; }
        
        public bool ShouldSerializeNonNullableVersion() { return Version.HasValue; }
        public bool ShouldSerializeServices() { return Services?.Count > 0; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }
        public bool ShouldSerializeDependencies() { return Dependencies?.Count > 0; }
        public bool ShouldSerializeVulnerabilities() { return Vulnerabilities?.Count > 0; }
        public bool ShouldSerializeAnnotations() { return Annotations?.Count > 0; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        public bool ShouldSerializeFormulation() { return Formulation?.Count > 0; }

        public object Clone()
        {
            return new Bom()
            {
                Annotations = this.Annotations.Select(x => (Annotation)x.Clone()).ToList(),
                Components = this.Components.Select(x => (Component)x.Clone()).ToList(),
                Compositions = this.Compositions.Select(x => (Composition)x.Clone()).ToList(),
                Dependencies = this.Dependencies.Select(x => (Dependency)x.Clone()).ToList(),
                ExternalReferences = this.ExternalReferences.Select(x => (ExternalReference)x.Clone()).ToList(),
                Formulation = this.Formulation.Select(x => (Formula)x.Clone()).ToList(),
                Metadata = (Metadata)this.Metadata.Clone(),
                NonNullableVersion = this.NonNullableVersion,
                Properties = this.Properties.Select(x => (Property)x.Clone()).ToList(),
                SerialNumber = this.SerialNumber,
                Services = this.Services.Select(x => (Service)x.Clone()).ToList(),
                SpecVersion = this.SpecVersion,
                SpecVersionString = this.SpecVersionString,
                Version = this.Version,
                Vulnerabilities = this.Vulnerabilities.Select(x => (Vulnerability)x.Clone()).ToList(),
            };
        }
    }
}