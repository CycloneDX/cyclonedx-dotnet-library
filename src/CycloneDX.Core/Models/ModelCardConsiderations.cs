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
    [XmlType("modelCardConsiderations")]
    [ProtoContract]
    public class ModelCardConsiderations : IEquatable<ModelCardConsiderations>
    {
        [ProtoContract]
        public class ModelCardEthicalConsideration : IEquatable<ModelCardEthicalConsideration>
        {
            [XmlElement("name")]
            [ProtoMember(1)]
            public string Name { get; set; }
            
            [XmlElement("mitigationStrategy")]
            [ProtoMember(2)]
            public string MitigationStrategy { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as ModelCardEthicalConsideration);
            }

            public bool Equals(ModelCardEthicalConsideration obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.MitigationStrategy, obj.MitigationStrategy) ||
                    this.MitigationStrategy.Equals(obj.MitigationStrategy, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.Name, obj.Name) ||
                    this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        
        [ProtoContract]
        public class ModelCardFairnessAssessment : IEquatable<ModelCardFairnessAssessment>
        {
            [XmlElement("groupAtRisk")]
            [ProtoMember(1)]
            public string GroupAtRisk { get; set; }
            
            [XmlElement("benefits")]
            [ProtoMember(2)]
            public string Benefits { get; set; }
            
            [XmlElement("harms")]
            [ProtoMember(3)]
            public string Harms { get; set; }
            
            [XmlElement("mitigationStrategy")]
            [ProtoMember(4)]
            public string MitigationStrategy { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as ModelCardFairnessAssessment);
            }

            public bool Equals(ModelCardFairnessAssessment obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.Benefits, obj.Benefits) ||
                    this.Benefits.Equals(obj.Benefits, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.GroupAtRisk, obj.GroupAtRisk) ||
                    this.GroupAtRisk.Equals(obj.GroupAtRisk, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.Harms, obj.Harms) ||
                    this.Harms.Equals(obj.Harms, StringComparison.InvariantCultureIgnoreCase)) &&
                    (object.ReferenceEquals(this.MitigationStrategy, obj.MitigationStrategy) ||
                    this.MitigationStrategy.Equals(obj.MitigationStrategy, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        
        [XmlArray("users")]
        [XmlArrayItem("user")]
        [ProtoMember(1)]
        public List<string> Users { get; set; }

        [XmlArray("useCases")]
        [XmlArrayItem("useCase")]
        [ProtoMember(2)]
        public List<string> UseCases { get; set; }

        [XmlArray("technicalLimitations")]
        [XmlArrayItem("technicalLimitation")]
        [ProtoMember(3)]
        public List<string> TechnicalLimitations { get; set; }

        [XmlArray("performanceTradeoffs")]
        [XmlArrayItem("performanceTradeoff")]
        [ProtoMember(4)]
        public List<string> PerformanceTradeoffs { get; set; }

        [XmlArray("ethicalConsiderations")]
        [XmlArrayItem("ethicalConsideration")]
        [ProtoMember(5)]
        public List<ModelCardEthicalConsideration> EthicalConsiderations { get; set; }

        [XmlArray("fairnessAssessments")]
        [XmlArrayItem("fairnessAssessment")]
        [ProtoMember(6)]
        public List<ModelCardFairnessAssessment> FairnessAssessments { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ModelCardConsiderations);
        }

        public bool Equals(ModelCardConsiderations obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.EthicalConsiderations, obj.EthicalConsiderations) ||
                this.EthicalConsiderations.SequenceEqual(obj.EthicalConsiderations)) &&
                (object.ReferenceEquals(this.FairnessAssessments, obj.FairnessAssessments) ||
                this.FairnessAssessments.SequenceEqual(obj.FairnessAssessments)) &&
                (object.ReferenceEquals(this.PerformanceTradeoffs, obj.PerformanceTradeoffs) ||
                this.PerformanceTradeoffs.SequenceEqual(obj.PerformanceTradeoffs)) &&
                (object.ReferenceEquals(this.TechnicalLimitations, obj.TechnicalLimitations) ||
                this.TechnicalLimitations.SequenceEqual(obj.TechnicalLimitations)) &&
                (object.ReferenceEquals(this.UseCases, obj.UseCases) ||
                this.UseCases.SequenceEqual(obj.UseCases)) &&
                (object.ReferenceEquals(this.Users, obj.Users) ||
                this.Users.SequenceEqual(obj.Users));
        }
    }
}