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
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    [ProtoContract]
    public class CertificateProperties
    {
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
        [XmlElement("certificateExtension")]
        [ProtoMember(8)]
        public string CertificateExtension { get; set; }
    }


}
