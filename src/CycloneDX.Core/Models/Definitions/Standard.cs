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
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Standard : IEquatable<Standard>, IHasBomRef
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(3)]
        public string Version { get; set; }

        [XmlElement("description")]
        [ProtoMember(4)]
        public string Description { get; set; }

        [XmlElement("owner")]
        [ProtoMember(5)]
        public string Owner { get; set; }

        [XmlArray("requirements")]
        [XmlArrayItem("requirement")]
        [ProtoMember(6)]
        public List<Requirement> Requirements { get; set; }

        [XmlArray("levels")]
        [XmlArrayItem("level")]
        [ProtoMember(7)]
        public List<Level> Levels { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("externalReference")]
        [ProtoMember(8)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() => ExternalReferences?.Count > 0;



        [XmlAnyElement]
        public List<System.Xml.XmlElement> Any { get; set; }

        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] AnyAttr { get; set; }

        [XmlIgnore]
        public SignatureChoice Signature { get; set; }




        public override bool Equals(object obj)
        {
            var other = obj as Standard;
            if (other == null)
            {
                return false;
            }

            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(other);
        }

        public bool Equals(Standard obj)
        {
            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}
