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

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class EnvironmentVarChoices : List<EnvironmentVarChoice>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.LocalName == "value" || reader.LocalName == "environmentVar")
            {
                if (reader.LocalName == "value")
                {
                    var valueString = reader.ReadElementContentAsString();
                    this.Add(new EnvironmentVarChoice { Value = valueString });
                }
                if (reader.LocalName == "environmentVar")
                {
                    var nameString = reader.GetAttribute("name");
                    var valueString = reader.ReadElementContentAsString();
                    this.Add(new EnvironmentVarChoice { Property = new Property { Name = nameString, Value = valueString }});
                }
            }
            reader.ReadEndElement();
        }
        
        public void WriteXml(System.Xml.XmlWriter writer) {
            foreach (var envVar in this)
            {
                if (envVar.Value != null)
                {
                    writer.WriteElementString("value", envVar.Value);
                }
                else
                {
                    writer.WriteStartElement("environmentVar");
                    writer.WriteAttributeString("name", envVar.Property.Name);
                    writer.WriteString(envVar.Property.Value);
                    writer.WriteEndElement();
                }
            }
        }
    }
}
