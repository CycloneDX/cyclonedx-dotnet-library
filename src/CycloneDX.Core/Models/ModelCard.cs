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
    [XmlType("modelCard")]
    [ProtoContract]
    public class ModelCard : BomEntity, IBomEntityWithRefType_String_BomRef
    {
        [ProtoContract]
        public enum ModelParameterApproachType
        {
            [XmlEnum(Name = "supervised")]
            Supervised,
            [XmlEnum(Name = "unsupervised")]
            Unsupervised,
            [XmlEnum(Name = "reinforced-learning")]
            ReinforcedLearning,
            [XmlEnum(Name = "semi-supervised")]
            SemiSupervised,
            [XmlEnum(Name = "self-supervised")]
            SelfSupervised,
        }

        [ProtoContract]
        public class ModelCardQuantitativeAnalysis : BomEntity
        {
            [ProtoContract]
            public class PerformanceMetric : BomEntity
            {
                [ProtoContract]
                public class PerformanceMetricConfidenceInterval : BomEntity
                {
                    [XmlElement("lowerBound")]
                    [ProtoMember(1)]
                    public string LowerBound { get; set; }
                    
                    [XmlElement("upperBound")]
                    [ProtoMember(2)]
                    public string UpperBound { get; set; }
                }

                [XmlElement("type")]
                [ProtoMember(1)]
                public string Type { get; set; }

                [XmlElement("value")]
                [ProtoMember(2)]
                public string Value { get; set; }

                [XmlElement("slice")]
                [ProtoMember(3)]
                public string Slice { get; set; }
                
                [XmlElement("confidenceInterval")]
                [ProtoMember(4)]
                public PerformanceMetricConfidenceInterval ConfidenceInterval { get; set; }
            }
            
            [XmlArray("performanceMetrics")]
            [XmlArrayItem("performanceMetric")]
            [ProtoMember(1)]
            public List<PerformanceMetric> PerformanceMetrics { get; set; }
            
            [XmlElement("graphics")]
            [ProtoMember(2)]
            public GraphicsCollection Graphics { get; set; }
        }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("modelParameters")]
        [ProtoMember(2)]
        public ModelParameters ModelParameters { get; set; }

        [XmlElement("quantitativeAnalysis")]
        [ProtoMember(3)]
        public ModelCardQuantitativeAnalysis QuantitativeAnalysis { get; set; }

        [XmlElement("considerations")]
        [ProtoMember(4)]
        public ModelCardConsiderations Considerations { get; set; }
    }
}