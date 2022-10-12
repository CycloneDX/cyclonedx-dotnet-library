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

namespace CycloneDX.Spdx.Models.v2_2
{
    public class ExtractedLicensingInfo
    {
        /// <summary>
        /// A human readable short form license identifier for a license. The license ID is iether on the standard license oist or the form "LicenseRef-"[idString] where [idString] is a unique string containing letters, numbers, ".", "-" or "+".
        /// </summary>
        [XmlElement("licenseId")]
        public string LicenseId { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Verbatim license or licensing notice text that was discovered.
        /// </summary>
        [XmlElement("extractedText")]
        public string ExtractedText { get; set; }

        /// <summary>
        /// Identify name of this SpdxElement.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Cross Reference Detail for a license SeeAlso URL
        /// </summary>
        [XmlElement("crossRefs")]
        public List<CrossRef> CrossRefs { get; set; }

        [XmlElement("seeAlsos")]
        public List<string> SeeAlsos { get; set; }
    }
}
