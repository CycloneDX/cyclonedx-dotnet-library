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
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using CycloneDX.Core.Models;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("component")]
    [ProtoContract]
    public class Component: IEquatable<Component>, IHasBomRef
    {
        [ProtoContract]
        public enum Classification
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "application")]
            Application,
            [XmlEnum(Name = "framework")]
            Framework,
            [XmlEnum(Name = "library")]
            Library,
            [XmlEnum(Name = "operating-system")]
            Operating_System,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "file")]
            File,
            [XmlEnum(Name = "container")]
            Container,
            [XmlEnum(Name = "firmware")]
            Firmware,
            [XmlEnum(Name = "device-driver")]
            Device_Driver,
            [XmlEnum(Name = "platform")]
            Platform,
            [XmlEnum(Name = "machine-learning-model")]
            Machine_Learning_Model,
            [XmlEnum(Name = "data")]
            Data,
            [XmlEnum(Name = "cryptographic-asset")]
            Cryptographic_Asset,
        }

        [ProtoContract]
        public enum ComponentScope
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "required")]
            Required,
            [XmlEnum(Name = "optional")]
            Optional,
            [XmlEnum(Name = "excluded")]
            Excluded
        }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public Classification Type { get; set; }

        [JsonPropertyName("mime-type")]
        [XmlAttribute("mime-type")]
        [ProtoMember(2)]
        public string MimeType { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(3)]
        public string BomRef { get; set; }

        [XmlElement("supplier")]
        [ProtoMember(4)]
        public OrganizationalEntity Supplier { get; set; }

        [XmlElement("manufacturer")]
        [ProtoMember(28)]
        public OrganizationalEntity Manufacturer { get; set; }
        public bool ShouldSerializeManufacturer() { return Manufacturer != null; }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        [ProtoMember(29)]
        public List<OrganizationalContact> Authors { get; set; }
        public bool ShouldSerializeAuthors() { return Authors?.Count > 0; }

        [Obsolete("This will be removed in a future version. Use @.authors or @.manufacturer instead.")]
        [XmlIgnore]
        [ProtoMember(5)]
        public string Author { get; set; }

        #pragma warning disable 618
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("author")]
        [JsonIgnore]
        public string Author_Xml { get { return Author; } set { Author = value; } }
        public bool ShouldSerializeAuthor_Xml() { return Author != null; }
        #pragma warning restore 618


        [XmlElement("publisher")]
        [ProtoMember(6)]
        public string Publisher { get; set; }

        [XmlElement("group")]
        [ProtoMember(7)]
        public string Group { get; set; }

        [XmlElement("name")]
        [ProtoMember(8)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(9)]
        public string Version { get; set; }

        [XmlElement("description")]
        [ProtoMember(10)]
        public string Description { get; set; }

        [XmlIgnore]
        [ProtoMember(11)]
        public ComponentScope? Scope { get; set; }
        [XmlElement("scope")]
        [JsonIgnore]
        public ComponentScope NonNullableScope
        {
            get
            {
                return Scope.Value;
            }
            set
            {
                Scope = value;
            }
        }
        public bool ShouldSerializeNonNullableScope() { return Scope.HasValue; }

        [XmlArray("hashes")]
        [ProtoMember(12)]
        public List<Hash> Hashes { get; set; }
        public bool ShouldSerializeHashes() { return Hashes?.Count > 0; }

        [XmlIgnore]
        [ProtoMember(13)]
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

        [XmlElement("copyright")]
        [ProtoMember(14)]
        public string Copyright { get; set; }

        [XmlElement("cpe")]
        [ProtoMember(15)]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        [ProtoMember(16)]
        public string Purl { get; set; }

        [XmlElement("swid")]
        [ProtoMember(17)]
        public Swid Swid { get; set; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
        [ProtoMember(18)]
        public bool? Modified { get; set; }
        [XmlElement("modified")]
        [JsonIgnore]
        public bool NonNullableModified
        {
            get
            {
                return Modified.HasValue && Modified.Value;
            }
            set
            {
                Modified = value;
            }
        }
        public bool ShouldSerializeNonNullableModified() { return Modified.HasValue; }

        [XmlElement("pedigree")]
        [ProtoMember(19)]
        public Pedigree Pedigree { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(20)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }

        //In the xml format, Properties is in front of Components.
        //XML serialization uses the member order unless explicitly specified differently.
        public bool ShouldSerializeComponents() { return Components?.Count > 0; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(22)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        [XmlArray("components")]
        [ProtoMember(21)]
        public List<Component> Components { get; set; }
        
        [XmlElement("evidence")]
        [ProtoMember(23)]
        public Evidence Evidence { get; set; }

        [XmlElement("releaseNotes")]
        [ProtoMember(24)]
        public ReleaseNotes ReleaseNotes { get; set; }
        public bool ShouldSerializeReleaseNotes() { return ReleaseNotes != null; }
        
        [XmlElement("modelCard")]
        [ProtoMember(25)]
        public ModelCard ModelCard { get; set; }

        [XmlElement("data")]
        [ProtoMember(26)]
        public Data Data { get; set; }

        [XmlElement("cryptoProperties")]
        [ProtoMember(27)]
        public CryptoProperties CryptoProperties { get; set; }

        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        [ProtoMember(30)]
        public List<string> Tags { get; set; }
        public bool ShouldSerializeTags() { return Tags?.Count > 0; }

        [XmlElement("omniborId")]
        [ProtoMember(31)]
        public List<string> OmniborId { get; set; }
        public bool ShouldSerializeOmniborId() { return OmniborId?.Count > 0; }

        [XmlElement("swhid")]
        [ProtoMember(32)]
        public List<string> Swhid { get; set; }
        public bool ShouldSerializeSwhid() { return Swhid?.Count > 0; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }
        [XmlIgnore]
        public Signature Signature { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Component;
            if (other == null)
            {
                return false;
            }

            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(other);
        }

        public bool Equals(Component obj)
        {
            return Json.Serializer.Serialize(this) == Json.Serializer.Serialize(obj);
        }
    
        public override int GetHashCode()
        {
            return Json.Serializer.Serialize(this).GetHashCode();
        }
    }
}