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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("output")]
    [ProtoContract]
    public class Output
    {
        [ProtoContract]
        public enum OutputType
        {
            [XmlEnum(Name = "artifact")]
            Artifact,
            [XmlEnum(Name = "attestation")]
            Attestation,
            [XmlEnum(Name = "log")]
            Log,
            [XmlEnum(Name = "evidence")]
            Evidence,
            [XmlEnum(Name = "metrics")]
            Metrics,
            [XmlEnum(Name = "other")]
            Other,
        }

        [XmlElement("resource")]
        [ProtoMember(4)]
        public ResourceReferenceChoice Resource { get; set; }

        [XmlElement("type")]
        [ProtoMember(1)]
        public OutputType? Type { get; set; }
        public bool ShouldSerializeType() { return Type != null; }

        [XmlElement("source")]
        [ProtoMember(2)]
        public ResourceReferenceChoice Source { get; set; }

        [XmlElement("target")]
        [ProtoMember(3)]
        public ResourceReferenceChoice Target { get; set; }

        [XmlElement("data")]
        [ProtoMember(5)]
        public AttachedText Data { get; set; }
        
        [XmlElement("environmentVars")]
        [ProtoMember(6)]
        public EnvironmentVarChoices EnvironmentVars { get; set; }
        public bool ShouldSerializeEnvironmentVars() { return EnvironmentVars?.Count > 0; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(7)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
    }
}