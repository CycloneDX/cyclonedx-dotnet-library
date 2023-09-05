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
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.Vulnerabilities
{
    [ProtoContract]
    public class Analysis
    {
        [XmlElement("state")]
        [ProtoMember(1)]
        public ImpactAnalysisState State { get; set; }

        [XmlElement("justification")]
        [ProtoMember(2)]
        public ImpactAnalysisJustification Justification { get; set; }

        [XmlArray("responses")]
        [XmlArrayItem("response")]
        [ProtoMember(3)]
        public List<Response> Response { get; set; }

        [XmlElement("detail")]
        [ProtoMember(4)]
        public string Detail { get; set; }

        private DateTime? _firstIssued;
        [XmlElement("firstIssued")]
        [ProtoMember(5)]
        public DateTime? FirstIssued
        { 
            get => _firstIssued;
            set { _firstIssued = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeFirstIssued() { return FirstIssued != null; }

        private DateTime? _lastUpdated;
        [XmlElement("lastUpdated")]
        [ProtoMember(6)]
        public DateTime? LastUpdated
        { 
            get => _lastUpdated;
            set { _lastUpdated = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeLastUpdated() { return LastUpdated != null; }
    }
}
