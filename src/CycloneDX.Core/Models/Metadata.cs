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
    public class Metadata : IEquatable<Metadata>
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
        
        // this is to support a bug in v1.5 of the protobuf spec
        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(2)]
        #pragma warning disable 618
        public List<Tool> ProtobufTools
        #pragma warning restore 618
        {
            get => Tools?.Tools;
            set
            {
                if (value == null)
                {
                    Tools = null;
                }
                else
                {
                    Tools = new ToolChoices
                    {
                        Tools = value
                    };
                }
            }
        }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        [ProtoMember(3)]
        public List<OrganizationalContact> Authors { get; set; }

        [XmlElement("component")]
        [ProtoMember(4)]
        public Component Component { get; set; }

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Metadata);
        }

        public bool Equals(Metadata obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Authors, obj.Authors) ||
                this.Authors.SequenceEqual(obj.Authors)) &&
                (object.ReferenceEquals(this.Component, obj.Component) ||
                this.Component.Equals(obj.Component)) &&
                (object.ReferenceEquals(this.Licenses, obj.Licenses) ||
                this.Licenses.SequenceEqual(obj.Licenses)) &&
                (object.ReferenceEquals(this.LicensesSerialized, obj.LicensesSerialized) ||
                this.LicensesSerialized.Equals(obj.LicensesSerialized)) &&
                (object.ReferenceEquals(this.Lifecycles, obj.Lifecycles) ||
                this.Lifecycles.SequenceEqual(obj.Lifecycles)) &&
                (object.ReferenceEquals(this.Manufacture, obj.Manufacture) ||
                this.Manufacture.Equals(obj.Manufacture)) &&
                (object.ReferenceEquals(this.Properties, obj.Properties) ||
                this.Properties.SequenceEqual(obj.Properties)) &&
                (object.ReferenceEquals(this.ProtobufTools, obj.ProtobufTools) ||
                this.ProtobufTools.Equals(obj.ProtobufTools)) &&
                (object.ReferenceEquals(this.Supplier, obj.Supplier) ||
                this.Supplier.Equals(obj.Supplier)) &&
                (this.Timestamp.Equals(obj.Timestamp)) &&
                (object.ReferenceEquals(this.Tools, obj.Tools) ||
                this.Tools.Equals(obj.Tools));
        }
    }
}
