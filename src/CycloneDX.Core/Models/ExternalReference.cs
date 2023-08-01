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
            [XmlEnum(Name = "security-contact")]
            SecurityContact,
            [XmlEnum(Name = "attestation")]
            Attestation,
            [XmlEnum(Name = "threat-model")]
            ThreatModel,
            [XmlEnum(Name = "adversary-model")]
            AdversaryModel,
            [XmlEnum(Name = "risk-assessment")]
            RiskAssessment,
            [XmlEnum(Name = "distribution-intake")]
            DistributionIntake,
            [XmlEnum(Name = "vulnerability-assertion")]
            VulnerabilityAssertion,
            [XmlEnum(Name = "exploitability-statement")]
            ExploitabilityStatement,
            [XmlEnum(Name = "pentest-report")]
            PentestReport,
            [XmlEnum(Name = "static-analysis-report")]
            StaticAnalysisReport,
            [XmlEnum(Name = "dynamic-analysis-report")]
            DynamicAnalysisReport,
            [XmlEnum(Name = "runtime-analysis-report")]
            RuntimeAnalysisReport,
            [XmlEnum(Name = "component-analysis-report")]
            ComponentAnalysisReport,
            [XmlEnum(Name = "maturity-report")]
            MaturityReport,
            [XmlEnum(Name = "certification-report")]
            CertificationReport,
            [XmlEnum(Name = "quality-metrics")]
            QualityMetrics,
            [XmlEnum(Name = "codified-infrastructure")]
            CodifiedInfrastructure,
            [XmlEnum(Name = "model-card")]
            ModelCard,
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
            ReleaseNotes,
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
