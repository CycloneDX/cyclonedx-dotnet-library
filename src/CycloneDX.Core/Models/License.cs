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

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("license")]
    [ProtoContract]
    public class License : BomEntity, IBomEntityWithRefType_String_BomRef
    {
        [XmlElement("id")]
        [ProtoMember(1)]
        public string Id { get; set; }
        public bool ShouldSerializeId() { return Id != null; }

        [XmlElement("name")]
        [ProtoMember(2)]
        public string Name { get; set; }
        public bool ShouldSerializeName() { return string.IsNullOrEmpty(Id); }

        [XmlElement("text")]
        [ProtoMember(3)]
        public AttachedText Text { get; set; }
        
        [XmlElement("url")]
        [ProtoMember(4)]
        public string Url { get; set; }
    
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(5)]
        public string BomRef { get; set; }
        
        [XmlElement("licensing")]
        [ProtoMember(6)]
        public Licensing Licensing { get; set; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(7)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
    }
}
