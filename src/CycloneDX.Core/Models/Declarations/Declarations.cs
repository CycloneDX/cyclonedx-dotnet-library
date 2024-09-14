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
using System.Xml;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Declarations
    {
        [XmlArray("assessors")]
        [XmlArrayItem("assessor")]
        [ProtoMember(1)]
        public List<Assessor> Assessors { get; set; }

        [XmlArray("attestations")]
        [XmlArrayItem("attestation")]
        [ProtoMember(2)]
        public List<Attestation> Attestations { get; set; }

        [XmlArray("claims")]
        [XmlArrayItem("claim")]

        [ProtoMember(3)]
        public List<Claim> Claims { get; set; }

        [XmlArray("evidence")]
        [XmlArrayItem("evidence")]
        [ProtoMember(4)]
        public List<DeclarationsEvidence> Evidence { get; set; }
        [XmlElement("targets")]
        [ProtoMember(5)]
        public Targets Targets { get; set; }
        [XmlElement("affirmation")]

        [ProtoMember(6)]
        public Affirmation Affirmation { get; set; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }
        [XmlIgnore]
        public SignatureChoice Signature { get; set; }

    }


}
