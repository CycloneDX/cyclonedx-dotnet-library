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
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("callstack")]
    [ProtoContract]
    public class Callstack : ICloneable
    {
        [XmlType("frame")]
        [ProtoContract]
        public class Frame : ICloneable
        {
            [XmlElement("package")]
            [ProtoMember(1)]
            public string Package { get; set; }
            
            [XmlElement("module")]
            [ProtoMember(2)]
            public string Module { get; set; }

            [XmlElement("function")]
            [ProtoMember(3)]
            public string Function { get; set; }

            [XmlArray("parameters")]
            [XmlArrayItem("parameter")]
            [ProtoMember(4)]
            public List<string> Parameters { get; set; }

            [XmlElement("line")]
            [ProtoMember(5)]
            public int? Line { get; set; }

            [XmlElement("column")]
            [ProtoMember(6)]
            public int? Column { get; set; }

            [XmlElement("fullFilename")]
            [ProtoMember(7)]
            public string FullFilename { get; set; }

            public bool ShouldSerializeParameters() { return Parameters?.Count > 0; }

            public object Clone()
            {
                return new Frame()
                {
                    Column = this.Column,
                    FullFilename = this.FullFilename,
                    Function = this.Function,
                    Line = this.Line,
                    Module = this.Module,
                    Package = this.Package,
                    Parameters = this.Parameters.Select(x => x).ToList()
                };
            }
        }
        
        [XmlArray("frames")]
        [XmlArrayItem("frame")]
        [ProtoMember(1)]
        public List<Frame> Frames { get; set; }

        public object Clone()
        {
            return new Callstack()
            {
                Frames = this.Frames.Select(x => (Frame)x.Clone()).ToList()
            };
        }
    }
}