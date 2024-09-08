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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("graphics")]
    [ProtoContract]
    public class GraphicsCollection : IEquatable<GraphicsCollection>
    {
        [ProtoContract]
        public class Graphic : IEquatable<Graphic> 
        {
            [XmlElement("name")]
            [ProtoMember(1)]
            public string Name { get; set; }
            
            [XmlElement("image")]
            [ProtoMember(2)]
            public AttachedText Image { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Graphic);
            }

            public bool Equals(Graphic obj)
            {
                return obj != null &&
                    (object.ReferenceEquals(this.Image, obj.Image) ||
                    this.Image.Equals(obj.Image)) &&
                    (object.ReferenceEquals(this.Name, obj.Name) ||
                    this.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [XmlElement("description")]
        [ProtoMember(1)]
        public string Description { get; set; }

        [XmlArray("collection")]
        [XmlArrayItem("graphic")]
        [ProtoMember(2)]
        public List<Graphic> Collection { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GraphicsCollection);
        }

        public bool Equals(GraphicsCollection obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Collection, obj.Collection) ||
                this.Collection.Equals(obj.Collection)) &&
                (object.ReferenceEquals(this.Description, obj.Description) ||
                this.Description.Equals(obj.Description, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}