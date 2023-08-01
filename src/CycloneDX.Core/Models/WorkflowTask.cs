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
    [XmlType("task")]
    [ProtoContract]
    public class WorkflowTask
    {
        [ProtoContract]
        public enum TaskType
        {
            [XmlEnum(Name = "copy")]
            Copy,
            [XmlEnum(Name = "clone")]
            Clone,
            [XmlEnum(Name = "lint")]
            Lint,
            [XmlEnum(Name = "scan")]
            Scan,
            [XmlEnum(Name = "merge")]
            Merge,
            [XmlEnum(Name = "build")]
            Build,
            [XmlEnum(Name = "test")]
            Test,
            [XmlEnum(Name = "deliver")]
            Deliver,
            [XmlEnum(Name = "deploy")]
            Deploy,
            [XmlEnum(Name = "release")]
            Release,
            [XmlEnum(Name = "clean")]
            Clean,
            [XmlEnum(Name = "other")]
            Other,
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

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(5)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        
        [XmlElement("resourceReferences")]
        [ProtoMember(6)]
        public ResourceReferenceChoices ResourceReferences { get; set; }

        [XmlArray("taskTypes")]
        [XmlArrayItem("taskType")]
        [ProtoMember(7)]
        public List<TaskType> TaskTypes { get; set; }

        [XmlElement("trigger")]
        [ProtoMember(8)]
        public Trigger Trigger { get; set; }

        [XmlArray("steps")]
        [ProtoMember(9)]
        public List<Step> Steps { get; set; }

        [XmlArray("inputs")]
        [ProtoMember(10)]
        public List<Input> Inputs { get; set; }

        [XmlArray("outputs")]
        [ProtoMember(11)]
        public List<Output> Outputs { get; set; }

        private DateTime? _timeStart;
        [XmlElement("timeStart")]
        [ProtoMember(14)]
        public DateTime? TimeStart
        { 
            get => _timeStart;
            set { _timeStart = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimeStart() { return TimeStart != null; }

        private DateTime? _timeEnd;
        [XmlElement("timeEnd")]
        [ProtoMember(15)]
        public DateTime? TimeEnd
        { 
            get => _timeEnd;
            set { _timeEnd = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimeEnd() { return TimeEnd != null; }

        [XmlArray("workspaces")]
        [XmlArrayItem("workspace")]
        [ProtoMember(16)]
        public List<Workspace> Workspaces { get; set; }

        [XmlArray("runtimeTopology")]
        [XmlArrayItem("dependency")]
        [ProtoMember(17)]
        public List<Dependency> RuntimeTopologies { get; set; }
    }
}