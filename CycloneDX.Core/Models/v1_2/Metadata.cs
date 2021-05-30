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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CycloneDX.Models.v1_2
{
    public class Metadata
    {
        private DateTime? _timestamp;
        [XmlElement("timestamp")]
        public DateTime? Timestamp
        {
            get => _timestamp;
            set
            {
                if (value == null)
                {
                    _timestamp = null;
                }
                else if (value.Value.Kind == DateTimeKind.Unspecified)
                {
                    _timestamp = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                }
                else if (value.Value.Kind == DateTimeKind.Local)
                {
                    _timestamp = value.Value.ToUniversalTime();
                }
                else
                {
                    _timestamp = value;
                }
            }
        }
        public bool ShouldSerializeTimestamp() { return Timestamp != null; }

        [XmlArray("tools")]
        [XmlArrayItem("tool")]
        public List<Tool> Tools { get; set; }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        public List<OrganizationalContact> Authors { get; set; }

        [XmlElement("component")]
        public Component Component { get; set; }

        [XmlElement("manufacture")]
        public OrganizationalEntity Manufacture { get; set; }

        [XmlElement("supplier")]
        public OrganizationalEntity Supplier { get; set; }

        public Metadata() {}
        
        public Metadata(v1_3.Metadata metadata)
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
