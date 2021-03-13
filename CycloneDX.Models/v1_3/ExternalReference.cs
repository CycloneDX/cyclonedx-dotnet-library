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

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace CycloneDX.Models.v1_3
{
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    public class ExternalReference
    {
        public enum ExternalReferenceType
        {
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
            BuildSystem,
            [XmlEnum(Name = "other")]
            Other
        }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlAttribute("type")]
        public ExternalReferenceType Type { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        public ExternalReference() {}

        public ExternalReference(v1_1.ExternalReference externalReference)
        {
            Url = externalReference.Url;
            Type = (ExternalReferenceType)(int)externalReference.Type;
            Comment = externalReference.Comment;
        }

        public ExternalReference(v1_2.ExternalReference externalReference)
        {
            Url = externalReference.Url;
            Type = (ExternalReferenceType)(int)externalReference.Type;
            Comment = externalReference.Comment;
        }
    }
}
