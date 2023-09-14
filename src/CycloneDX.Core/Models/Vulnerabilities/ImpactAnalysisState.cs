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
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.Vulnerabilities
{
    [ProtoContract]
    public enum ImpactAnalysisState
    {
        // to make working with protobuf easier
        Null,
        [XmlEnum(Name = "resolved")]
        Resolved,
        [XmlEnum(Name = "resolved_with_pedigree")]
        Resolved_With_Pedigree,
        [XmlEnum(Name = "exploitable")]
        Exploitable,
        [XmlEnum(Name = "in_triage")]
        In_Triage,
        [XmlEnum(Name = "false_positive")]
        False_Positive,
        [XmlEnum(Name = "not_affected")]
        Not_Affected,
    }
}
