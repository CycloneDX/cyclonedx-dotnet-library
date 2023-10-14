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

namespace CycloneDX.Models
{
    [ProtoContract]
    public class ReleaseNotes : BomEntity
    {
        [XmlElement("type")]
        [ProtoMember(1)]
        public string Type { get; set; }
        
        [XmlElement("title")]
        [ProtoMember(2)]
        public string Title { get; set; }
        
        [XmlElement("featuredImage")]
        [ProtoMember(3)]
        public string FeaturedImage { get; set; }
        
        [XmlElement("socialImage")]
        [ProtoMember(4)]
        public string SocialImage { get; set; }
        
        [XmlElement("description")]
        [ProtoMember(5)]
        public string Description { get; set; }
        
        private DateTime? _timestamp;
        [XmlElement("timestamp")]
        [ProtoMember(6)]
        public DateTime? Timestamp
        {
            get => _timestamp;
            set { _timestamp = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }

        [XmlArray("aliases")]
        [XmlArrayItem("alias")]
        [ProtoMember(7)]
        public List<string> Aliases { get; set; }
        
        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        [ProtoMember(8)]
        public List<string> Tags { get; set; }
        
        [XmlArray("resolves")]
        [XmlArrayItem("issue")]
        [ProtoMember(9)]
        public List<Issue> Resolves { get; set; }

        [XmlArray("notes")]
        [XmlArrayItem("note")]
        [ProtoMember(10)]
        public List<Note> Notes { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(11)]
        public List<Property> Properties { get; set; }
    }
}
