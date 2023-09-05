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
    public class Lifecycles
    {
        [ProtoContract]
        public enum LifecyclePhase
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "design")]
            Design,
            [XmlEnum(Name = "pre-build")]
            Pre_Build,
            [XmlEnum(Name = "build")]
            Build,
            [XmlEnum(Name = "post-build")]
            Post_Build,
            [XmlEnum(Name = "operations")]
            Operations,
            [XmlEnum(Name = "discovery")]
            Discovery,
            [XmlEnum(Name = "decommission")]
            Decommission,
        }

        [XmlElement("phase")]
        [ProtoMember(1)]
        [JsonIgnore]
        public LifecyclePhase Phase { get; set; }
        public bool ShouldSerializePhase() => Phase != LifecyclePhase.Null;

        [XmlIgnore]
        [JsonPropertyName("phase")]
        public LifecyclePhase? JsonPhase
        {
            get => Phase == LifecyclePhase.Null ? (LifecyclePhase?)null : Phase;
            set => Phase = (value == null ? LifecyclePhase.Null : value.Value);
        }

        [XmlElement("name")]
        [ProtoMember(2)]
        public string Name { get; set; }
        
        [XmlElement("description")]
        [ProtoMember(3)]
        public string Description { get; set; }
    }
}
