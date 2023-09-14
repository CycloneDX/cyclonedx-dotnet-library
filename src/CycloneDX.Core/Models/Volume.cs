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

namespace CycloneDX.Models
{
    [XmlType("volume")]
    [ProtoContract]
    public class Volume
    {
        [ProtoContract]
        public enum VolumeMode
        {
            [XmlEnum(Name = "filesystem")]
            Filesystem,
            [XmlEnum(Name = "block")]
            Block,
        }

        [XmlElement("uid")]
        [ProtoMember(1)]
        public string Uid { get; set; }

        [XmlElement("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [XmlElement("mode")]
        [ProtoMember(3)]
        public VolumeMode Mode { get; set; }

        [XmlElement("path")]
        [ProtoMember(4)]
        public string Path { get; set; }

        [XmlElement("sizeAllocated")]
        [ProtoMember(5)]
        public string SizeAllocated { get; set; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
        [ProtoMember(6)]
        public bool? Persistent { get; set; }
        [XmlElement("persistent")]
        [JsonIgnore]
        public bool NonNullablePersistent
        {
            get
            {
                return Persistent.HasValue && Persistent.Value;
            }
            set
            {
                Persistent = value;
            }
        }
        public bool ShouldSerializeNonNullablePersistent() { return Persistent.HasValue; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
        [ProtoMember(7)]
        public bool? Remote { get; set; }
        [XmlElement("remote")]
        [JsonIgnore]
        public bool NonNullableRemote
        {
            get
            {
                return Remote.HasValue && Remote.Value;
            }
            set
            {
                Remote = value;
            }
        }
        public bool ShouldSerializeNonNullableRemote() { return Remote.HasValue; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(8)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
    }
}