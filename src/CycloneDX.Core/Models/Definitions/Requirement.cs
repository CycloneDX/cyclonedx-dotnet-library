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

using ProtoBuf;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Requirement
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("identifier")]
        [ProtoMember(2)]
        public string Identifier { get; set; }

        [XmlElement("title")]
        [ProtoMember(3)]
        public string Title { get; set; }

        [XmlElement("text")]
        [ProtoMember(4)]
        public string Text { get; set; }

        [XmlArray("descriptions")]
        [XmlArrayItem("description")]
        [ProtoMember(5)]
        //This is maxOccurs="1" in xsd
        public List<string> Descriptions { get; set; }
        public bool ShouldSerializeDescriptions() => Descriptions?.Count > 0;


        [XmlElement("openCre")]
        [ProtoMember(6)]
        public List<string> OpenCre { get; set; }

        [XmlElement("parent")]
        [ProtoMember(7)]
        public string Parent { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(8)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() => Properties?.Count > 0;

        [XmlArray("externalReferences")]
        [XmlArrayItem("externalReference")]
        [ProtoMember(9)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() => ExternalReferences?.Count > 0;


    }
}
