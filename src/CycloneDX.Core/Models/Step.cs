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
    [XmlType("step")]
    [ProtoContract]
    public class Step : ICloneable
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        [XmlArray("commands")]
        [XmlArrayItem("command")]
        [ProtoMember(3)]
        public List<Command> Commands { get; set; }
        
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(4)]
        public List<Property> Properties { get; set; }
        
        public bool ShouldSerializeCommands() { return Commands?.Count > 0; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        public object Clone()
        {
            return new Step()
            {
                Commands = this.Commands.Select(x => (Command)x.Clone()).ToList(),
                Description = this.Description,
                Name = this.Name,
                Properties = this.Properties.Select(x => (Property)x.Clone()).ToList()
            };
        }
    }
}