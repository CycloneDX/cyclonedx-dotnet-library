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
    }
}
