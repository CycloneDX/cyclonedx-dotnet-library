// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class Patch
    {
        [ProtoContract]
        public enum PatchClassification
        {
            [XmlEnum(Name = "unofficial")]
            Unofficial,
            [XmlEnum(Name = "monkey")]
            Monkey,
            [XmlEnum(Name = "backport")]
            Backport,
            [XmlEnum(Name = "cherry-pick")]
            CherryPick
        }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public PatchClassification Type { get; set; }

        [XmlElement("diff")]
        [ProtoMember(2)]
        public Diff Diff { get; set; }

        [XmlArray("resolves")]        
        [XmlArrayItem("issue")]        
        [ProtoMember(3)]
        public List<Issue> Resolves { get; set; }

        public Patch() {}

        public Patch(v1_2.Patch patch)
        {
            Type = (PatchClassification)patch.Type;
            if (patch.Diff != null)
                Diff = new Diff(patch.Diff);
            if (patch.Resolves != null)
            {
                Resolves = new List<Issue>();
                foreach (var issue in patch.Resolves)
                {
                    Resolves.Add(new Issue(issue));
                }
            }
        }
    }
}
