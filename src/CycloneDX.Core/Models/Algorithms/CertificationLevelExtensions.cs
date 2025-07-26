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

namespace CycloneDX.Core.Models
{
    public static class CertificationLevelExtensions {
        public static CertificationLevel CertificationLevelFromString(string value)
        {
            switch (value)
            {
                case "none": return CertificationLevel.None;
                case "fips140-1-l1": return CertificationLevel.FIPS140_1_L1;
                case "fips140-1-l2": return CertificationLevel.FIPS140_1_L2;
                case "fips140-1-l3": return CertificationLevel.FIPS140_1_L3;
                case "fips140-1-l4": return CertificationLevel.FIPS140_1_L4;
                case "fips140-2-l1": return CertificationLevel.FIPS140_2_L1;
                case "fips140-2-l2": return CertificationLevel.FIPS140_2_L2;
                case "fips140-2-l3": return CertificationLevel.FIPS140_2_L3;
                case "fips140-2-l4": return CertificationLevel.FIPS140_2_L4;
                case "fips140-3-l1": return CertificationLevel.FIPS140_3_L1;
                case "fips140-3-l2": return CertificationLevel.FIPS140_3_L2;
                case "fips140-3-l3": return CertificationLevel.FIPS140_3_L3;
                case "fips140-3-l4": return CertificationLevel.FIPS140_3_L4;
                case "cc-eal1": return CertificationLevel.CC_EAL1;
                case "cc-eal1+": return CertificationLevel.CC_EAL1plus;
                case "cc-eal2": return CertificationLevel.CC_EAL2;
                case "cc-eal2+": return CertificationLevel.CC_EAL2plus;
                case "cc-eal3": return CertificationLevel.CC_EAL3;
                case "cc-eal3+": return CertificationLevel.CC_EAL3plus;
                case "cc-eal4": return CertificationLevel.CC_EAL4;
                case "cc-eal4+": return CertificationLevel.CC_EAL4plus;
                case "cc-eal5": return CertificationLevel.CC_EAL5;
                case "cc-eal5+": return CertificationLevel.CC_EAL5plus;
                case "cc-eal6": return CertificationLevel.CC_EAL6;
                case "cc-eal6+": return CertificationLevel.CC_EAL6plus;
                case "cc-eal7": return CertificationLevel.CC_EAL7;
                case "cc-eal7+": return CertificationLevel.CC_EAL7plus;
                case "other": return CertificationLevel.Other;
                case "unknown": return CertificationLevel.Unknown;
                default: return CertificationLevel.Null;
            }
        }

        public static string CertificationLevelToString(CertificationLevel level)
        {
            switch (level)
            {
                case CertificationLevel.None: return "none";
                case CertificationLevel.FIPS140_1_L1: return "fips140-1-l1";
                case CertificationLevel.FIPS140_1_L2: return "fips140-1-l2";
                case CertificationLevel.FIPS140_1_L3: return "fips140-1-l3";
                case CertificationLevel.FIPS140_1_L4: return "fips140-1-l4";
                case CertificationLevel.FIPS140_2_L1: return "fips140-2-l1";
                case CertificationLevel.FIPS140_2_L2: return "fips140-2-l2";
                case CertificationLevel.FIPS140_2_L3: return "fips140-2-l3";
                case CertificationLevel.FIPS140_2_L4: return "fips140-2-l4";
                case CertificationLevel.FIPS140_3_L1: return "fips140-3-l1";
                case CertificationLevel.FIPS140_3_L2: return "fips140-3-l2";
                case CertificationLevel.FIPS140_3_L3: return "fips140-3-l3";
                case CertificationLevel.FIPS140_3_L4: return "fips140-3-l4";
                case CertificationLevel.CC_EAL1: return "cc-eal1";
                case CertificationLevel.CC_EAL1plus: return "cc-eal1+";
                case CertificationLevel.CC_EAL2: return "cc-eal2";
                case CertificationLevel.CC_EAL2plus: return "cc-eal2+";
                case CertificationLevel.CC_EAL3: return "cc-eal3";
                case CertificationLevel.CC_EAL3plus: return "cc-eal3+";
                case CertificationLevel.CC_EAL4: return "cc-eal4";
                case CertificationLevel.CC_EAL4plus: return "cc-eal4+";
                case CertificationLevel.CC_EAL5: return "cc-eal5";
                case CertificationLevel.CC_EAL5plus: return "cc-eal5+";
                case CertificationLevel.CC_EAL6: return "cc-eal6";
                case CertificationLevel.CC_EAL6plus: return "cc-eal6+";
                case CertificationLevel.CC_EAL7: return "cc-eal7";
                case CertificationLevel.CC_EAL7plus: return "cc-eal7+";
                case CertificationLevel.Other: return "other";
                case CertificationLevel.Unknown: return "unknown";
                default: return "null";
            }
        }
    }
}
