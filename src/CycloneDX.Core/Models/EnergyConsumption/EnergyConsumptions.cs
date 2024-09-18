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

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class EnergyConsumption
    {
        [XmlElement("activity")]
        [ProtoMember(1)]
        public ActivityType Activity { get; set; }

        [XmlElement("energyProviders")]
        [ProtoMember(2)]
        public List<EnergyProvider> EnergyProviders { get; set; }

        [XmlElement("activityEnergyCost")]
        [ProtoMember(3)]
        public EnergyMeasure ActivityEnergyCost { get; set; }

        [XmlElement("co2CostEquivalent")]
        [ProtoMember(4)]
        public Co2Measure Co2CostEquivalent { get; set; }

        [XmlElement("co2CostOffset")]
        [ProtoMember(5)]
        public Co2Measure Co2CostOffset { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(6)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() => Properties?.Count > 0;
    }
}
