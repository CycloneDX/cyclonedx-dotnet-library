﻿// This file is part of CycloneDX Library for .NET
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
    public class AnnotatorChoice : ICloneable
    {
        [XmlElement("organization")]
        [ProtoMember(1)]
        public OrganizationalEntity Organization { get; set; }

        [XmlElement("individual")]
        [ProtoMember(2)]
        public OrganizationalContact Individual { get; set; }

        [XmlElement("component")]
        [ProtoMember(3)]
        public Component Component { get; set; }

        [XmlElement("service")]
        [ProtoMember(4)]
        public Service Service { get; set; }

        public object Clone()
        {
            return new AnnotatorChoice()
            {
                Component = (Component)this.Component.Clone(),
                Individual = (OrganizationalContact)this.Individual.Clone(),
                Organization = (OrganizationalEntity)this.Organization.Clone(),
                Service = (Service)this.Service.Clone(),
            };
        }
    }
}
