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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Service : BomEntity, IBomEntityWithRefType_String_BomRef
    {
        public Service()
        {
            SpecVersion = SpecificationVersionHelpers.CurrentVersion;
        }
        
        internal SpecificationVersion SpecVersion { get; set; }

        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("provider")]
        [ProtoMember(2)]
        public OrganizationalEntity Provider { get; set; }

        [XmlElement("group")]
        [ProtoMember(3)]
        public string Group { get; set; }

        [XmlElement("name")]
        [ProtoMember(4)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(5)]
        public string Version { get; set; }

        [XmlElement("description")]
        [ProtoMember(6)]
        public string Description { get; set; }

        [XmlArray("endpoints")]
        [XmlArrayItem("endpoint")]
        [ProtoMember(7)]
        public List<string> Endpoints { get; set; }

        [XmlIgnore]
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        
        [XmlElement("trustZone")]
        [ProtoMember(16)]
        public string TrustZone { get; set; }
        
        [XmlIgnore]
        [JsonPropertyName("data")]
        [ProtoMember(10)]
        public List<DataFlow> Data { get; set; }
        public bool ShouldSerializeData() => Data?.Count > 0;

        [XmlElement("data")]
        [JsonIgnore]
        public ServiceDataChoices XmlData
        {
            get
            {
                if (Data == null) return null;
                if (SpecVersion < SpecificationVersion.v1_5)
                {
                    var result = new ServiceDataChoices()
                    {
                        SpecVersion = SpecVersion,
                        DataClassifications = new List<DataClassification>()
                    };
                    foreach (var data in Data)
                    {
                        result.DataClassifications.Add(data.XmlClassification);
                    }

                    return result;
                }
                else
                {
                    return new ServiceDataChoices
                    {
                        SpecVersion = SpecVersion,
                        DataFlows = Data
                    };
                }
            }
            set
            {
                if (value != null)
                {
                    Data = value.DataFlows != null ? Data = value.DataFlows : Data = new List<DataFlow>();
                    if (value.DataClassifications != null)
                    {
                        foreach (var classification in value.DataClassifications)
                        {
                            Data.Add(new DataFlow
                            {
                                XmlClassification = classification
                            });
                        }
                    }
                }
                else
                {
                    Data = null;
                }
            }
        }
        public bool ShouldSerializeXmlData() => ShouldSerializeData();
        
        [XmlElement("licenses")]
        [ProtoMember(11)]
        public List<LicenseChoice> Licenses { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(12)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() => ExternalReferences?.Count > 0;

        [XmlArray("services")]
        [XmlArrayItem("service")]
        [ProtoMember(13)]
        public List<Service> Services { get; set; }
        public bool ShouldSerializeServices() => Services?.Count > 0;

        [XmlElement("releaseNotes")]
        [ProtoMember(15)]
        public ReleaseNotes ReleaseNotes { get; set; }
        public bool ShouldSerializeReleaseNotes() { return ReleaseNotes != null; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(14)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() => Properties?.Count > 0;

        /// <summary>
        /// See BomEntity.NormalizeList() and ListMergeHelper.SortByImpl().
        /// Note that as a static method this is not exactly an "override",
        /// but the BomEntity base class implementation makes it behave
        /// like that in practice.
        /// </summary>
        /// <param name="ascending">Ascending (true) or Descending (false)</param>
        /// <param name="recursive">Passed to BomEntity.NormalizeList() (effective if recursing), not handled right here</param>
        /// <param name="list">List<Service> to sort</param>
        public static void NormalizeList(bool ascending, bool recursive, List<Service> list)
        {
            var sortHelper = new ListMergeHelper<Service>();
            sortHelper.SortByImpl(ascending, recursive, list,
                o => (o?.BomRef, o?.Group, o?.Name, o?.Version),
                null);
        }
    }
}
