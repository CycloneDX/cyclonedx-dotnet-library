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
    public class ToolChoices : IXmlSerializable
    {
        internal SpecificationVersion SpecVersion { get; set; }

        [ProtoMember(1)]
        #pragma warning disable 618
        public List<Tool> Tools { get; set; }
        #pragma warning restore 618
        
        [ProtoMember(6)]
        public List<Component> Components { get; set; }

        public bool ShouldSerializeComponents() => Components?.Count > 0;

        [ProtoMember(7)]
        public List<Service> Services { get; set; }

        public bool ShouldSerializeServices() => Services?.Count > 0;
        
        public ToolChoices()
        {
            SpecVersion = SpecificationVersionHelpers.CurrentVersion;
        }
        
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool toolsIsEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            SpecVersion = SpecificationVersionHelpers.Version(SpecificationVersionHelpers.XmlNamespaceSpecificationVersion(reader.NamespaceURI));
            while (reader.LocalName == "tool" || reader.LocalName == "components" || reader.LocalName == "services")
            {
                if (reader.LocalName == "tool")
                {
                    #pragma warning disable 618
                    if (this.Tools == null)
                    {
                        this.Tools = new List<Tool>();
                    }
                    var serializer = Xml.Serializer.GetElementSerializer<Tool>(SpecVersion, "tool");
                    var tool = (Tool)serializer.Deserialize(reader);
                    #pragma warning restore 618
                    this.Tools.Add(tool);
                }
                if (reader.LocalName == "components")
                {
                    if (this.Components == null)
                    {
                        this.Components = new List<Component>();
                    }
                    var serializer = Xml.Serializer.GetElementSerializer<Component>(SpecVersion, "component");
                    var isEmpty = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    while (reader.LocalName == "component")
                    {
                        var component = (Component)serializer.Deserialize(reader);
                        this.Components.Add(component);
                    }
                    if (!isEmpty) {
                        reader.ReadEndElement();
                    }
                }
                if (reader.LocalName == "services")
                {
                    if (this.Services == null)
                    {
                        this.Services = new List<Service>();
                    }
                    var serializer = Xml.Serializer.GetElementSerializer<Service>(SpecVersion, "service");
                    var isEmpty = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    while (reader.LocalName == "service")
                    {
                        var service = (Service)serializer.Deserialize(reader);
                        this.Services.Add(service);
                    }
                    if (!isEmpty)
                    {
                        reader.ReadEndElement();
                    }
                }
            }
            if (!toolsIsEmpty)
            {
                reader.ReadEndElement();
            }
        }
        
        public void WriteXml(System.Xml.XmlWriter writer) {
            if (this.Tools != null)
            {
                #pragma warning disable 618
                var serializer = Xml.Serializer.GetElementSerializer<Tool>(SpecVersion, "tool");
                #pragma warning restore 618
                foreach (var tool in this.Tools)
                {
                    serializer.Serialize(writer, tool);
                }
            }

            if (this.Components != null)
            {
                writer.WriteStartElement("components");
                var serializer = Xml.Serializer.GetElementSerializer<Component>(SpecVersion, "component");
                foreach (var component in this.Components)
                {
                    serializer.Serialize(writer, component);
                }
                writer.WriteEndElement();
            }

            if (this.Services != null)
            {
                writer.WriteStartElement("services");
                var serializer = Xml.Serializer.GetElementSerializer<Service>(SpecVersion, "service");
                foreach (var service in this.Services)
                {
                    serializer.Serialize(writer, service);
                }
                writer.WriteEndElement();
            }
        }
    }

    [ProtoContract]
    public class ProtobufTools
    {

        public ProtobufTools()
        {

        }

        #pragma warning disable 618
        public ProtobufTools(Tool tool)
        {
            if (tool == null)
            {
                return;
            }
            Vendor = tool.Vendor;
            Name = tool.Name;
            Version = tool.Version;
            Hashes = tool.Hashes;
            ExternalReferences = tool.ExternalReferences;
        }
        
        public Tool ToTool()
        {
            return new Tool { Vendor = Vendor, Name = Name, Version = Version, Hashes = Hashes, ExternalReferences = ExternalReferences };
        }
        #pragma warning restore 618

        [ProtoMember(1)]
        public string Vendor { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public string Version { get; set; }

        [ProtoMember(4)]
        public List<Hash> Hashes { get; set; }

        [ProtoMember(5)]
        public List<ExternalReference> ExternalReferences { get; set; }

        [ProtoMember(6)]
        public List<Component> Components { get; set; }

        [ProtoMember(7)]
        public List<Service> Services { get; set; }

    }
}
