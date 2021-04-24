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

using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class Swid
    {
        [XmlAttribute("tagId")]
        [ProtoMember(1)]
        public string TagId { get; set; }

        [XmlAttribute("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [XmlAttribute("version")]
        [ProtoMember(3)]
        public string Version { get; set; }

        [XmlIgnore]
        [ProtoMember(4)]
        public int? TagVersion { get; set; }
        [XmlAttribute("tagVersion")]
        [JsonIgnore]
        public int NonNullableTagVersion
        { 
            get
            {
                return TagVersion.HasValue ? TagVersion.Value : 0;
            }
            set
            {
                TagVersion = value;
            }
        }
        public bool ShouldSerializeNonNullableTagVersion() { return TagVersion.HasValue; }

        [XmlIgnore]
        [ProtoMember(5)]
        public bool? Patch { get; set; }
        [XmlAttribute("patch")]
        [JsonIgnore]
        public bool NonNullablePatch
        { 
            get
            {
                return Patch.HasValue ? Patch.Value : false;
            }
            set
            {
                Patch = value;
            }
        }
        public bool ShouldSerializeNonNullablePatch() { return Patch.HasValue; }

        [XmlElement("text")]
        [ProtoMember(6)]
        public AttachedText Text { get; set; }
        
        [XmlAttribute("url")]
        [ProtoMember(7)]
        public string Url { get; set; }

        public Swid() {}

        public Swid(v1_2.Swid swid)
        {
            TagId = swid.TagId;
            Name = swid.Name;
            Version = swid.Version;
            TagVersion = swid.TagVersion;
            Patch = swid.Patch;
            if (swid.Text != null)
                Text = new AttachedText(swid.Text);
            Url = swid.Url;
        }
    }
}
