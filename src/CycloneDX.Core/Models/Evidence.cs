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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("evidence")]
    [ProtoContract]
    public class Evidence
    {
        [XmlIgnore]
        [ProtoMember(1)]
        public List<LicenseChoice> Licenses { get; set; }
        
        [XmlElement("licenses", Order = 3)]
        [JsonIgnore, ProtoIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LicenseChoiceList LicensesSerialized
        {
            get { return Licenses != null ? new LicenseChoiceList(Licenses) : null; }
            set { Licenses = value.Licenses; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLicensesSerialized() { return Licenses?.Count > 0; }

        [XmlArray("copyright", Order = 4)]
        [XmlArrayItem("text")]
        [ProtoMember(2)]
        public List<EvidenceCopyright> Copyright { get; set; }
        
        [XmlElement("identity", Order = 0)]
        [ProtoMember(3)]
        public EvidenceIdentity Identity { get; set; }

        [XmlArray("occurrences", Order = 1)]
        [XmlArrayItem("occurrence")]
        [ProtoMember(4)]
        public List<EvidenceOccurrence> Occurrences { get; set; }

        [XmlElement("callstack", Order = 2)]
        [ProtoMember(5)]
        public Callstack Callstack { get; set; }
    }
}