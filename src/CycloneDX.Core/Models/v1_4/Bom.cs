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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_4
{
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlRoot("bom", Namespace="http://cyclonedx.org/schema/bom/1.3", IsNullable=false)]
    [ProtoContract]
    public class Bom
    {
        [XmlIgnore]
        public string BomFormat => "CycloneDX";

        [XmlIgnore]
        [ProtoMember(1)]
        public string SpecVersion { get; set; } = "1.4";

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
        [ProtoMember(5)]
        public List<Component> Components { get; set; } = new List<Component>();

        [XmlArray("services")]
        [XmlArrayItem("service")]
        [ProtoMember(6)]
        public List<Service> Services { get; set; }

        [XmlArray("externalReferences")]
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
        
        public Bom() {}
        
        public Bom(v1_3.Bom bom)
        {
            SerialNumber = bom.SerialNumber;
            Version = bom.Version;
            if (bom.Metadata != null)
            {
                Metadata = new Metadata(bom.Metadata);
            }
            if (bom.Components != null)
            {
                Components = new List<Component>();
                foreach (var component in bom.Components)
                {
                    Components.Add(new Component(component));
                }
            }
            if (bom.Services != null)
            {
                Services = new List<Service>();
                foreach (var service in bom.Services)
                {
                    Services.Add(new Service(service));
                }
            }
            if (bom.ExternalReferences != null)
            {
                ExternalReferences = new List<ExternalReference>();
                foreach (var externalReference in bom.ExternalReferences)
                {
                    ExternalReferences.Add(new ExternalReference(externalReference));
                }
            }
            if (bom.Dependencies != null)
            {
                Dependencies = new List<Dependency>();
                foreach (var dependency in bom.Dependencies)
                {
                    Dependencies.Add(new Dependency(dependency));
                }
            }
            if (bom.Compositions != null)
            {
                Compositions = new List<Composition>();
                foreach (var composition in bom.Compositions)
                {
                    Compositions.Add(new Composition(composition));
                }
            }
        }
    }
}