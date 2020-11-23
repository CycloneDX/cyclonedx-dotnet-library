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

namespace CycloneDX.Models.v1_1
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("component")]
    public class Component
    {
        public enum ComponentType
        {
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
            File
        }

        [XmlAttribute("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        public string BomRef { get; set; }

        [XmlElement("author")]
        public string Author { get; set; }

        [XmlElement("publisher")]
        public string Publisher { get; set; }

        [XmlElement("group")]
        public string Group { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("scope")]
        public string Scope { get; set; }

        [XmlArray("hashes")]
        public List<Hash> Hashes { get; set; }

        [XmlElement("licenses")]
        public List<ComponentLicense> Licenses { get; set; }

        [XmlElement("copyright")]
        public string Copyright { get; set; }

        [XmlElement("cpe")]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        public string Purl { get; set; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
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
        public Pedigree Pedigree { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        public List<ExternalReference> ExternalReferences { get; set; }
        
        [XmlArray("components")]
        public List<Component> Components { get; set; }

        public Component() {}

        public Component(v1_0.Component component)
        {
            Type = (ComponentType)(int)component.Type;
            Author = component.Author;
            Publisher = component.Publisher;
            Group = component.Group;
            Name = component.Name;
            Version = component.Version;
            Description = component.Description;
            Scope = component.Scope;
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
                Licenses = new List<ComponentLicense>();
                foreach (var componentLicense in component.Licenses)
                {
                    Licenses.Add(new ComponentLicense(componentLicense));
                }
            }
            Copyright = component.Copyright;
            Cpe = component.Cpe;
            Purl = component.Purl;
            Modified = component.Modified;
            if (component.Components != null)
            {
                Components = new List<Component>();
                foreach (var subComponent in component.Components)
                {
                    Components.Add(new Component(subComponent));
                }
            }
        }

        public Component(v1_2.Component component)
        {
            Type = (ComponentType)(int)component.Type;
            BomRef = component.BomRef;
            Author = component.Author;
            Publisher = component.Publisher;
            Group = component.Group;
            Name = component.Name;
            Version = component.Version;
            Description = component.Description;
            Scope = component.Scope;
            if (component.Hashes != null)
            {
                Hashes = new List<Hash>();
                foreach (var hash in component.Hashes)
                {
                    var convertedHash = new Hash(hash);
                    if (Enum.IsDefined(typeof(Hash.HashAlgorithm), convertedHash.Alg))
                    {
                        Hashes.Add(convertedHash);
                    }
                }
            }
            if (component.Licenses != null)
            {
                Licenses = new List<ComponentLicense>();
                foreach (var componentLicense in component.Licenses)
                {
                    Licenses.Add(new ComponentLicense(componentLicense));
                }
            }
            Copyright = component.Copyright;
            Cpe = component.Cpe;
            Purl = component.Purl;
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