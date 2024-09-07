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
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Service: IEquatable<Service>
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

        [XmlIgnore]
        [ProtoMember(11)]
        public List<LicenseChoice> Licenses { get; set; }

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Service);
        }

        public bool Equals(Service obj)
        {
            return obj != null &&
                (this.Authenticated == obj.Authenticated) &&
                (object.ReferenceEquals(this.BomRef, obj.BomRef) ||
                this.BomRef.Equals(obj.BomRef, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Data, obj.Data) ||
                this.Data.SequenceEqual(obj.Data)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.BomRef.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Endpoints, obj.Endpoints) ||
                this.Endpoints.SequenceEqual(obj.Endpoints)) &&
                (object.ReferenceEquals(this.ExternalReferences, obj.ExternalReferences) ||
                this.ExternalReferences.SequenceEqual(obj.ExternalReferences)) &&
                (object.ReferenceEquals(this.Group, obj.Group) ||
                this.Group.Equals(obj.Group, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Licenses, obj.Licenses) ||
                this.Licenses.SequenceEqual(obj.Licenses)) &&
                (object.ReferenceEquals(this.LicensesSerialized, obj.LicensesSerialized) ||
                this.LicensesSerialized.Equals(obj.LicensesSerialized)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (this.NonNullableAuthenticated.Equals(obj.NonNullableAuthenticated)) &&
                (this.NonNullableXTrustBoundary.Equals(obj.NonNullableXTrustBoundary)) &&
                (object.ReferenceEquals(this.Properties, obj.Properties) ||
                this.Properties.SequenceEqual(obj.Properties)) &&
                (object.ReferenceEquals(this.Provider, obj.Provider) ||
                this.Provider.Equals(obj.Provider)) &&
                (object.ReferenceEquals(this.ReleaseNotes, obj.ReleaseNotes) ||
                this.ReleaseNotes.Equals(obj.ReleaseNotes)) &&
                (object.ReferenceEquals(this.Services, obj.Services) ||
                this.Services.SequenceEqual(obj.Services)) &&
                (this.SpecVersion.Equals(obj.SpecVersion)) &&
                (object.ReferenceEquals(this.TrustZone, obj.TrustZone) ||
                this.TrustZone.Equals(obj.TrustZone, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Version, obj.Version) ||
                this.Version.Equals(obj.Version, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.XmlData, obj.XmlData) ||
                this.XmlData.Equals(obj.XmlData)) &&
                (this.TrustZone.Equals(obj.XTrustBoundary));
        }
    
        public override int GetHashCode()
        {
            return CycloneDX.Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}
