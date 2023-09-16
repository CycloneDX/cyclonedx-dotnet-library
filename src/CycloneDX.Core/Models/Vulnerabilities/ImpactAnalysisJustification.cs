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
        Code_Not_Present,
        [XmlEnum(Name = "code_not_reachable")]
        Code_Not_Reachable,
        [XmlEnum(Name = "requires_configuration")]
        Requires_Configuration,
        [XmlEnum(Name = "requires_dependency")]
        Requires_Dependency,
        [XmlEnum(Name = "requires_environment")]
        Requires_Environment,
        [XmlEnum(Name = "protected_by_compiler")]
        Protected_By_Compiler,
        [XmlEnum(Name = "protected_at_runtime")]
        Protected_At_Runtime,
        [XmlEnum(Name = "protected_at_perimeter")]
        Protected_At_Perimeter,
        [XmlEnum(Name = "protected_by_mitigating_control")]
        Protected_By_Mitigating_Control,
    }
}
