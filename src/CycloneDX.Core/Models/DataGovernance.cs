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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("data-governance")]
    [ProtoContract]
    public class DataGovernance : IEquatable<DataGovernance>
    {
        [XmlArray("custodians")]
        [XmlArrayItem("custodian")]
        [ProtoMember(1)]
        public List<OrganizationalEntityOrContact> Custodians { get; set; }
        public bool ShouldSerializeCustodians() { return Custodians?.Count > 0; }
        
        [XmlArray("stewards")]
        [XmlArrayItem("steward")]
        [ProtoMember(2)]
        public List<OrganizationalEntityOrContact> Stewards { get; set; }
        public bool ShouldSerializeStewards() { return Stewards?.Count > 0; }

        [XmlArray("owners")]
        [XmlArrayItem("owner")]
        [ProtoMember(3)]
        public List<OrganizationalEntityOrContact> Owners { get; set; }
        public bool ShouldSerializeOwners() { return Owners?.Count > 0; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DataGovernance);
        }

        public bool Equals(DataGovernance obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Custodians, obj.Custodians) ||
                this.Custodians.SequenceEqual(obj.Custodians)) &&
                (object.ReferenceEquals(this.Owners, obj.Owners) ||
                this.Owners.SequenceEqual(obj.Owners)) &&
                (object.ReferenceEquals(this.Stewards, obj.Stewards) ||
                this.Stewards.SequenceEqual(obj.Stewards));
        }
    }
}