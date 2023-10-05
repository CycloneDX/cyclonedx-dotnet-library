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
using System.Xml;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Composition : BomEntity, IXmlSerializable, IBomEntityWithRefType_String_BomRef, IBomEntityWithRefLinkType_StringList
    {
        [ProtoContract]
        public enum AggregateType
        {
            [XmlEnum(Name = "not_specified")]
            Not_Specified,
            [XmlEnum(Name = "complete")]
            Complete,
            [XmlEnum(Name = "incomplete")]
            Incomplete,
            [XmlEnum(Name = "incomplete_first_party_only")]
            Incomplete_First_Party_Only,
            [XmlEnum(Name = "incomplete_third_party_only")]
            Incomplete_Third_Party_Only,
            [XmlEnum(Name = "unknown")]
            Unknown,
            [XmlEnum(Name = "incomplete_first_party_proprietary_only")]
            Incomplete_First_Party_Proprietary_Only,
            [XmlEnum(Name = "incomplete_first_party_opensource_only")]
            Incomplete_First_Party_Opensource_Only,
            [XmlEnum(Name = "incomplete_third_party_proprietary_only")]
            Incomplete_Third_Party_Proprietary_Only,
            [XmlEnum(Name = "incomplete_third_party_opensource_only")]
            Incomplete_Third_Party_Opensource_Only,
        }

        [ProtoMember(1, IsRequired=true)]
        public AggregateType Aggregate { get; set; }

        [ProtoMember(2)]
        public List<string> Assemblies { get; set; }

        [ProtoMember(3)]
        public List<string> Dependencies { get; set; }
        
        [ProtoMember(4)]
        public List<string> Vulnerabilities { get; set; }
        
        [JsonPropertyName("bom-ref")]
        [ProtoMember(5)]
        public string BomRef { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            BomRef = reader.GetAttribute("bom-ref");
            reader.ReadStartElement();
            
            if (reader.LocalName == "aggregate")
            {
                var aggregateString = reader.ReadElementContentAsString();
                var aggregateType = AggregateType.Not_Specified;
                Enum.TryParse<AggregateType>(aggregateString, ignoreCase: true, out aggregateType);
                Aggregate = aggregateType;
            }

            if (reader.LocalName == "assemblies")
            {
                Assemblies = new List<string>();
                reader.ReadToDescendant("assembly");
                while (reader.LocalName == "assembly")
                {
                    if (reader.HasAttributes)
                    {
                        var bomRef = reader["ref"];
                        if (bomRef != null)
                        {
                            Assemblies.Add(bomRef);
                        }
                    }

                    reader.Read();
                }
                reader.ReadEndElement();
            }

            if (reader.LocalName == "dependencies")
            {
                Dependencies = new List<string>();
                reader.ReadToDescendant("dependency");
                while (reader.LocalName == "dependency")
                {
                    if (reader.HasAttributes)
                    {
                        var bomRef = reader["ref"];
                        if (bomRef != null)
                        {
                            Dependencies.Add(bomRef);
                        }
                    }

                    reader.Read();
                }
                reader.ReadEndElement();
            }

            if (reader.LocalName == "vulnerabilities")
            {
                Vulnerabilities = new List<string>();
                reader.ReadToDescendant("vulnerability");
                while (reader.LocalName == "vulnerability")
                {
                    if (reader.HasAttributes)
                    {
                        var bomRef = reader["ref"];
                        if (bomRef != null)
                        {
                            Vulnerabilities.Add(bomRef);
                        }
                    }

                    reader.Read();
                }
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }
        
        public void WriteXml(System.Xml.XmlWriter writer) {
            if (BomRef != null)
            {
                writer.WriteAttributeString("bom-ref", BomRef);
            }
            writer.WriteElementString("aggregate", Aggregate.ToString().ToLowerInvariant());
            if (Assemblies != null)
            {
                writer.WriteStartElement("assemblies");
                foreach (var assembly in Assemblies)
                {
                    writer.WriteStartElement("assembly");
                    writer.WriteStartAttribute("ref");
                    writer.WriteString(assembly);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (Dependencies != null)
            {
                writer.WriteStartElement("dependencies");
                foreach (var dependency in Dependencies)
                {
                    writer.WriteStartElement("dependency");
                    writer.WriteStartAttribute("ref");
                    writer.WriteString(dependency);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (Vulnerabilities != null)
            {
                writer.WriteStartElement("vulnerabilities");
                foreach (var vulnerability in Vulnerabilities)
                {
                    writer.WriteStartElement("vulnerability");
                    writer.WriteStartAttribute("ref");
                    writer.WriteString(vulnerability);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// See BomEntity.NormalizeList() and ListMergeHelper.SortByImpl().
        /// Note that as a static method this is not exactly an "override",
        /// but the BomEntity base class implementation makes it behave
        /// like that in practice.
        /// </summary>
        /// <param name="ascending">Ascending (true) or Descending (false)</param>
        /// <param name="recursive">Passed to BomEntity.NormalizeList() (effective if recursing), not handled right here</param>
        /// <param name="list">List<Composition> to sort</param>
        public static void NormalizeList(bool ascending, bool recursive, List<Composition> list)
        {
            var sortHelper = new ListMergeHelper<Composition>();
            sortHelper.SortByImpl(ascending, recursive, list,
                o => (o?.Aggregate, o?.Assemblies, o?.Dependencies),
                null);
        }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_List_v1_3 =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(Composition).GetProperty("Assemblies", typeof(List<string>)), RefLinkConstraints_ComponentOrService },
            { typeof(Composition).GetProperty("Dependencies", typeof(List<string>)), RefLinkConstraints_ComponentOrService }
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_List_v1_5 =
        new Dictionary<PropertyInfo, ImmutableList<Type>>()
        {
            { typeof(Composition).GetProperty("Assemblies", typeof(List<string>)), RefLinkConstraints_ComponentOrService },
            { typeof(Composition).GetProperty("Dependencies", typeof(List<string>)), RefLinkConstraints_ComponentOrService },
            { typeof(Composition).GetProperty("Vulnerabilities", typeof(List<string>)), RefLinkConstraints_Vulnerability }
        }.ToImmutableDictionary();

        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            switch (specificationVersion)
            {
                case v1_0:
                case v1_1:
                case v1_2:
                    return null;

                case v1_3:
                case v1_4:
                    return RefLinkConstraints_List_v1_3;

                case v1_5:
                default:
                    return RefLinkConstraints_List_v1_5;
            }
        }
    }
}
