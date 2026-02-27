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
using CycloneDX.Models;

namespace CycloneDX.Core.Models
{
    [ProtoContract]
    public class RelatedCryptoMaterialProperties
    {
        [XmlElement("type")]
        [ProtoMember(1)]
        public RelatedCryptoMaterialType Type { get; set; }

        [XmlElement("id")]
        [ProtoMember(2)]
        public string Id { get; set; }

        [XmlElement("state")]
        [ProtoMember(3)]
        public KeyState State { get; set; }

        [XmlElement("algorithmRef")]
        [ProtoMember(4)]
        public string AlgorithmRef { get; set; }

        private DateTime? _creationDate;
        [XmlElement("creationDate")]
        [ProtoMember(5)]
        public DateTime? CreationDate
        {
            get => _creationDate;
            set { _creationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeCreationDate() { return CreationDate != null; }

        private DateTime? _activationDate;
        [XmlElement("activationDate")]
        [ProtoMember(6)]
        public DateTime? ActivationDate
        {
            get => _activationDate;
            set { _activationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeActivationDate() { return ActivationDate != null; }

        private DateTime? _updateDate;
        [XmlElement("updateDate")]
        [ProtoMember(7)]
        public DateTime? UpdateDate
        {
            get => _updateDate;
            set { _updateDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeUpdateDate() { return UpdateDate != null; }

        private DateTime? _expirationDate;
        [XmlElement("expirationDate")]
        [ProtoMember(8)]
        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set { _expirationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeExpirationDate() { return ExpirationDate != null; }

        [XmlElement("value")]
        [ProtoMember(9)]
        public string Value { get; set; }

        [XmlElement("size")]
        [ProtoMember(10)]
        public int Size { get; set; }

        [XmlElement("format")]
        [ProtoMember(11)]
        public string Format { get; set; }

        [XmlElement("securedBy")]
        [ProtoMember(12)]
        public SecuredBy SecuredBy { get; set; }

        [XmlElement("fingerprint")]
        [ProtoMember(13)]
        public Hash Fingerprint { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(14)]
        public RelatedCryptographicAssetsProto RelatedCryptographicAssetsProto { get; set; }

        [XmlArray("relatedCryptographicAssets")]
        [XmlArrayItem("relatedCryptographicAsset")]
        [JsonPropertyName("relatedCryptographicAssets")]
        public List<RelatedCryptographicAsset> RelatedCryptographicAssets
        {
            get => RelatedCryptographicAssetsProto?.Assets;
            set
            {
                if (value == null) { RelatedCryptographicAssetsProto = null; return; }
                if (RelatedCryptographicAssetsProto == null) { RelatedCryptographicAssetsProto = new RelatedCryptographicAssetsProto(); }
                RelatedCryptographicAssetsProto.Assets = value;
            }
        }
        public bool ShouldSerializeRelatedCryptographicAssets() { return RelatedCryptographicAssets?.Count > 0; }
    }
}
