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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Models.v1_2
{
    public class Swid
    {
        [XmlAttribute("tagId")]
        public string TagId { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlIgnore]
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
        public AttachedText Text { get; set; }
        
        [XmlAttribute("url")]
        public string Url { get; set; }

        public Swid() {}

        public Swid(v1_3.Swid swid)
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
