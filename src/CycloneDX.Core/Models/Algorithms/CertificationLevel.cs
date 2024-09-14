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

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    public enum CertificationLevel
    {
        Null,
        [XmlEnum("none")]
        None,
        [XmlEnum("fips140-1-l1")]
        FIPS140_1_L1,
        [XmlEnum("fips140-1-l2")]
        FIPS140_1_L2,
        [XmlEnum("fips140-1-l3")]
        FIPS140_1_L3,
        [XmlEnum("fips140-1-l4")]
        FIPS140_1_L4,
        [XmlEnum("fips140-2-l1")]
        FIPS140_2_L1,
        [XmlEnum("fips140-2-l2")]
        FIPS140_2_L2,
        [XmlEnum("fips140-2-l3")]
        FIPS140_2_L3,
        [XmlEnum("fips140-2-l4")]
        FIPS140_2_L4,
        [XmlEnum("fips140-3-l1")]
        FIPS140_3_L1,
        [XmlEnum("fips140-3-l2")]
        FIPS140_3_L2,
        [XmlEnum("fips140-3-l3")]
        FIPS140_3_L3,
        [XmlEnum("fips140-3-l4")]
        FIPS140_3_L4,
        [XmlEnum("cc-eal1")]
        CC_EAL1,
        [XmlEnum("cc-eal1+")]
        [EnumMember(Value = "cc-eal1+")]
        CC_EAL1plus,
        [XmlEnum("cc-eal2")]
        CC_EAL2,
        [XmlEnum("cc-eal2+")]
        [EnumMember(Value = "cc-eal2+")]
        CC_EAL2plus,
        [XmlEnum("cc-eal3")]
        CC_EAL3,
        [XmlEnum("cc-eal3+")]
        [EnumMember(Value = "cc-eal3+")]
        CC_EAL3plus,
        [XmlEnum("cc-eal4")]
        CC_EAL4,
        [XmlEnum("cc-eal4+")]
        [EnumMember(Value = "cc-eal4+")]
        CC_EAL4plus,
        [XmlEnum("cc-eal5")]
        CC_EAL5,
        [XmlEnum("cc-eal5+")]
        [EnumMember(Value = "cc-eal5+")]
        CC_EAL5plus,
        [XmlEnum("cc-eal6")]
        CC_EAL6,
        [XmlEnum("cc-eal6+")]
        [EnumMember(Value = "cc-eal6+")]
        CC_EAL6plus,
        [XmlEnum("cc-eal7")]
        CC_EAL7,
        [XmlEnum("cc-eal7+")]
        [EnumMember(Value = "cc-eal7+")]
        CC_EAL7plus,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown

    }
}
