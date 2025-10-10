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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class PackageVerificationCode
    {
        /// <summary>
        /// A file that was excluded when calculating the package verification code. This is usually a file containing SPDX data regarding the package. If a package contains more than one SPDX file all SPDX files must be excluded from the package verification code. If this is not done it would be impossible to correctly calculate the verification codes in both files.
        /// </summary>
        [XmlElement("packageVerificationCodeExcludedFiles")]
        public List<string> PackageVerificationCodeExcludedFiles { get; set; }

        /// <summary>
        /// The actual package verification code as a hex encoded value.
        /// </summary>
        [XmlElement("packageVerificationCodeValue")]
        public string PackageVerificationCodeValue { get; set; }
    }
}
