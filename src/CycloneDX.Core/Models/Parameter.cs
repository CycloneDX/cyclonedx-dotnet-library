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

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Parameter : IEquatable<Parameter>
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("value")]
        [ProtoMember(2)]
        public string Value { get; set; }
        
        [XmlElement("data-type")]
        [ProtoMember(3)]
        public string DataType { get; set; }


        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public bool Equals(Parameter obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.DataType, obj.DataType) ||
                this.DataType.Equals(obj.DataType, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Name, obj.Name) ||
                this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Value, obj.Value) ||
                this.Value.Equals(obj.Value, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
