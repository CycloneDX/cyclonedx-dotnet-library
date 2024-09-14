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

namespace CycloneDX.Core.Models
{
    [ProtoContract]
    public class Ikev2TransformTypes
    {
        [XmlElement("encr")]
        [JsonPropertyName("encr")]
        [ProtoMember(1)]
        public List<string> EncryptionAlgorithms { get; set; }

        [XmlElement("prf")]
        [JsonPropertyName("prf")]
        [ProtoMember(2)]
        public List<string> PseudorandomFunctions { get; set; }

        [XmlElement("integ")]
        [JsonPropertyName("integ")]
        [ProtoMember(3)]
        public List<string> IntegrityAlgorithms { get; set; }

        [XmlElement("ke")]
        [JsonPropertyName("ke")]
        [ProtoMember(4)]
        public List<string> KeyExchangeMethods { get; set; }

        [XmlElement("esn")]
        [JsonPropertyName("esn")]
        [ProtoMember(5)]
        public List<bool> ExtendedSequenceNumbers { get; set; }

        [XmlElement("auth")]
        [JsonPropertyName("auth")]
        [ProtoMember(6)]
        public List<string> AuthenticationMethods { get; set; }
    }


}
