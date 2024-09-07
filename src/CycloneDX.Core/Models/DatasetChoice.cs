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
    public class DatasetChoice : IEquatable<DatasetChoice>
    {
        [XmlElement("dataset")]
        [ProtoMember(1)]
        public Data DataSet { get; set; }

        [XmlElement("ref")]
        [ProtoMember(2)]
        public string Ref { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DatasetChoice);
        }

        public bool Equals(DatasetChoice obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.DataSet, obj.DataSet) ||
                this.DataSet.Equals(obj.DataSet)) &&
                (object.ReferenceEquals(this.Ref, obj.Ref) ||
                this.Ref.Equals(obj.Ref, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
