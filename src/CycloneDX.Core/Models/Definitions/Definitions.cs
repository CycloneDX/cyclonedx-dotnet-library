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
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Definitions
    {
        [XmlElement("standards"), JsonIgnore]
        public StandardsType Standards_XML { get; set; }

        [JsonPropertyName("standards"), XmlIgnore]
        [ProtoMember(1)]
        public List<Standard> Standards
        {
            get => Standards_XML?.Standards;
            set
            {
                if (Standards_XML == null)
                {
                    Standards_XML = new StandardsType();
                }
                Standards_XML.Standards = value;
            }
        }
    }

    public class StandardsType
    {
        [XmlElement("standard")]
        public List<Standard> Standards { get; set; }

        [XmlAnyElement]
        public List<System.Xml.XmlElement> Any { get; set; }

        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }
}
