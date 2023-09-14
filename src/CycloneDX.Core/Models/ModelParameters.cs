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
    [XmlType("model-parameters")]
    [ProtoContract]
    public class ModelParameters
    {
        [ProtoContract]
        public class ModelApproach
        {
            [XmlElement("type")]
            [ProtoMember(1)]
            public ModelCard.ModelParameterApproachType Type { get; set; }    
        }
        
        [ProtoContract]
        public class ModelDataset
        {
            [XmlElement("dataset")]
            [ProtoMember(1)]
            public Data Data { get; set; }    
            
            [XmlElement("ref")]
            [ProtoMember(2)]
            public string Ref { get; set; }    
        }

        [ProtoContract]
        public class MachineLearningInputOutputParameter
        {
            [XmlElement("format")]
            [ProtoMember(1)]
            public string Format { get; set; }
        }
        
        [XmlElement("approach")]
        [ProtoMember(1)]
        public ModelApproach Approach { get; set; }

        [XmlElement("task")]
        [ProtoMember(2)]
        public string Task { get; set; }

        [XmlElement("architectureFamily")]
        [ProtoMember(3)]
        public string ArchitectureFamily { get; set; }

        [XmlElement("modelArchitecture")]
        [ProtoMember(4)]
        public string ModelArchitecture { get; set; }
        
        [XmlElement("datasets")]
        [ProtoMember(5)]
        public DatasetChoices Datasets { get; set; }

        [XmlArray("inputs")]
        [XmlArrayItem("input")]
        [ProtoMember(6)]
        public List<MachineLearningInputOutputParameter> Inputs { get; set; }

        [XmlArray("outputs")]
        [XmlArrayItem("output")]
        [ProtoMember(7)]
        public List<MachineLearningInputOutputParameter> Outputs { get; set; }
    }
}