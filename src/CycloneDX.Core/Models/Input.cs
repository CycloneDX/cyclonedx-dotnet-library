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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("input")]
    [ProtoContract]
    public class Input : ICloneable
    {
        [XmlElement("resource")]
        [ProtoMember(3)]
        public ResourceReferenceChoice Resource { get; set; }

        [XmlElement("source")]
        [ProtoMember(1)]
        public ResourceReferenceChoice Source { get; set; }

        [XmlElement("target")]
        [ProtoMember(2)]
        public ResourceReferenceChoice Target { get; set; }

        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        [ProtoMember(4)]
        public List<Parameter> Parameters { get; set; }
        
        [XmlElement("environmentVars")]
        [ProtoMember(5)]
        public EnvironmentVarChoices EnvironmentVars { get; set; }
        
        [XmlElement("data")]
        [ProtoMember(6)]
        public AttachedText Data { get; set; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(7)]
        public List<Property> Properties { get; set; }
        
        public bool ShouldSerializeParameters() { return Parameters?.Count > 0; }
        public bool ShouldSerializeEnvironmentVars() { return EnvironmentVars != null; }

        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        public object Clone()
        {
            return new Input()
            {
                Data = (AttachedText)this.Data.Clone(),
                EnvironmentVars = (EnvironmentVarChoices)this.EnvironmentVars.Clone(),
                Parameters = this.Parameters.Select(x => (Parameter)x.Clone()).ToList(),
                Properties = this.Properties.Select(x => (Property)x.Clone()).ToList(),
                Resource = (ResourceReferenceChoice)this.Resource.Clone(),
                Source = (ResourceReferenceChoice)this.Source.Clone(),
                Target = (ResourceReferenceChoice)this.Target.Clone(),
            };
        }
    }
}