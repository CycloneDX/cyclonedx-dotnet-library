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
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class DatasetChoice : BomEntity, IBomEntityWithRefLinkType_String_Ref
    {
        [XmlElement("dataset")]
        [ProtoMember(1)]
        public Data DataSet { get; set; }

        [XmlElement("ref")]
        [ProtoMember(2)]
        public string Ref { get; set; }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_ModelDataset =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(DatasetChoice).GetProperty("Ref", typeof(string)), RefLinkConstraints_ModelDataset }
        }.ToImmutableDictionary();

        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            // TODO: switch/case for CDX spec newer than 1.5 where this type got introduced
            if (specificationVersion == v1_5)
            {
                return RefLinkConstraints_StringRef_ModelDataset;
            }
            return null;
        }
    }
}
