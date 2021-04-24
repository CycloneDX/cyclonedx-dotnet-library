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
    public class OrganizationalEntity
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlElement("url")]
        [ProtoMember(2)]
        public List<string> Url { get; set; }

        [XmlElement("contact")]
        [ProtoMember(3)]
        public List<OrganizationalContact> Contact { get; set; }

        public OrganizationalEntity() {}

        public OrganizationalEntity(v1_2.OrganizationalEntity organizationalEntity)
        {
            Name = organizationalEntity.Name;
            Url = organizationalEntity.Url;
            if (organizationalEntity.Contact != null)
            {
                Contact = new List<OrganizationalContact>();
                foreach (var contact in organizationalEntity.Contact)
                {
                    Contact.Add(new OrganizationalContact(contact));
                }
            }
        }
    }
}
