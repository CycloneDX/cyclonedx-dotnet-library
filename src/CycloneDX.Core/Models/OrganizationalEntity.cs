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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class OrganizationalEntity : IEquatable<OrganizationalEntity>, IHasBomRef
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("url")]
        [ProtoMember(2)]
        public List<string> Url { get; set; }

        [XmlElement("contact")]
        [ProtoMember(3)]
        public List<OrganizationalContact> Contact { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(4)]
        public string BomRef { get; set; }

        [XmlElement("address")]
        [ProtoMember(5)]
        public PostalAddress Address { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OrganizationalEntity;
            if (other == null)
            {
                return false;
            }

            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(other);
        }

        public bool Equals(OrganizationalEntity obj)
        {
            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}
