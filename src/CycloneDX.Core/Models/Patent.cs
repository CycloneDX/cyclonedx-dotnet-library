// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Patent
    {
        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("patentNumber")]
        [ProtoMember(2)]
        public string PatentNumber { get; set; }

        [XmlElement("applicationNumber")]
        [ProtoMember(3)]
        public string ApplicationNumber { get; set; }

        [XmlElement("jurisdiction")]
        [ProtoMember(4)]
        public string Jurisdiction { get; set; }

        [XmlElement("priorityApplication")]
        [ProtoMember(5)]
        public PriorityApplication PriorityApplication { get; set; }

        [XmlElement("publicationNumber")]
        [ProtoMember(6)]
        public string PublicationNumber { get; set; }

        [XmlElement("title")]
        [ProtoMember(7)]
        public string Title { get; set; }

        [XmlElement("abstract")]
        [ProtoMember(8)]
        public string Abstract { get; set; }

        // Proto uses google.protobuf.Timestamp for dates, but XML/JSON use date-only strings.
        // We use DateTime? for protobuf and expose string properties for XML/JSON.

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(9)]
        public DateTime? FilingDateProto { get; set; }

        [XmlElement("filingDate")]
        [JsonPropertyName("filingDate")]
        public string FilingDate
        {
            get
            {
                if (FilingDateProto.HasValue)
                    return FilingDateProto.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                return _filingDate;
            }
            set
            {
                _filingDate = value;
                if (value != null && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                    FilingDateProto = dt;
                else
                    FilingDateProto = null;
            }
        }
        private string _filingDate;
        public bool ShouldSerializeFilingDate() { return FilingDate != null; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(10)]
        public DateTime? GrantDateProto { get; set; }

        [XmlElement("grantDate")]
        [JsonPropertyName("grantDate")]
        public string GrantDate
        {
            get
            {
                if (GrantDateProto.HasValue)
                    return GrantDateProto.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                return _grantDate;
            }
            set
            {
                _grantDate = value;
                if (value != null && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                    GrantDateProto = dt;
                else
                    GrantDateProto = null;
            }
        }
        private string _grantDate;
        public bool ShouldSerializeGrantDate() { return GrantDate != null; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(11)]
        public DateTime? PatentExpirationDateProto { get; set; }

        [XmlElement("patentExpirationDate")]
        [JsonPropertyName("patentExpirationDate")]
        public string PatentExpirationDate
        {
            get
            {
                if (PatentExpirationDateProto.HasValue)
                    return PatentExpirationDateProto.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                return _patentExpirationDate;
            }
            set
            {
                _patentExpirationDate = value;
                if (value != null && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                    PatentExpirationDateProto = dt;
                else
                    PatentExpirationDateProto = null;
            }
        }
        private string _patentExpirationDate;
        public bool ShouldSerializePatentExpirationDate() { return PatentExpirationDate != null; }

        [XmlElement("patentLegalStatus")]
        [ProtoMember(12)]
        public PatentLegalStatus PatentLegalStatus { get; set; }

        [XmlElement("patentAssignee")]
        [ProtoMember(13)]
        [System.Text.Json.Serialization.JsonConverter(typeof(CycloneDX.Json.Converters.PatentAssigneeConverter))]
        public List<OrganizationalEntityOrContact> PatentAssignee { get; set; }
        public bool ShouldSerializePatentAssignee() { return PatentAssignee?.Count > 0; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(14)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }
    }
}
