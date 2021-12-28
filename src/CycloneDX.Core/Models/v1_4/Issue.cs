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

using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_4
{
    [ProtoContract]
    public class Issue
    {
        [ProtoContract]
        public enum IssueClassification
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "defect")]
            Defect,
            [XmlEnum(Name = "enhancement")]
            Enhancement,
            [XmlEnum(Name = "security")]
            Security
        }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public IssueClassification Type { get; set; }

        [XmlElement("id")]
        [ProtoMember(2)]
        public string Id { get; set; }

        [XmlElement("name")]
        [ProtoMember(3)]
        public string Name { get; set; }

        [XmlElement("description")]
        [ProtoMember(4)]
        public string Description { get; set; }

        [XmlElement("source")]
        [ProtoMember(5)]
        public Source Source { get; set; }

        [XmlArray("references")]
        [XmlArrayItem("url")]
        [ProtoMember(6)]
        public List<string> References { get; set; }

        public Issue() {}

        public Issue(v1_3.Issue issue)
        {
            Type = (IssueClassification)issue.Type;
            Id = issue.Id;
            Name = issue.Name;
            Description = issue.Description;
            if (issue.Source != null)
            {
                Source = new Source(issue.Source);
            }
            References = issue.References;
        }
    }
}
