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

namespace CycloneDX.Spdx.Models.v2_2
{
    public class CreationInfo
    {
        public string Comment { get; set; }

        /// <summary>
        /// Identify when the SPDX file was originally created. The date is to be specified according to combined date and time in UTC format as specified in ISO 8601 standard. This field is distinct from the fields in section 8, which involves the addition of information during a subsequent review.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Identify who (or what, in the case of a tool) created the SPDX file. If the SPDX file was created by an individual, indicate the person's name. If the SPDX file was created on behalf of a company or organization, indicate the entity name. If the SPDX file was created using a software tool, indicate the name and version for that tool. If multiple participants or tools were involved, use multiple instances of this field. Person name or organization name may be designated as “anonymous” if appropriate.
        /// </summary>
        public List<string> Creators { get; set; }

        /// <summary>
        /// An optional field for creators of the SPDX file to provide the version of the SPDX License List used when the SPDX file was created.
        /// </summary>
        public string LicenseListVersion { get; set; }
    }
}
