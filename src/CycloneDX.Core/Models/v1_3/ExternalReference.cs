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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [ProtoContract]
    public class ExternalReference
    {
        [ProtoContract]
        public enum ExternalReferenceType
        {
            [XmlEnum(Name = "other")]
            Other,
            [XmlEnum(Name = "vcs")]
            Vcs,
            [XmlEnum(Name = "issue-tracker")]
            IssueTracker,
            [XmlEnum(Name = "website")]
            Website,
            [XmlEnum(Name = "advisories")]
            Advisories,
            [XmlEnum(Name = "bom")]
            Bom,
            [XmlEnum(Name = "mailing-list")]
            MailingList,
            [XmlEnum(Name = "social")]
            Social,
            [XmlEnum(Name = "chat")]
            Chat,
            [XmlEnum(Name = "documentation")]
            Documentation,
            [XmlEnum(Name = "support")]
            Support,
            [XmlEnum(Name = "distribution")]
            Distribution,
            [XmlEnum(Name = "license")]
            License,
            [XmlEnum(Name = "build-meta")]
            BuildMeta,
            [XmlEnum(Name = "build-system")]
            BuildSystem
        }

        [XmlElement("url")]
        [ProtoMember(2)]
        public string Url { get; set; }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public ExternalReferenceType Type { get; set; }

        [XmlElement("comment")]
        [ProtoMember(3)]
        public string Comment { get; set; }

        [XmlArray("hashes")]
        [ProtoMember(4)]
        public List<Hash> Hashes { get; set; }
        public bool ShouldSerializeHashes() { return Hashes?.Count > 0; }

        public ExternalReference() {}

        public ExternalReference(v1_2.ExternalReference externalReference)
        {
            Url = externalReference.Url;
            if (externalReference.Type == v1_2.ExternalReference.ExternalReferenceType.Other)
            {
                Type = ExternalReferenceType.Other;
            }
            else
            {
                Type = (ExternalReferenceType)((int)externalReference.Type+1);
            }
            Comment = externalReference.Comment;
        }
    }
}
