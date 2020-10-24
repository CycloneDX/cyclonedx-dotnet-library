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

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    // This suppression should maybe be revisited when/if a CycloneDX library is published
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    [XmlType("component")]
    public class Component : IComparable<Component>
    {
        public enum ComponentType
        {
            [XmlEnum(Name = "application")]
            Application,
            [XmlEnum(Name = "framework")]
            Framework,
            [XmlEnum(Name = "library")]
            Library,
            [XmlEnum(Name = "container")]
            Container,
            [XmlEnum(Name = "operating-system")]
            OperationSystem,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "firmware")]
            Firmware,
            [XmlEnum(Name = "file")]
            File
        }

        [XmlAttribute("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("mime-type")]
        [XmlAttribute("mime-type")]
        public string MimeType { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        public string BomRef { get; set; }

        [XmlElement("supplier")]
        public OrganizationalEntity Supplier { get; set; }

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

        [Obsolete("DEPRECATED - DO NOT USE. This will be removed in a future version.")]
        [XmlElement("cpe")]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        public string Purl { get; set; }

        [XmlElement("swid")]
        public Swid Swid { get; set; }

        [Obsolete("DEPRECATED - DO NOT USE. This will be removed in a future version.")]
        [XmlIgnore]
        public bool? Modified { get; set; }
        [XmlElement("modified")]
        [JsonIgnore]
        [Obsolete("Do not use directly, this is a serialization workaround.")]
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

        public Pedigree Pedigree { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        public List<ExternalReference> ExternalReferences { get; set; }
        
        [XmlArray("components")]
        public List<Component> Components { get; set; }

        public int CompareTo(Component other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                var nameComparison = string.Compare(this.Name.ToUpperInvariant(), other.Name.ToUpperInvariant(), StringComparison.Ordinal);
                return nameComparison == 0
                    ? string.Compare(this.Version, other.Version, StringComparison.Ordinal)
                    : nameComparison;
            }
        }

        public int CompareTo(object obj)
        {
            var other = obj as Component;
            return CompareTo(other);
        }
    }
}