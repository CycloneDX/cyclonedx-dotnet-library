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
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class Snippet
    {
        /// <summary>
        /// Uniquely identify any element in an SPDX document which may be referenced by other elements.
        /// </summary>
        [JsonPropertyName("SPDXID")]
        public string SPDXID { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The text of copyright declarations recited in the Package or File.
        /// </summary>
        [XmlElement("copyrightText")]
        public string CopyrightText { get; set; }

        /// <summary>
        /// The licenseComments property allows the preparer of the SPDX document to describe why the licensing in spdx:licenseConcluded was chosen.
        /// </summary>
        [XmlElement("licenseComments")]
        public string LicenseComments { get; set; }

        /// <summary>
        /// License expression for licenseConcluded.  The licensing that the preparer of this SPDX document has concluded, based on the evidence, actually applies to the package.
        /// </summary>
        [XmlElement("licenseConcluded")]
        public string LicenseConcluded { get; set; }

        /// <summary>
        /// Licensing information that was discovered directly in the subject snippet. This is also considered a declared license for the snippet.
        /// </summary>
        [XmlElement("licenseInfoInSnippets")]
        public List<string> LicenseInfoInSnippets { get; set; }

        /// <summary>
        /// This field provides a place for the SPDX data creator to record acknowledgements that may be required to be communicated in some contexts. This is not meant to include theactual complete license text (see licenseConculded and licenseDeclared), and may or may not include copyright notices (see also copyrightText). The SPDX data creator may use this field to record other acknowledgements, such as particular clauses from license texts, which may be necessary or desirable to reproduce.
        /// </summary>
        [XmlElement("attributionTexts")]
        public List<string> AttributionTexts { get; set; }

        /// <summary>
        /// Identify name of this SpdxElement.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// This field defines the byte range in the original host file (in X.2) that the snippet information applies to
        /// </summary>
        [XmlElement("ranges")]
        public List<Range> Ranges { get; set; }

        /// <summary>
        /// SPDX ID for File.  File containing the SPDX element (e.g. the file contaning a snippet).
        /// </summary>
        [XmlElement("snippetFromFile")]
        public string SnippetFromFile { get; set; }

        /// <summary>
        /// Provide additional information about an SpdxElement.
        /// </summary>
        [XmlElement("annotations")]
        public List<Annotation> Annotations { get; set; }
    }
}
