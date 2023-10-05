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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("trigger")]
    [ProtoContract]
    public class Trigger : BomEntity, IBomEntityWithRefType_String_BomRef
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
        public class Condition : BomEntity
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
    }
}