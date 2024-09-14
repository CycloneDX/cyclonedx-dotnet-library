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
using ProtoBuf;
using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Assessor : IEquatable<Assessor>, IHasBomRef
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("thirdParty")]
        [ProtoMember(2)]
        public bool ThirdParty { get; set; }

        [XmlElement("organization")]
        [ProtoMember(3)]
        public OrganizationalEntity Organization { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Assessor;
            if (other == null)
            {
                return false;
            }

            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(other);
        }

        public bool Equals(Assessor obj)
        {
            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}
