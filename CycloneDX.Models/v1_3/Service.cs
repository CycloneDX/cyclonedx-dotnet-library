// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Models.v1_3
{
    public class Service
    {
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        public string BomRef { get; set; }

        [XmlElement("provider")]
        public OrganizationalEntity Provider { get; set; }

        [XmlElement("group")]
        public string Group { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlArray("endpoints")]
        [XmlArrayItem("endpoint")]
        public List<string> Endpoints { get; set; }

        [XmlIgnore]
        public bool? Authenticated { get; set; }
        [XmlElement("authenticated")]
        [JsonIgnore]
        // This is a serialization workaround
        public bool NonNullableAuthenticated
        {
            get
            {
                return Authenticated.HasValue && Authenticated.Value;
            }
            set
            {
                Authenticated = value;
            }
        }
        public bool ShouldSerializeNonNullableAuthenticated() { return Authenticated.HasValue; }

        [XmlIgnore]
        [JsonPropertyName("x-trust-boundary")]
        public bool? XTrustBoundary { get; set; }
        [XmlElement("x-trust-boundary")]
        [JsonIgnore]
        // This is a serialization workaround
        public bool NonNullableXTrustBoundary
        {
            get
            {
                return XTrustBoundary.HasValue && XTrustBoundary.Value;
            }
            set
            {
                XTrustBoundary = value;
            }
        }
        public bool ShouldSerializeNonNullableXTrustBoundary() { return XTrustBoundary.HasValue; }

        [XmlArray("data")]
        [XmlArrayItem("classification")]
        public List<DataClassification> Data { get; set; }

        [XmlElement("licenses")]
        public List<ComponentLicense> Licenses { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        public List<ExternalReference> ExternalReferences { get; set; }

        [XmlArray("services")]
        [XmlArrayItem("service")]
        public List<Service> Services { get; set; }

        public Service() {}

        public Service(v1_2.Service service)
        {
            BomRef = service.BomRef;
            if (service.Provider != null)
                Provider = new OrganizationalEntity(service.Provider);
            Group = service.Group;
            Name = service.Name;
            Version = service.Version;
            Description = service.Description;
            Endpoints = service.Endpoints;
            Authenticated = service.Authenticated;
            XTrustBoundary = service.XTrustBoundary;
            if (service.Data != null)
            {
                Data = new List<DataClassification>();
                foreach (var data in service.Data)
                {
                    Data.Add(new DataClassification(data));
                }
            }
            if (service.Licenses != null)
            {
                Licenses = new List<ComponentLicense>();
                foreach (var license in service.Licenses)
                {
                    Licenses.Add(new ComponentLicense(license));
                }
            }
            if (service.ExternalReferences != null)
            {
                ExternalReferences = new List<ExternalReference>();
                foreach (var externalReference in service.ExternalReferences)
                {
                    ExternalReferences.Add(new ExternalReference(externalReference));
                }
            }
            if (service.Services != null)
            {
                Services = new List<Service>();
                foreach (var serv in service.Services)
                {
                    Services.Add(new Service(serv));
                }
            }
        }
    }
}
