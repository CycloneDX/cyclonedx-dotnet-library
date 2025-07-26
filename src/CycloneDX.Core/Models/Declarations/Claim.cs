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

using CycloneDX.Core.Models;
using CycloneDX.Models.Vulnerabilities;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Claim : IEquatable<Claim>, IHasBomRef
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("target")]
        [ProtoMember(2)]
        public string Target { get; set; }

        [XmlElement("predicate")]
        [ProtoMember(3)]
        public string Predicate { get; set; }

        [XmlArray("mitigationStrategies")]
        [XmlArrayItem("mitigationStrategy")]
        [ProtoMember(4)]
        public List<string> MitigationStrategies { get; set; }

        [XmlElement("reasoning")]
        [ProtoMember(5)]
        public string Reasoning { get; set; }

        [XmlElement("evidence")]
        [ProtoMember(6)]
        public List<string> Evidence { get; set; }

        [XmlElement("counterEvidence")]
        [ProtoMember(7)]
        public List<string> CounterEvidence { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(8)]
        public List<ExternalReference> ExternalReferences { get; set; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }

        [XmlIgnore]
        public SignatureChoice Signature { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Claim;
            if (other == null)
            {
                return false;
            }

            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(other, Json.Serializer.SerializerOptionsForHash);
        }

        public bool Equals(Claim obj)
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(obj, Json.Serializer.SerializerOptionsForHash);
        }

        public override int GetHashCode()
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash).GetHashCode();
        }
    }
}
