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

using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    // this is the preferred way to represent this information for services since v1.5
    [XmlType("dataflow")]
    [ProtoContract]
    public class DataFlow
    {
        [XmlElement("classification")]
        [ProtoMember(2)]
        public DataClassification Classification { get; set; }

        [XmlAttribute("name")]
        [ProtoMember(3)]
        public string Name { get; set; }
        
        [XmlAttribute("description")]
        [ProtoMember(4)]
        public string Description { get; set; }
        
        [XmlElement("governance")]
        [ProtoMember(7)]
        public DataGovernance Governance { get; set; }

        [XmlElement("source")]
        [ProtoMember(5)]
        public DataflowSourceDestination Source { get; set; }
        
        [XmlElement("destination")]
        [ProtoMember(6)]
        public DataflowSourceDestination Destination { get; set; }

        internal bool IsDataClassification()
        {
            return Name == null
                   && Description == null
                   && Governance == null
                   && Source == null
                   && Destination == null;
        }
    }
}
