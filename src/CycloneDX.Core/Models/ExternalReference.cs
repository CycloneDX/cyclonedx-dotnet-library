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

namespace CycloneDX.Models
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
            Issue_Tracker,
            [XmlEnum(Name = "website")]
            Website,
            [XmlEnum(Name = "advisories")]
            Advisories,
            [XmlEnum(Name = "bom")]
            Bom,
            [XmlEnum(Name = "mailing-list")]
            Mailing_List,
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
            Build_Meta,
            [XmlEnum(Name = "build-system")]
            Build_System,
            [XmlEnum(Name = "security-contact")]
            Security_Contact,
            [XmlEnum(Name = "attestation")]
            Attestation,
            [XmlEnum(Name = "threat-model")]
            Threat_Model,
            [XmlEnum(Name = "adversary-model")]
            Adversary_Model,
            [XmlEnum(Name = "risk-assessment")]
            Risk_Assessment,
            [XmlEnum(Name = "distribution-intake")]
            Distribution_Intake,
            [XmlEnum(Name = "vulnerability-assertion")]
            Vulnerability_Assertion,
            [XmlEnum(Name = "exploitability-statement")]
            Exploitability_Statement,
            [XmlEnum(Name = "pentest-report")]
            Pentest_Report,
            [XmlEnum(Name = "static-analysis-report")]
            Static_Analysis_Report,
            [XmlEnum(Name = "dynamic-analysis-report")]
            Dynamic_Analysis_Report,
            [XmlEnum(Name = "runtime-analysis-report")]
            Runtime_Analysis_Report,
            [XmlEnum(Name = "component-analysis-report")]
            Component_Analysis_Report,
            [XmlEnum(Name = "maturity-report")]
            Maturity_Report,
            [XmlEnum(Name = "certification-report")]
            Certification_Report,
            [XmlEnum(Name = "quality-metrics")]
            Quality_Metrics,
            [XmlEnum(Name = "codified-infrastructure")]
            Codified_Infrastructure,
            [XmlEnum(Name = "model-card")]
            Model_Card,
            [XmlEnum(Name = "poam")]
            Poam,
            [XmlEnum(Name = "log")]
            Log,
            [XmlEnum(Name = "configuration")]
            Configuration,
            [XmlEnum(Name = "evidence")]
            Evidence,
            [XmlEnum(Name = "formulation")]
            Formulation,
            [XmlEnum(Name = "release-notes")]
            Release_Notes,
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
    }
}
