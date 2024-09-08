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
    public class Commit : IEquatable<Commit>
    {
        [XmlElement("uid")]
        [ProtoMember(1)]
        public string Uid { get; set; }

        [XmlElement("url")]
        [ProtoMember(2)]
        public string Url { get; set; }
        
        [XmlElement("author")]
        [ProtoMember(3)]
        public IdentifiableAction Author { get; set; }

        [XmlElement("committer")]
        [ProtoMember(4)]
        public IdentifiableAction Committer { get; set; }
        
        [XmlElement("message")]
        [ProtoMember(5)]
        public string Message { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Commit);
        }

        public bool Equals(Commit obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Author, obj.Author) ||
                this.Author.Equals(obj.Author)) &&
                (object.ReferenceEquals(this.Committer, obj.Committer) ||
                this.Committer.Equals(obj.Committer)) &&
                (object.ReferenceEquals(this.Message, obj.Message) ||
                this.Message.Equals(obj.Message, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Uid, obj.Uid) ||
                this.Uid.Equals(obj.Uid, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Url, obj.Url) ||
                this.Url.Equals(obj.Url, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
