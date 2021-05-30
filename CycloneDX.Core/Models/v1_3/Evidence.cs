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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [XmlType("evidence")]
    [ProtoContract]
    public class Evidence
    {
        [XmlElement("licenses")]
        [ProtoMember(1)]
        public List<LicenseChoice> Licenses { get; set; }

        [XmlArray("copyright")]
        [XmlArrayItem("text")]
        [ProtoMember(2)]
        public List<EvidenceCopyright> Copyright { get; set; }
        
        public Evidence() {}

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}