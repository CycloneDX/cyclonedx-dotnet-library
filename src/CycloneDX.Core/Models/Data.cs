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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Data : IEquatable<Data>
    {
        [ProtoContract]
        public enum DataType
        {
            [XmlEnum(Name = "source-code")]
            SourceCode,
            [XmlEnum(Name = "configuration")]
            Configuration,
            [XmlEnum(Name = "dataset")]
            Dataset,
            [XmlEnum(Name = "definition")]
            Definition,
            [XmlEnum(Name = "other")]
            Other,
        }

        [ProtoContract]
        public class DataContents : IEquatable<DataContents>
        {
            [XmlElement("attachment")]
            [ProtoMember(1)]
            public AttachedText Attachment { get; set; }
            
            [XmlElement("url")]
            [ProtoMember(2)]
            public string Url { get; set; }
            
            [XmlArray("properties")]
            [XmlArrayItem("property")]
            [ProtoMember(22)]
            public List<Property> Properties { get; set; }
            public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

            public override bool Equals(object obj)
            {
                return Equals(obj as DataContents);
            }

            public bool Equals(DataContents obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.Attachment, obj.Attachment) ||
                    this.Attachment.Equals(obj.Attachment)) &&
                    (object.ReferenceEquals(this.Properties, obj.Properties) ||
                    this.Properties.SequenceEqual(obj.Properties)) &&
                    (object.ReferenceEquals(this.Url, obj.Url) ||
                    this.Url.Equals(obj.Url, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("type")]
        [ProtoMember(2)]
        public DataType Type { get; set; }

        [XmlElement("name")]
        [ProtoMember(3)]
        public string Name { get; set; }

        [XmlElement("contents")]
        [ProtoMember(4)]
        public DataContents Contents { get; set; }

        [XmlElement("classification")]
        [ProtoMember(5)]
        public string Classification { get; set; }

        [XmlElement("sensitiveData")]
        [ProtoMember(6)]
        public string SensitiveData { get; set; }

        [XmlElement("graphics")]
        [ProtoMember(7)]
        public GraphicsCollection Graphics { get; set; }

        [XmlElement("description")]
        [ProtoMember(8)]
        public string Description { get; set; }

        [XmlElement("governance")]
        [ProtoMember(9)]
        public DataGovernance Governance { get; set; }


        public override bool Equals(object obj)
        {
            return Equals(obj as Data);
        }

        public bool Equals(Data obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.BomRef, obj.BomRef) ||
                this.BomRef.Equals(obj.BomRef, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Classification, obj.Classification) ||
                this.Classification.Equals(obj.Classification, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Contents, obj.Contents) ||
                this.Contents.Equals(obj.Contents)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.Description.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Governance, obj.Governance) ||
                this.Governance.Equals(obj.Governance)) &&
                (object.ReferenceEquals(this.Graphics, obj.Graphics) ||
                this.Graphics.Equals(obj.Graphics)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.SensitiveData, obj.SensitiveData) ||
                this.SensitiveData.Equals(obj.SensitiveData, StringComparison.InvariantCultureIgnoreCase)) &&
                (this.Type.Equals(obj.Type));
        }
    }
}