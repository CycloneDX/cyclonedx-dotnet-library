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

using System.Xml.Serialization;

namespace CycloneDX.Models
{
    public class Swid
    {
        [XmlAttribute("tagId")]
        public string TagId { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("version")]
        public string Version { get; set; }
        [XmlAttribute("tagVersion")]
        public string TagVersion { get; set; }
        [XmlAttribute("patch")]
        public string Patch { get; set; }
        [XmlElement("text")]
        public AttachedText Text { get; set; }
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
