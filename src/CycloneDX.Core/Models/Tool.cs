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

        public bool Equals(Tool obj)
        {
            return CycloneDX.Json.Serializer.Serialize(this) == CycloneDX.Json.Serializer.Serialize(obj);
        }
    
        public override int GetHashCode()
        {
            return CycloneDX.Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}