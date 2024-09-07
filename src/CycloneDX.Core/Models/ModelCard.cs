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
    public class ModelCard : IEquatable<ModelCard>
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
        public class ModelCardQuantitativeAnalysis : IEquatable<ModelCardQuantitativeAnalysis>
        {
            [ProtoContract]
            public class PerformanceMetric : IEquatable<PerformanceMetric>
            {
                [ProtoContract]
                public class PerformanceMetricConfidenceInterval : IEquatable<PerformanceMetricConfidenceInterval>
                {
                    [XmlElement("lowerBound")]
                    [ProtoMember(1)]
                    public string LowerBound { get; set; }
                    
                    [XmlElement("upperBound")]
                    [ProtoMember(2)]
                    public string UpperBound { get; set; }

                    public override bool Equals(object obj)
                    {
                        return Equals(obj as PerformanceMetricConfidenceInterval);
                    }

                    public bool Equals(PerformanceMetricConfidenceInterval obj)
                    {
                        return obj != null &&
                            (object.ReferenceEquals(this.LowerBound, obj.LowerBound) ||
                            this.LowerBound.Equals(obj.LowerBound)) &&
                            (object.ReferenceEquals(this.UpperBound, obj.UpperBound) ||
                            this.UpperBound.Equals(obj.UpperBound));
                    }
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

                public override bool Equals(object obj)
                {
                    return Equals(obj as PerformanceMetric);
                }

                public bool Equals(PerformanceMetric obj)
                {
                    return obj != null &&
                        (object.ReferenceEquals(this.ConfidenceInterval, obj.ConfidenceInterval) ||
                        this.ConfidenceInterval.Equals(obj.ConfidenceInterval)) &&
                        (object.ReferenceEquals(this.Slice, obj.Slice) ||
                        this.Slice.Equals(obj.Slice, StringComparison.InvariantCultureIgnoreCase)) &&
                        (this.Type == obj.Type) &&
                        (object.ReferenceEquals(this.Value, obj.Value) ||
                        this.Value.Equals(obj.Value, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            
            [XmlArray("performanceMetrics")]
            [XmlArrayItem("performanceMetric")]
            [ProtoMember(1)]
            public List<PerformanceMetric> PerformanceMetrics { get; set; }
            
            [XmlElement("graphics")]
            [ProtoMember(2)]
            public GraphicsCollection Graphics { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as ModelCardQuantitativeAnalysis);
            }

            public bool Equals(ModelCardQuantitativeAnalysis obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.Graphics, obj.Graphics) ||
                    this.Graphics.Equals(obj.Graphics)) &&
                    (object.ReferenceEquals(this.PerformanceMetrics, obj.PerformanceMetrics) ||
                    this.PerformanceMetrics.Equals(obj.PerformanceMetrics));
            }
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

        public override bool Equals(object obj)
        {
            return Equals(obj as ModelCard);
        }

        public bool Equals(ModelCard obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.BomRef, obj.BomRef) ||
                this.BomRef.Equals(obj.BomRef, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Considerations, obj.Considerations) ||
                this.Considerations.Equals(obj.Considerations)) &&
                (object.ReferenceEquals(this.ModelParameters, obj.ModelParameters) ||
                this.ModelParameters.Equals(obj.ModelParameters)) &&
                (object.ReferenceEquals(this.QuantitativeAnalysis, obj.QuantitativeAnalysis) ||
                this.QuantitativeAnalysis.Equals(obj.QuantitativeAnalysis));
        }
    }
}