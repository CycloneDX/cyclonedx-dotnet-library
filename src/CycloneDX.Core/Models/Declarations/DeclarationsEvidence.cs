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
    public class DeclarationsEvidence
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }
        [XmlElement("propertyName")]
        [ProtoMember(2)]
        public string PropertyName { get; set; }
        [XmlElement("description")]
        [ProtoMember(3)]
        public string Description { get; set; }

        [XmlElement("data")]
        [ProtoMember(4)]
        public List<DeclarationData> Data { get; set; }

        private DateTime? _created;

        [XmlElement("created")]
        [ProtoMember(5)]
        public DateTime? Created
        {
            get => _created;
            set { _created = BomUtils.UtcifyDateTime(value); }
        }

        private DateTime? _expires;

        [XmlElement("expires")]
        [ProtoMember(6)]
        public DateTime? Expires
        {
            get => _expires;
            set { _expires = BomUtils.UtcifyDateTime(value); }
        }
        [XmlElement("author")]
        [ProtoMember(7)]
        public OrganizationalContact Author { get; set; }
        [XmlElement("reviewer")]
        [ProtoMember(8)]
        public OrganizationalContact Reviewer { get; set; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }
        [XmlIgnore]
        public Signature Signature { get; set; }

    }
}
