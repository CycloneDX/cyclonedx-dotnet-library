// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class DataClassification
    {
        [XmlAttribute("flow")]
        [ProtoMember(1, IsRequired=true)]
        public DataFlow Flow { get; set; }

        [XmlText]
        [ProtoMember(2)]
        public string Classification { get; set; }

        public DataClassification() {}

        public DataClassification(v1_2.DataClassification dataClassification)
        {
            Flow = (DataFlow)((int)dataClassification.Flow + 1);
            Classification = dataClassification.Classification;
        }
    }
}
