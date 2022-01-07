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
    public enum ImpactAnalysisJustification
    {
        // to make working with protobuf easier
        Null,
        [XmlEnum(Name = "code_not_present")]
        CodeNotPresent,
        [XmlEnum(Name = "code_not_reachable")]
        CodeNotReachable,
        [XmlEnum(Name = "requires_configuration")]
        RequiresConfiguration,
        [XmlEnum(Name = "requires_dependency")]
        RequiresDependency,
        [XmlEnum(Name = "requires_environment")]
        RequiresEnvironment,
        [XmlEnum(Name = "protected_by_compiler")]
        ProtectedByCompiler,
        [XmlEnum(Name = "protected_at_runtime")]
        ProtectedAtRuntime,
        [XmlEnum(Name = "protected_at_perimeter")]
        ProtectedAtPerimeter,
        [XmlEnum(Name = "protected_by_mitigating_control")]
        ProtectedByMitigatingControl,
    }
}
