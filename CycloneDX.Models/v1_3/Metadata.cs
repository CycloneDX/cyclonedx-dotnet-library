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

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class Metadata
    {
        [XmlElement("timestamp")]
        [ProtoMember(1)]
        public DateTime? Timestamp { get; set; } = null;
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }

        [XmlArray("tools")]
        [XmlArrayItem("tool")]
        [ProtoMember(2)]
        public List<Tool> Tools { get; set; }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        [ProtoMember(3)]
        public List<OrganizationalContact> Authors { get; set; }

        [XmlElement("component")]
        [ProtoMember(4)]
        public Component Component { get; set; }

        [XmlElement("manufacture")]
        [ProtoMember(5)]
        public OrganizationalEntity Manufacture { get; set; }

        [XmlElement("supplier")]
        [ProtoMember(6)]
        public OrganizationalEntity Supplier { get; set; }

        public Metadata() {}
        
        public Metadata(v1_2.Metadata metadata)
        {
            Timestamp = metadata.Timestamp;
            if (metadata.Tools != null)
            {
                Tools = new List<Tool>();
                foreach(var tool in metadata.Tools)
                {
                    Tools.Add(new Tool(tool));
                }
            }
            if (metadata.Authors != null)
            {
                Authors = new List<OrganizationalContact>();
                foreach(var author in metadata.Authors)
                {
                    Authors.Add(new OrganizationalContact(author));
                }
            }
            if (metadata.Component != null)
                Component = new Component(metadata.Component);
            if (metadata.Manufacture != null)
                Manufacture = new OrganizationalEntity(metadata.Manufacture);
            if (metadata.Supplier != null)
                Supplier = new OrganizationalEntity(metadata.Supplier);
        }
    }
}
