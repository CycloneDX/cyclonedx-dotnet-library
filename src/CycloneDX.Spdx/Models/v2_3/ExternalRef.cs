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
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class ExternalRef
    {
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Category for the external reference
        /// </summary>
        [XmlElement("referenceCategory")]
        public ExternalRefCategory ReferenceCategory { get; set; }

        /// <summary>
        /// The unique string with no spaces necessary to access the package-specific information, metadata, or content within the target location. The format of the locator is subject to constraints defined by the &lt;type&gt;.
        /// </summary>
        [XmlElement("referenceLocator")]
        public string ReferenceLocator { get; set; }

        /// <summary>
        /// Type of the external reference. These are definined in an appendix in the SPDX specification.
        /// </summary>
        [XmlElement("referenceType")]
        public string ReferenceType { get; set; }
    }
}
