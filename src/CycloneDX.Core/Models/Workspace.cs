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
    public class Workspace : IEquatable<Workspace>
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
        public bool ShouldSerializeAliases() { return Aliases?.Count > 0; }

        [XmlElement("description")]
        [ProtoMember(5)]
        public string Description { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(6)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        
        [XmlElement("resourceReferences")]
        [ProtoMember(7)]
        public ResourceReferenceChoices ResourceReferences { get; set; }
        public bool ShouldSerializeResourceReferences() { return ResourceReferences?.Count > 0; }

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Workspace);
        }

        public bool Equals(Workspace obj)
        {
            return obj != null &&
                (this.AccessMode.Equals(obj.AccessMode)) &&
                (object.ReferenceEquals(this.Aliases, obj.Aliases) ||
                this.Aliases.SequenceEqual(obj.Aliases)) &&
                (object.ReferenceEquals(this.BomRef, obj.BomRef) ||
                this.BomRef.Equals(obj.BomRef)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.Description.Equals(obj.Description)) &&
                (object.ReferenceEquals(this.ManagedDateType, obj.ManagedDateType) ||
                this.ManagedDateType.Equals(obj.ManagedDateType, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.MountPath, obj.MountPath) ||
                this.MountPath.Equals(obj.MountPath, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Properties, obj.Properties) ||
                this.Properties.SequenceEqual(obj.Properties)) &&
                (object.ReferenceEquals(this.ResourceReferences, obj.ResourceReferences) ||
                this.ResourceReferences.Equals(obj.ResourceReferences)) &&
                (object.ReferenceEquals(this.Uid, obj.Uid) ||
                this.Uid.Equals(obj.Uid)) &&
                (object.ReferenceEquals(this.Volume, obj.Volume) ||
                this.Volume.Equals(obj.Volume)) &&
                (object.ReferenceEquals(this.VolumeRequest, obj.VolumeRequest) ||
                this.VolumeRequest.Equals(obj.VolumeRequest));
        }
    }
}