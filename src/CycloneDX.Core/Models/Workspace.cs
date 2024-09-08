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
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("workspace")]
    [ProtoContract]
    public class Workspace : ICloneable
    {
        [ProtoContract]
        public enum AccessModeType
        {
            [XmlEnum(Name = "read-only")]
            Read_Only,
            [XmlEnum(Name = "read-write")]
            Read_Write,
            [XmlEnum(Name = "read-write-once")]
            Read_Write_Once,
            [XmlEnum(Name = "write-once")]
            Write_Once,
            [XmlEnum(Name = "write-only")]
            Write_Only,
        }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("uid")]
        [ProtoMember(2)]
        public string Uid { get; set; }

        [XmlElement("name")]
        [ProtoMember(3)]
        public string Name { get; set; }

        [XmlArray("aliases")]
        [XmlArrayItem("alias")]
        [ProtoMember(4)]
        public List<string> Aliases { get; set; }
        
        [XmlElement("description")]
        [ProtoMember(5)]
        public string Description { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(6)]
        public List<Property> Properties { get; set; }
        
        [XmlElement("resourceReferences")]
        [ProtoMember(7)]
        public ResourceReferenceChoices ResourceReferences { get; set; }

        [XmlElement("accessMode")]
        [ProtoMember(8)]
        public AccessModeType AccessMode { get; set; }
        
        [XmlElement("mountPath")]
        [ProtoMember(9)]
        public string MountPath { get; set; }

        [XmlElement("managedDataType")]
        [ProtoMember(10)]
        public string ManagedDateType { get; set; }

        [XmlElement("volumeRequest")]
        [ProtoMember(11)]
        public string VolumeRequest { get; set; }

        [XmlElement("volume")]
        [ProtoMember(12)]
        public Volume Volume { get; set; }

        public bool ShouldSerializeAliases() { return Aliases?.Count > 0; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        public bool ShouldSerializeResourceReferences() { return ResourceReferences?.Count > 0; }

        public object Clone()
        {
            return new Workspace()
            {
                AccessMode = this.AccessMode,
                Aliases = this.Aliases.Select(x => x).ToList(),
                BomRef = this.BomRef,
                Description = this.Description,
                ManagedDateType = this.ManagedDateType,
                MountPath = this.MountPath,
                Name = this.Name,
                Properties = this.Properties.Select(x => (Property)x.Clone()).ToList(),
                ResourceReferences = (ResourceReferenceChoices)this.ResourceReferences.Clone(),
                Uid = this.Uid,
                Volume = (Volume)this.Volume.Clone(),
                VolumeRequest = this.VolumeRequest,
            };
        }
    }
}