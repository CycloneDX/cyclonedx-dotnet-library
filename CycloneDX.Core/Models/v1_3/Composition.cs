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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class Composition : IXmlSerializable
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
            Unknown
        }

        [ProtoMember(1, IsRequired=true)]
        public AggregateType Aggregate { get; set; }

        [ProtoMember(2)]
        public List<string> Assemblies { get; set; }

        [ProtoMember(3)]
        public List<string> Dependencies { get; set; }

        public Composition() { }
        
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            if (reader.LocalName == "aggregate")
            {
                var aggregateString = reader.ReadElementContentAsString();
                var aggregateType = AggregateType.Not_Specified;
                Enum.TryParse<AggregateType>(aggregateString.Replace("_", ""), ignoreCase: true, out aggregateType);
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
                        if (bomRef != null) Assemblies.Add(bomRef);
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
                        if (bomRef != null) Dependencies.Add(bomRef);
                    }

                    reader.Read();
                }
                reader.ReadEndElement();
            }
            
            reader.ReadEndElement();
        }
        
        public void WriteXml(System.Xml.XmlWriter writer) {
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
        }
    }
}