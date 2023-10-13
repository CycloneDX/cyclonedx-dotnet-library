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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("evidence-identity")]
    [ProtoContract]
    public class EvidenceIdentity : BomEntity, IBomEntityWithRefLinkType_StringList
    {
        [ProtoContract]
        public enum EvidenceFieldType
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "group")]
            Group,
            [XmlEnum(Name = "name")]
            Name,
            [XmlEnum(Name = "version")]
            Version,
            [XmlEnum(Name = "purl")]
            Purl,
            [XmlEnum(Name = "cpe")]
            Cpe,
            [XmlEnum(Name = "swid")]
            Swid,
            [XmlEnum(Name = "hash")]
            Hash,
        }

        [XmlElement("field")]
        [ProtoMember(1)]
        public EvidenceFieldType Field { get; set; }

        [XmlElement("confidence")]
        [ProtoMember(2)]
        public float? Confidence { get; set; }
        
        [XmlArray("methods")]
        [XmlArrayItem("method")]
        [ProtoMember(3)]
        public List<EvidenceMethods> Methods { get; set; }

        [XmlElement("tools")]
        [ProtoMember(4)]
        public EvidenceTools Tools { get; set; }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_List_AnyBomEntity =
        new Dictionary<PropertyInfo, ImmutableList<Type>>
        {
            // EvidenceTools is a List<string> as of CDX spec 1.5
            { typeof(EvidenceIdentity).GetProperty("Tools", typeof(EvidenceTools)), RefLinkConstraints_AnyBomEntity }
        }.ToImmutableDictionary();

        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            // TODO: switch/case for CDX spec newer than 1.5 where this type got introduced
            if (specificationVersion == v1_5)
            {
                return RefLinkConstraints_List_AnyBomEntity;
            }
            return null;
        }
    }
}