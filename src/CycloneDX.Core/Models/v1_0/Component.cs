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

namespace CycloneDX.Models.v1_0
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
            Device
        }

        public enum ComponentScope
        {
            [XmlEnum(Name = "required")]
            Required,
            [XmlEnum(Name = "optional")]
            Optional,
            [XmlEnum(Name = "excluded")]
            Excluded
        }

        [XmlAttribute("type")]
        public ComponentType Type { get; set; }

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

        [XmlIgnore]
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
        public List<Hash> Hashes { get; set; }

        [XmlElement("licenses")]
        public List<LicenseChoice> Licenses { get; set; }

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

        [XmlArray("components")]
        public List<Component> Components { get; set; }

        public Component() {}

        public Component(v1_1.Component component)
        {
            Type = (ComponentType)(int)component.Type;
            Author = component.Author;
            Publisher = component.Publisher;
            Group = component.Group;
            Name = component.Name;
            Version = component.Version;
            Description = component.Description;
            if (component.Scope.HasValue) Scope = (ComponentScope)(int)component.Scope;
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
                Licenses = new List<LicenseChoice>();
                foreach (var componentLicense in component.Licenses)
                {
                    Licenses.Add(new LicenseChoice(componentLicense));
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
        }    }
}