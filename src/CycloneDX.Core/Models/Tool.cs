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
using System.Linq;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [Obsolete("Tool is deprecated and will be removed in a future version")]
    [ProtoContract]
    public class Tool: IEquatable<Tool>
    {
        [XmlElement("vendor")]
        [ProtoMember(1)]
        public string Vendor { get; set; }

        [XmlElement("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(3)]
        public string Version { get; set; }

        [XmlArray("hashes")]
        [ProtoMember(4)]
        public List<Hash> Hashes { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(5)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Tool);
        }

        public bool Equals(Tool obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.ExternalReferences, obj.ExternalReferences) ||
                this.ExternalReferences.SequenceEqual(obj.ExternalReferences)) &&
                (object.ReferenceEquals(this.Hashes, obj.Hashes) ||
                this.Hashes.SequenceEqual(obj.Hashes)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Vendor, obj.Vendor) ||
                this.Vendor.Equals(obj.Vendor, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Version, obj.Version) ||
                this.Version.Equals(obj.Version, StringComparison.InvariantCultureIgnoreCase));
        }
    
        public override int GetHashCode()
        {
            return CycloneDX.Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}