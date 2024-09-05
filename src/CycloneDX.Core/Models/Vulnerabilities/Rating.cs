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
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.Vulnerabilities
{
    [ProtoContract]
    public class Rating : ICloneable
    {
        [XmlElement("source")]
        [ProtoMember(1)]
        public Source Source { get; set; }

        [XmlElement("score")]
        [ProtoMember(2)]
        public decimal Score { get; set; }

        [XmlElement("severity")]
        [ProtoMember(3)]
        public Severity Severity { get; set; }

        [XmlElement("method")]
        [ProtoMember(4)]
        public ScoreMethod Method { get; set; }

        [XmlElement("vector")]
        [ProtoMember(5)]
        public string Vector { get; set; }

        [XmlElement("justification")]
        [ProtoMember(6)]
        public string Justification { get; set; }

        public object Clone()
        {
            return new Rating()
            {
                Justification = this.Justification,
                Method = this.Method,
                Score = this.Score,
                Severity = this.Severity,
                Source = (Source)this.Source.Clone(),
                Vector = this.Vector
            };
        }
    }
}
