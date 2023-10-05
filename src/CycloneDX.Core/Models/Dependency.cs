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
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("dependency")]
    [ProtoContract]
    public class Dependency : BomEntity, IBomEntityWithRefLinkType_String_Ref
    {
        [XmlAttribute("ref")]
        [ProtoMember(1)]
        public string Ref { get; set; }

        [XmlElement("dependency")]
        [ProtoMember(2)]
        public List<Dependency> Dependencies { get; set; }

        /// <summary>
        /// See BomEntity.NormalizeList() and ListMergeHelper.SortByImpl().
        /// Note that as a static method this is not exactly an "override",
        /// but the BomEntity base class implementation makes it behave
        /// like that in practice.
        /// </summary>
        /// <param name="ascending">Ascending (true) or Descending (false)</param>
        /// <param name="recursive">Passed to BomEntity.NormalizeList() (effective if recursing), not handled right here</param>
        /// <param name="list">List<Dependency> to sort</param>
        public static void NormalizeList(bool ascending, bool recursive, List<Dependency> list)
        {
            var sortHelper = new ListMergeHelper<Dependency>();
            sortHelper.SortByImpl(ascending, recursive, list,
                o => (o?.Ref),
                null);
        }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_Component =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(Dependency).GetProperty("Ref", typeof(string)), RefLinkConstraints_Component }
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_ComponentOrService =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(Dependency).GetProperty("Ref", typeof(string)), RefLinkConstraints_ComponentOrService }
        }.ToImmutableDictionary();

        /// <summary>
        /// See IBomEntityWithRefLinkType.GetRefLinkConstraints().
        /// </summary>
        /// <param name="specificationVersion"></param>
        /// <returns></returns>
        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            switch (specificationVersion)
            {
                case v1_0:
                case v1_1:
                    return null;

                case v1_2:
                case v1_3:
                case v1_4:
                    // NOTE: XML and JSON schema descriptions differ:
                    // * in JSON, specs v1.2, 1.3 and 1.4 dealt with "components"
                    // * in XML since 1.2, and in JSON since 1.5, with "components or services"
                    //TOTHINK//return RefLinkConstraints_StringRef_Component?..

                case v1_5:
                default:
                    return RefLinkConstraints_StringRef_ComponentOrService;
            }
        }
    }
}
