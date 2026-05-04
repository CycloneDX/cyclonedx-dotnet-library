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
using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class PriorityApplication
    {
        [XmlElement("applicationNumber")]
        [ProtoMember(1)]
        public string ApplicationNumber { get; set; }

        [XmlElement("jurisdiction")]
        [ProtoMember(2)]
        public string Jurisdiction { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(3)]
        public DateTime? FilingDateProto { get; set; }

        [XmlElement("filingDate")]
        [JsonPropertyName("filingDate")]
        public string FilingDate
        {
            get
            {
                if (FilingDateProto.HasValue)
                {
                    return FilingDateProto.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                return _filingDate;
            }
            set
            {
                _filingDate = value;
                if (value != null && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                {
                    FilingDateProto = dt;
                }
                else
                {
                    FilingDateProto = null;
                }
            }
        }
        private string _filingDate;
        public bool ShouldSerializeFilingDate() { return FilingDate != null; }
    }
}
