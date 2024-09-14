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
using System.Xml;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Attestation : IEquatable<Attestation>
    {
        [XmlElement("summary")]
        [ProtoMember(1)]
        public string Summary { get; set; }

        [XmlElement("assessor")]
        [ProtoMember(2)]
        public string Assessor { get; set; }

        [XmlElement("map")]
        [ProtoMember(3)]
        public List<Map> Map { get; set; }

        [JsonIgnore]
        [XmlAnyElement]
        public List<System.Xml.XmlElement> Any { get; set; }

        [XmlIgnore]
        public SignatureChoice Signature { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Attestation;
            if (other == null)
            {
                return false;
            }

            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(other);
        }

        public bool Equals(Attestation obj)
        {
            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return Json.Serializer.Serialize(this).GetHashCode();
        }

    }
}
