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
    public class Ikev2Encr
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("keyLength")]
        [ProtoMember(2)]
        public int? KeyLength { get; set; }
        public bool ShouldSerializeKeyLength() { return KeyLength.HasValue; }

        [XmlElement("algorithm")]
        [ProtoMember(3)]
        public string Algorithm { get; set; }
    }

    [ProtoContract]
    public class Ikev2Prf
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("algorithm")]
        [ProtoMember(2)]
        public string Algorithm { get; set; }
    }

    [ProtoContract]
    public class Ikev2Integ
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("algorithm")]
        [ProtoMember(2)]
        public string Algorithm { get; set; }
    }

    [ProtoContract]
    public class Ikev2Ke
    {
        [XmlElement("group")]
        [ProtoMember(1)]
        public int? Group { get; set; }
        public bool ShouldSerializeGroup() { return Group.HasValue; }

        [XmlElement("algorithm")]
        [ProtoMember(2)]
        public string Algorithm { get; set; }
    }

    [ProtoContract]
    public class Ikev2Auth
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("algorithm")]
        [ProtoMember(2)]
        public string Algorithm { get; set; }
    }

    [ProtoContract]
    public class Ikev2TransformTypes
    {
        [XmlIgnore]
        [JsonPropertyName("encr")]
        [ProtoMember(1)]
        public List<Ikev2Encr> EncryptionAlgorithmsDetailed { get; set; }

        [XmlIgnore]
        [JsonPropertyName("prf")]
        [ProtoMember(2)]
        public List<Ikev2Prf> PseudorandomFunctionsDetailed { get; set; }

        [XmlIgnore]
        [JsonPropertyName("integ")]
        [ProtoMember(3)]
        public List<Ikev2Integ> IntegrityAlgorithmsDetailed { get; set; }

        [XmlIgnore]
        [JsonPropertyName("ke")]
        [ProtoMember(4)]
        public List<Ikev2Ke> KeyExchangeMethodsDetailed { get; set; }

        [XmlElement("esn")]
        [JsonPropertyName("esn")]
        [ProtoMember(5)]
        public bool? ExtendedSequenceNumbers { get; set; }
        public bool ShouldSerializeExtendedSequenceNumbers() { return ExtendedSequenceNumbers.HasValue; }

        [XmlIgnore]
        [JsonPropertyName("auth")]
        [ProtoMember(6)]
        public List<Ikev2Auth> AuthenticationMethodsDetailed { get; set; }
    }

    /// <summary>
    /// DEPRECATED - This is the old format for IKEv2 transform types using simple string refs.
    /// Kept for backwards compatibility with protobuf field 4 on ProtocolProperties.
    /// </summary>
    [ProtoContract]
    public class Ikev2TransformTypesLegacy
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
        public bool? ExtendedSequenceNumbers { get; set; }
        public bool ShouldSerializeExtendedSequenceNumbers() { return ExtendedSequenceNumbers.HasValue; }

        [XmlElement("auth")]
        [JsonPropertyName("auth")]
        [ProtoMember(6)]
        public List<string> AuthenticationMethods { get; set; }
    }
}
