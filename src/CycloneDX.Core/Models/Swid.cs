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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Swid : IEquatable<Swid>
    {
        [XmlAttribute("tagId")]
        [ProtoMember(1)]
        public string TagId { get; set; }

        [XmlAttribute("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [XmlAttribute("version")]
        [ProtoMember(3)]
        public string Version { get; set; } = "0.0";

        [XmlAttribute("tagVersion")]
        [ProtoMember(4)]
        public int TagVersion { get; set; }

        [XmlAttribute("patch")]
        [ProtoMember(5)]
        public bool Patch { get; set; }

        [XmlElement("text")]
        [ProtoMember(6)]
        public AttachedText Text { get; set; }
        
        [XmlAttribute("url")]
        [ProtoMember(7)]
        public string Url { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Swid);
        }

        public bool Equals(Swid obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (this.Patch.Equals(obj.Patch)) &&
                (object.ReferenceEquals(this.TagId, obj.TagId) ||
                this.TagId.Equals(obj.TagId, StringComparison.InvariantCultureIgnoreCase)) &&
                (this.TagVersion.Equals(obj.TagVersion)) &&
                (object.ReferenceEquals(this.Text, obj.Text) ||
                this.Text.Equals(obj.Text)) &&
                (object.ReferenceEquals(this.Url, obj.Url) ||
                this.Url.Equals(obj.Url, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Version, obj.Version) ||
                this.Version.Equals(obj.Version, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
