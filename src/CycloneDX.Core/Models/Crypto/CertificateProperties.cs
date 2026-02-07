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
    public class CertificateProperties
    {
        [XmlElement("serialNumber")]
        [ProtoMember(9)]
        public string SerialNumber { get; set; }

        [XmlElement("subjectName")]
        [ProtoMember(1)]
        public string SubjectName { get; set; }

        [XmlElement("issuerName")]
        [ProtoMember(2)]
        public string IssuerName { get; set; }

        [XmlElement("notValidBefore")]
        [ProtoMember(3)]
        public DateTime NotValidBefore { get; set; }

        [XmlElement("notValidAfter")]
        [ProtoMember(4)]
        public DateTime NotValidAfter { get; set; }

        [XmlElement("signatureAlgorithmRef")]
        [ProtoMember(5)]
        public string SignatureAlgorithmRef { get; set; }

        [XmlElement("subjectPublicKeyRef")]
        [ProtoMember(6)]
        public string SubjectPublicKeyRef { get; set; }

        [XmlElement("certificateFormat")]
        [ProtoMember(7)]
        public string CertificateFormat { get; set; }

        [System.Obsolete("Use CertificateFileExtension instead.")]
        [XmlElement("certificateExtension")]
        [ProtoMember(8)]
        public string CertificateExtension { get; set; }

        [XmlElement("certificateFileExtension")]
        [ProtoMember(10)]
        public string CertificateFileExtension { get; set; }

        [XmlElement("fingerprint")]
        [ProtoMember(11)]
        public Hash Fingerprint { get; set; }

        [XmlElement("certificateState")]
        [ProtoMember(12)]
        public List<CertificateState> CertificateStates { get; set; }
        public bool ShouldSerializeCertificateStates() { return CertificateStates?.Count > 0; }

        private DateTime? _creationDate;
        [XmlElement("creationDate")]
        [ProtoMember(13)]
        public DateTime? CreationDate
        {
            get => _creationDate;
            set { _creationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeCreationDate() { return CreationDate.HasValue; }

        private DateTime? _activationDate;
        [XmlElement("activationDate")]
        [ProtoMember(14)]
        public DateTime? ActivationDate
        {
            get => _activationDate;
            set { _activationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeActivationDate() { return ActivationDate.HasValue; }

        private DateTime? _deactivationDate;
        [XmlElement("deactivationDate")]
        [ProtoMember(15)]
        public DateTime? DeactivationDate
        {
            get => _deactivationDate;
            set { _deactivationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeDeactivationDate() { return DeactivationDate.HasValue; }

        private DateTime? _revocationDate;
        [XmlElement("revocationDate")]
        [ProtoMember(16)]
        public DateTime? RevocationDate
        {
            get => _revocationDate;
            set { _revocationDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeRevocationDate() { return RevocationDate.HasValue; }

        private DateTime? _destructionDate;
        [XmlElement("destructionDate")]
        [ProtoMember(17)]
        public DateTime? DestructionDate
        {
            get => _destructionDate;
            set { _destructionDate = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeDestructionDate() { return DestructionDate.HasValue; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(18)]
        public CertificateExtensionsProto CertificateExtensionsProto { get; set; }

        [XmlArray("certificateExtensions")]
        [XmlArrayItem("certificateExtension")]
        [JsonPropertyName("certificateExtensions")]
        public List<CertificateExtensionV2> CertificateExtensions
        {
            get => CertificateExtensionsProto?.Extensions;
            set
            {
                if (value == null) { CertificateExtensionsProto = null; return; }
                if (CertificateExtensionsProto == null) { CertificateExtensionsProto = new CertificateExtensionsProto(); }
                CertificateExtensionsProto.Extensions = value;
            }
        }
        public bool ShouldSerializeCertificateExtensions() { return CertificateExtensions?.Count > 0; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(19)]
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
