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
    [XmlType("trigger")]
    [ProtoContract]
    public class Trigger : IEquatable<Trigger>
    {
        [ProtoContract]
        public enum TriggerType
        {
            [XmlEnum(Name = "manual")]
            Manual,
            [XmlEnum(Name = "api")]
            Api,
            [XmlEnum(Name = "webhook")]
            Webhook,
            [XmlEnum(Name = "scheduled")]
            Scheduled,
        }

        [ProtoContract]
        public class Condition : IEquatable<Condition>
        {
            [XmlElement("description")]
            [ProtoMember(1)]
            public string Description { get; set; }

            [XmlElement("expression")]
            [ProtoMember(2)]
            public string Expression { get; set; }

            [XmlArray("properties")]
            [XmlArrayItem("property")]
            [ProtoMember(3)]
            public List<Property> Properties { get; set; }
            public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Condition);
            }

            public bool Equals(Condition obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.Description, obj.Description) ||
                    this.Description.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.Expression, obj.Expression) ||
                    this.Expression.Equals(obj.Expression, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.Properties, obj.Properties) ||
                    this.Properties.SequenceEqual(obj.Properties));
            }
        }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("uid")]
        [ProtoMember(2)]
        public string Uid { get; set; }

        [XmlElement("name")]
        [ProtoMember(3)]
        public string Name { get; set; }

        [XmlElement("description")]
        [ProtoMember(4)]
        public string Description { get; set; }

        [XmlElement("resourceReferences")]
        [ProtoMember(6)]
        public ResourceReferenceChoices ResourceReferences { get; set; }
        public bool ShouldSerializeResourceReferences() { return ResourceReferences?.Count > 0; }

        [XmlElement("type")]
        [ProtoMember(7)]
        public TriggerType Type { get; set; }

        [XmlElement("event")]
        [ProtoMember(8)]
        public Event Event { get; set; }

        [XmlArray("conditions")]
        [XmlArrayItem("condition")]
        [ProtoMember(9)]
        public List<Condition> Conditions { get; set; }
        public bool ShouldSerializeConditions() { return Conditions?.Count > 0; }

        private DateTime? _timeActivated;
        [XmlElement("timeActivated")]
        [ProtoMember(10)]
        public DateTime? TimeActivated
        { 
            get => _timeActivated;
            set { _timeActivated = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimeActivated() { return TimeActivated != null; }

        [XmlArray("inputs")]
        [ProtoMember(11)]
        public List<Input> Inputs { get; set; }
        public bool ShouldSerializeInputs() { return Inputs?.Count > 0; }

        [XmlArray("outputs")]
        [ProtoMember(12)]
        public List<Output> Outputs { get; set; }
        public bool ShouldSerializeOutputs() { return Outputs?.Count > 0; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(5)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Trigger);
        }

        public bool Equals(Trigger obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.BomRef, obj.BomRef) ||
                this.BomRef.Equals(obj.BomRef, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Conditions, obj.Conditions) ||
                this.Conditions.SequenceEqual(obj.Conditions)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.Description.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Event, obj.Event) ||
                this.Event.Equals(obj.Event)) &&
                (object.ReferenceEquals(this.Inputs, obj.Inputs) ||
                this.Inputs.SequenceEqual(obj.Inputs)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.SequenceEqual(obj.Name)) &&
                (object.ReferenceEquals(this.Outputs, obj.Outputs) ||
                this.Outputs.SequenceEqual(obj.Outputs)) &&
                (object.ReferenceEquals(this.Properties, obj.Properties) ||
                this.Properties.SequenceEqual(obj.Properties)) &&
                (object.ReferenceEquals(this.ResourceReferences, obj.ResourceReferences) ||
                this.ResourceReferences.SequenceEqual(obj.ResourceReferences)) &&
                (this.TimeActivated.Equals(obj.TimeActivated)) &&
                (this.Type.Equals(obj.Type)) &&
                (object.ReferenceEquals(this.Uid, obj.Uid) ||
                this.Uid.Equals(obj.Uid, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}