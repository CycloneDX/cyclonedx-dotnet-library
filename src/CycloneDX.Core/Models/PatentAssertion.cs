// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class PatentAssertion : IEquatable<PatentAssertion>
#if NET8_0_OR_GREATER
        , IMergeable<PatentAssertion>, IEquivalent<PatentAssertion>
#endif
    {
        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("assertionType")]
        [ProtoMember(2)]
        public PatentAssertionType AssertionType { get; set; }

        [XmlArray("patentRefs")]
        [XmlArrayItem("bom-ref")]
        [ProtoMember(3)]
        public List<string> PatentRefs { get; set; }
        public bool ShouldSerializePatentRefs() { return PatentRefs?.Count > 0; }

        [XmlElement("asserter")]
        [ProtoMember(4)]
        public Asserter Asserter { get; set; }

        [XmlElement("notes")]
        [ProtoMember(5)]
        public string Notes { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as PatentAssertion;
            if (other == null)
            {
                return false;
            }

            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(other, Json.Serializer.SerializerOptionsForHash);
        }

        public bool Equals(PatentAssertion obj)
        {
            return obj != null && JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(obj, Json.Serializer.SerializerOptionsForHash);
        }

        public override int GetHashCode()
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash).GetHashCode();
        }
    }
}
