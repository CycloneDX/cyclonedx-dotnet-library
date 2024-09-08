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
    [XmlType("event")]
    [ProtoContract]
    public class Event : IEquatable<Event>
    {
        [XmlElement("uid")]
        [ProtoMember(1)]
        public string Uid { get; set; }

        [XmlElement("description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        private DateTime? _timeReceived;
        [XmlElement("timeReceived")]
        [ProtoMember(3)]
        public DateTime? TimeReceived
        { 
            get => _timeReceived;
            set { _timeReceived = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimeReceived() { return TimeReceived != null; }
        
        [XmlElement("data")]
        [ProtoMember(4)]
        public AttachedText Data { get; set; }

        [XmlElement("source")]
        [ProtoMember(5)]
        public ResourceReferenceChoice Source { get; set; }

        [XmlElement("target")]
        [ProtoMember(6)]
        public ResourceReferenceChoice Target { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(7)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Event);
        }

        public bool Equals(Event obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Data, obj.Data) ||
                this.Data.Equals(obj.Data)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.Description.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Properties, obj.Properties) ||
                this.Properties.SequenceEqual(obj.Properties)) &&
                (object.ReferenceEquals(this.Source, obj.Source) ||
                this.Description.Equals(obj.Source)) &&
                (object.ReferenceEquals(this.Target, obj.Target) ||
                this.Description.Equals(obj.Target)) &&
                (this.TimeReceived.Equals(obj.TimeReceived)) &&
                (object.ReferenceEquals(this.Uid, obj.Uid) ||
                this.Uid.Equals(obj.Uid, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}