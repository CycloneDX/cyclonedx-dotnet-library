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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class CitationPointers
    {
        [ProtoMember(1)]
        public List<string> Pointer { get; set; }
    }

    [ProtoContract]
    public class CitationExpressions
    {
        [ProtoMember(1)]
        public List<string> Expression { get; set; }
    }

    [ProtoContract]
    public class Citation
    {
        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(2)]
        public CitationPointers PointersProto { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(3)]
        public CitationExpressions ExpressionsProto { get; set; }

        [XmlArray("pointers")]
        [XmlArrayItem("pointer")]
        [JsonPropertyName("pointers")]
        public List<string> Pointers
        {
            get => PointersProto?.Pointer;
            set
            {
                if (value == null) { PointersProto = null; return; }
                if (PointersProto == null) PointersProto = new CitationPointers();
                PointersProto.Pointer = value;
            }
        }
        public bool ShouldSerializePointers() { return Pointers?.Count > 0; }

        [XmlArray("expressions")]
        [XmlArrayItem("expression")]
        [JsonPropertyName("expressions")]
        public List<string> Expressions
        {
            get => ExpressionsProto?.Expression;
            set
            {
                if (value == null) { ExpressionsProto = null; return; }
                if (ExpressionsProto == null) ExpressionsProto = new CitationExpressions();
                ExpressionsProto.Expression = value;
            }
        }
        public bool ShouldSerializeExpressions() { return Expressions?.Count > 0; }

        private DateTime? _timestamp;
        [XmlElement("timestamp")]
        [ProtoMember(4)]
        public DateTime? Timestamp
        {
            get => _timestamp;
            set { _timestamp = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }

        [XmlElement("attributedTo")]
        [ProtoMember(5)]
        public string AttributedTo { get; set; }

        [XmlElement("process")]
        [ProtoMember(6)]
        public string Process { get; set; }

        [XmlElement("note")]
        [ProtoMember(7)]
        public string Note { get; set; }
    }
}
