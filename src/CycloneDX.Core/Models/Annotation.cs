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

using static CycloneDX.SpecificationVersion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Annotation : BomEntity, IBomEntityWithRefType_String_BomRef
    // NOTE: *Not* IBomEntityWithRefLinkType_StringList due
    // to inlaid "subject" property type with dedicated class
    {
        [XmlType("subject")]
        public class XmlAnnotationSubject : BomEntity, IBomEntityWithRefLinkType_String_Ref
        {
            [XmlAttribute("ref")]
            public string Ref { get; set; }

            private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_AnyBomEntity =
            new Dictionary<PropertyInfo, ImmutableList<Type>>
            {
                { typeof(XmlAnnotationSubject).GetProperty("Ref", typeof(string)), RefLinkConstraints_AnyBomEntity }
            }.ToImmutableDictionary();

            public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
            {
                // TODO: switch/case for CDX spec newer than 1.5 where this type got introduced
                if (specificationVersion == v1_5)
                {
                    return RefLinkConstraints_StringRef_AnyBomEntity;
                }
                return null;
            }
        }
        
        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlIgnore]
        [JsonPropertyName("subjects")]
        [ProtoMember(2)]
        public List<string> Subjects
        {
            get
            {
                if (XmlSubjects == null) return null;
                var result = new List<string>();
                foreach (var subject in XmlSubjects) result.Add(subject.Ref);
                return result;
            }
            set
            {
                if (value == null)
                {
                    XmlSubjects = null;
                }
                else
                {
                    XmlSubjects = new List<XmlAnnotationSubject>();
                    foreach (var subject in value) XmlSubjects.Add(new XmlAnnotationSubject() { Ref = subject});
                }
            }
        }

        [JsonIgnore]
        [XmlArray("subjects")]
        [XmlArrayItem("subject")]
        public List<XmlAnnotationSubject> XmlSubjects { get; set; }

        [XmlElement("annotator")] [ProtoMember(3)]
        public AnnotatorChoice Annotator { get; set; }

        private DateTime? _timestamp;
        [XmlElement("timestamp")]
        [ProtoMember(4)]
        public DateTime? Timestamp
        { 
            get => _timestamp;
            set { _timestamp = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }
        
        [XmlElement("text")]
        [ProtoMember(5)]
        public string Text { get; set; }
    }
}
