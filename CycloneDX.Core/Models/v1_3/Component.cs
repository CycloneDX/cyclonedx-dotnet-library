// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("component")]
    [ProtoContract]
    public class Component
    {
        [ProtoContract]
        public enum Classification
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "application")]
            Application,
            [XmlEnum(Name = "framework")]
            Framework,
            [XmlEnum(Name = "library")]
            Library,
            [XmlEnum(Name = "operating-system")]
            OperationSystem,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "file")]
            File,
            [XmlEnum(Name = "container")]
            Container,
            [XmlEnum(Name = "firmware")]
            Firmware
        }

        [ProtoContract]
        public enum ComponentScope
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "required")]
            Required,
            [XmlEnum(Name = "optional")]
            Optional,
            [XmlEnum(Name = "excluded")]
            Excluded
        }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public Classification Type { get; set; }

        [JsonPropertyName("mime-type")]
        [XmlAttribute("mime-type")]
        [ProtoMember(2)]
        public string MimeType { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(3)]
        public string BomRef { get; set; }

        [XmlElement("supplier")]
        [ProtoMember(4)]
        public OrganizationalEntity Supplier { get; set; }

        [XmlElement("author")]
        [ProtoMember(5)]
        public string Author { get; set; }

        [XmlElement("publisher")]
        [ProtoMember(6)]
        public string Publisher { get; set; }

        [XmlElement("group")]
        [ProtoMember(7)]
        public string Group { get; set; }

        [XmlElement("name")]
        [ProtoMember(8)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(9)]
        public string Version { get; set; }

        [XmlElement("description")]
        [ProtoMember(10)]
        public string Description { get; set; }

        [XmlIgnore]
        [ProtoMember(11)]
        public ComponentScope? Scope { get; set; }
        [XmlElement("scope")]
        [JsonIgnore]
        public ComponentScope NonNullableScope
        {
            get
            {
                return Scope.Value;
            }
            set
            {
                Scope = value;
            }
        }
        public bool ShouldSerializeNonNullableScope() { return Scope.HasValue; }

        [XmlArray("hashes")]
        [ProtoMember(12)]
        public List<Hash> Hashes { get; set; }

        [XmlElement("licenses")]
        [ProtoMember(13)]
        public List<LicenseChoice> Licenses { get; set; }

        [XmlElement("copyright")]
        [ProtoMember(14)]
        public string Copyright { get; set; }

        [XmlElement("cpe")]
        [ProtoMember(15)]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        [ProtoMember(16)]
        public string Purl { get; set; }

        [XmlElement("swid")]
        [ProtoMember(17)]
        public Swid Swid { get; set; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
        [ProtoMember(18)]
        public bool? Modified { get; set; }
        [XmlElement("modified")]
        [JsonIgnore]
        public bool NonNullableModified
        {
            get
            {
                return Modified.HasValue && Modified.Value;
            }
            set
            {
                Modified = value;
            }
        }
        public bool ShouldSerializeNonNullableModified() { return Modified.HasValue; }

        [XmlElement("pedigree")]
        [ProtoMember(19)]
        public Pedigree Pedigree { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(20)]
        public List<ExternalReference> ExternalReferences { get; set; }
        
        [XmlArray("components")]
        [ProtoMember(21)]
        public List<Component> Components { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(22)]
        public List<Property> Properties { get; set; }
        
        [XmlElement("evidence")]
        [ProtoMember(23)]
        public Evidence Evidence { get; set; }
        
        public Component() {}

        public Component(v1_2.Component component)
        {
            Type = (Classification)((int)component.Type + 1);
            MimeType = component.MimeType;
            BomRef = component.BomRef;
            if (component.Supplier != null)
                Supplier = new OrganizationalEntity(component.Supplier);
            Author = component.Author;
            Publisher = component.Publisher;
            Group = component.Group;
            Name = component.Name;
            Version = component.Version;
            Description = component.Description;
            if (component.Scope.HasValue) Scope = (ComponentScope)((int)component.Scope + 1);
            if (component.Hashes != null)
            {
                Hashes = new List<Hash>();
                foreach (var hash in component.Hashes)
                {
                    Hashes.Add(new Hash(hash));
                }
            }
            if (component.Licenses != null)
            {
                Licenses = new List<LicenseChoice>();
                foreach (var componentLicense in component.Licenses)
                {
                    Licenses.Add(new LicenseChoice(componentLicense));
                }
            }
            Copyright = component.Copyright;
            Cpe = component.Cpe;
            Purl = component.Purl;
            if (component.Swid != null)
                Swid = new Swid(component.Swid);
            Modified = component.Modified;
            if (component.Pedigree != null)
                Pedigree = new Pedigree(component.Pedigree);
            if (component.ExternalReferences != null)
            {
                ExternalReferences = new List<ExternalReference>();
                foreach (var externalReference in component.ExternalReferences)
                {
                    ExternalReferences.Add(new ExternalReference(externalReference));
                }
            }
            if (component.Components != null)
            {
                Components = new List<Component>();
                foreach (var subComponent in component.Components)
                {
                    Components.Add(new Component(subComponent));
                }
            }
        }
    }
}