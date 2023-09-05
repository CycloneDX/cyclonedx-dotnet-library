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
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("evidence-methods")]
    [ProtoContract]
    public class EvidenceMethods
    {
        [ProtoContract]
        public enum EvidenceTechnique
        {
            [XmlEnum(Name = "source-code-analysis")]
            Source_Code_Analysis,
            [XmlEnum(Name = "binary-analysis")]
            Binary_Analysis,
            [XmlEnum(Name = "manifest-analysis")]
            Manifest_Analysis,
            [XmlEnum(Name = "ast-fingerprint")]
            Ast_Fingerprint,
            [XmlEnum(Name = "hash-comparison")]
            Hash_Comparison,
            [XmlEnum(Name = "instrumentation")]
            Instrumentation,
            [XmlEnum(Name = "dynamic-analysis")]
            Dynamic_Analysis,
            [XmlEnum(Name = "filename")]
            Filename,
            [XmlEnum(Name = "attestation")]
            Attestation,
            [XmlEnum(Name = "other")]
            Other,
        }

        [XmlElement("technique")]
        [ProtoMember(1)]
        public EvidenceTechnique Technique { get; set; }

        [XmlElement("confidence")]
        [ProtoMember(2)]
        public float Confidence { get; set; }
        
        [XmlElement("value")]
        [ProtoMember(3)]
        public string Value { get; set; }
    }
}