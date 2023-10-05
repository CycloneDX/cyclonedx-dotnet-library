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

using static CycloneDX.SpecificationVersion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.Vulnerabilities
{
    [ProtoContract]
    public class Affects : BomEntity, IBomEntityWithRefLinkType_String_Ref
    {
        [XmlElement("ref")]
        [ProtoMember(1)]
        public string Ref { get; set; }

        [XmlArray("versions")]
        [XmlArrayItem("version")]
        [JsonPropertyName("versions")]
        [ProtoMember(2)]
        public List<AffectedVersions> Versions { get; set; }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_ComponentOrService =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(Affects).GetProperty("Ref", typeof(string)), RefLinkConstraints_ComponentOrService }
        }.ToImmutableDictionary();

        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            switch (specificationVersion)
            {
                case v1_0:
                case v1_1:
                case v1_2:
                case v1_3:
                    return null;

                case v1_4:
                case v1_5:
                default:
                    return RefLinkConstraints_StringRef_ComponentOrService;
            }
        }
    }
}
