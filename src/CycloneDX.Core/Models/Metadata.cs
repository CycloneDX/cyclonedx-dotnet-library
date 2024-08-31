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
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Metadata
    {
        private DateTime? _timestamp;
        [XmlElement("timestamp")]
        [ProtoMember(1)]
        public DateTime? Timestamp
        {
            get => _timestamp;
            set { _timestamp = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }

        [XmlElement("tools")]
        public ToolChoices Tools { get; set; }
        
        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(2)]
        public List<ProtobufTools> ProtobufTools
        {
            get {
                if (Tools == null)
                {
                    return null;
                }
                var protobufTools = new List<ProtobufTools>();
                if (Tools.Tools != null)
                {
                    foreach(var tool in Tools.Tools)
                    {
                        protobufTools.Add(new ProtobufTools(tool));
                    }
                }
                if (Tools.Components != null)
                {
                    if (protobufTools.Count == 0)
                    {
                        protobufTools.Add(new ProtobufTools());
                    }
                    protobufTools[0].Components = Tools.Components;
                }
                if (Tools.Services != null)
                {
                    if (protobufTools.Count == 0)
                    {
                        protobufTools.Add(new ProtobufTools());
                    }
                    protobufTools[0].Services = Tools.Services;
                }
                return protobufTools;
            }
            set
            {
                if (value == null)
                {
                    Tools = null;
                    return;
                }
                #pragma warning disable 618
                List<Tool> tools = null;
                #pragma warning restore 618
                List<Component> components = null;
                List<Service> services = null;
                foreach (var protobufTools in value)
                {
                    if (!(protobufTools.Components?.Count > 0 || (protobufTools.Services?.Count > 0)))
                    {
                        if (tools == null)
                        {
                            #pragma warning disable 618
                            tools = new List<Tool>();
                            #pragma warning restore 618
                        }
                        tools.Add(protobufTools.ToTool());
                    }
                    if (protobufTools.Components != null)
                    {
                        if (components == null)
                        {
                            components = new List<Component>();
                        }
                        components.AddRange(protobufTools.Components);
                    }
                    if (protobufTools.Services != null)
                    {
                        if (services == null)
                        {
                            services = new List<Service>();
                        }
                        services.AddRange(protobufTools.Services);
                    }
                }
                Tools = new ToolChoices
                {
                    Tools = tools, Components = components, Services = services
                };
            }
        }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        [ProtoMember(3)]
        public List<OrganizationalContact> Authors { get; set; }

        [XmlElement("component")]
        [ProtoMember(4)]
        public Component Component { get; set; }

        [XmlElement("manufacturer")]
        [ProtoMember(10)]
        public OrganizationalEntity Manufacturer { get; set; }
        public bool ShouldSerializeManufacturer() { return Manufacturer != null; }

        [XmlElement("manufacture")]
        [ProtoMember(5)]
        public OrganizationalEntity Manufacture { get; set; }

        [XmlElement("supplier")]
        [ProtoMember(6)]
        public OrganizationalEntity Supplier { get; set; }

        [XmlIgnore]
        [ProtoMember(7)]
        public List<LicenseChoice> Licenses { get; set; }
        public bool ShouldSerializeLicenses() { return Licenses?.Count > 0; }


        [XmlElement("licenses")]
        [JsonIgnore, ProtoIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        // This is a serialization workaround
        public LicenseChoiceList LicensesSerialized
        {
            get { return Licenses != null ? new LicenseChoiceList(Licenses) : null; }
            set { Licenses = value.Licenses; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLicensesSerialized() { return Licenses?.Count > 0; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(8)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        
        [XmlArray("lifecycles")]
        [XmlArrayItem("lifecycle")]
        [ProtoMember(9)]
        public List<Lifecycles> Lifecycles { get; set; }
        public bool ShouldSerializeLifecycles() { return Lifecycles?.Count > 0; }
    }
}
