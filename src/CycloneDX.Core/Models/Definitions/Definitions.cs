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
using System.Linq;
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

        [XmlElement("patents"), JsonIgnore]
        public PatentsType Patents_XML { get; set; }
        public bool ShouldSerializePatents_XML() { return Patents_XML?.XmlItems?.Count > 0; }

        [JsonPropertyName("patents"), XmlIgnore]
        [ProtoMember(2)]
        public List<PatentOrFamily> Patents
        {
            get => Patents_XML?.Items;
            set
            {
                if (Patents_XML == null)
                {
                    Patents_XML = new PatentsType();
                }
                Patents_XML.Items = value;
            }
        }
        public bool ShouldSerializePatents() { return Patents?.Count > 0; }
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

    public class PatentsType
    {
        [XmlElement("patent", typeof(Patent))]
        [XmlElement("patentFamily", typeof(PatentFamily))]
        public List<object> XmlItems { get; set; }

        [XmlIgnore]
        public List<PatentOrFamily> Items
        {
            get
            {
                if (XmlItems == null) return null;
                var result = new List<PatentOrFamily>();
                foreach (var item in XmlItems)
                {
                    if (item is Patent patent)
                        result.Add(new PatentOrFamily { Patent = patent });
                    else if (item is PatentFamily family)
                        result.Add(new PatentOrFamily { PatentFamily = family });
                }
                return result;
            }
            set
            {
                if (value == null)
                {
                    XmlItems = null;
                    return;
                }
                XmlItems = new List<object>();
                foreach (var item in value)
                {
                    if (item.Patent != null)
                        XmlItems.Add(item.Patent);
                    else if (item.PatentFamily != null)
                        XmlItems.Add(item.PatentFamily);
                }
            }
        }
    }

    [ProtoContract]
    public class PatentOrFamily
    {
        [ProtoMember(1)]
        public Patent Patent { get; set; }

        [ProtoMember(2)]
        public PatentFamily PatentFamily { get; set; }
    }
}
