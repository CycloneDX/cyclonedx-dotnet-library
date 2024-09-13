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

using CycloneDX.Models;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    [ProtoContract]
    public class AlgorithmProperties
    {
        #region primitive
        [XmlIgnore]
        [ProtoMember(1)]
        public Primitive? Primitive { get; set; }
        [XmlElement("primitive"), JsonIgnore]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Primitive Primitive_XML
        {
            get { return Primitive.Value; }
            set { Primitive = value; }
        }        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePrimitive_XML()
        {
            return Primitive.HasValue;
        }
        #endregion

        [XmlElement("parameterSetIdentifier")]
        [ProtoMember(2)]
        public string ParameterSetIdentifier { get; set; }

        [XmlElement("curve")]
        [ProtoMember(3)]
        public string Curve { get; set; }

        [XmlElement("executionEnvironment")]
        [ProtoMember(4)]
        public ExecutionEnvironment? ExecutionEnvironment { get; set; }

        [XmlElement("implementationPlatform")]
        [ProtoMember(5)]
        public ImplementationPlatform? ImplementationPlatform { get; set; }

        [XmlElement("certificationLevel")]
        public List<CertificationLevel> CertificationLevel { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(6)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<string> CertificationLevel_Protobuf
        {
            get
            {
                if (CertificationLevel == null)
                {
                    return null;
                }
                return CertificationLevel.Select((certificationLevel) =>
                {
                    return CertificationLevelExtensions.CertificationLevelToString(certificationLevel);
                }).ToList();
            }
            set
            {
                if (value == null)
                {
                    CertificationLevel = null;
                    return;
                }
                CertificationLevel = value.Select((certificationLevel) =>
                {
                    return CertificationLevelExtensions.CertificationLevelFromString(certificationLevel);
                }).ToList();
            }
        }

        [ProtoMember(7)]
        [XmlElement("mode")]
        public AlgorithmMode? Mode { get; set; }

        #region Padding
        [XmlIgnore]
        [ProtoMember(8)]
        public PaddingScheme? Padding { get; set; }
        [XmlElement("padding"), JsonIgnore]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public PaddingScheme Padding_XML
        {
            get { return Padding.Value; }
            set { Padding = value; }
        }        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePadding_XML() => Padding.HasValue;
        #endregion Padding

        [XmlArray("cryptoFunctions")]
        [XmlArrayItem("cryptoFunction")]
        [ProtoMember(9)]
        public List<CryptoFunction> CryptoFunctions { get; set; }

        [XmlElement("classicalSecurityLevel")]
        [ProtoMember(10)]
        public int ClassicalSecurityLevel { get; set; }

        [XmlElement("nistQuantumSecurityLevel")]
        [ProtoMember(11)]
        public int NistQuantumSecurityLevel { get; set; }
        
    }
}
