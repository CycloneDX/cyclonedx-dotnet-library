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
    public class File
    {
        /// <summary>
        /// Uniquely identify any element in an SPDX document which may be referenced by other elements.
        /// </summary>
        [JsonPropertyName("SPDXID")]
        public string SPDXID { get; set; }

        /// <summary>
        /// Provide additional information about an SpdxElement.
        /// </summary>
        [XmlElement("annotations")]
        public List<Annotation> Annotations { get; set; }

        /// <summary>
        /// The checksum property provides a mechanism that can be used to verify that the contents of a File or Package have not changed.
        /// </summary>
        [XmlElement("checksums")]
        public List<Checksum> Checksums { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The text of copyright declarations recited in the Package or File.
        /// </summary>
        [XmlElement("copyrightText")]
        public string CopyrightText { get; set; }

        /// <summary>
        /// This field provides a place for the SPDX file creator to record file contributors. Contributors could include names of copyright holders and/or authors who may not be copyright holders yet contributed to the file content.
        /// </summary>
        [XmlElement("fileContributors")]
        public List<string> FileContributors { get; set; }

        /// <summary>
        /// The name of the file relative to the root of the package.
        /// </summary>
        [XmlElement("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// The type of the file.
        /// </summary>
        [XmlElement("fileTypes")]
        public List<FileType> FileTypes { get; set; }

        /// <summary>
        /// This field provides a place for the SPDX data creator to record acknowledgements that may be required to be communicated in some contexts. This is not meant to include theactual complete license text (see licenseConculded and licenseDeclared), and may or may not include copyright notices (see also copyrightText). The SPDX data creator may use this field to record other acknowledgements, such as particular clauses from license texts, which may be necessary or desirable to reproduce.
        /// </summary>
        [XmlElement("attributionTexts")]
        public List<string> AttributionTexts { get; set; }

        /// <summary>
        /// Indicates the project in which the SpdxElement originated. Tools must preserve doap:homepage and doap:name properties and the URI (if one is known) of doap:Project resources that are values of this property. All other properties of doap:Projects are not directly supported by SPDX and may be dropped when translating to or from some SPDX formats.
        /// </summary>
        [Obsolete("Artifacts Of is deprecated, use Relationships instead")]
        public List<string> ArtifactOfs { get; set; }

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
        /// Licensing information that was discovered directly in the subject file. This is also considered a declared license for the file.
        /// </summary>
        [XmlElement("licenseInfoInFiles")]
        public List<string> LicenseInfoInFiles { get; set; }

        /// <summary>
        /// This field provides a place for the SPDX file creator to record potential legal notices found in the file. This may or may not include copyright statements.
        /// </summary>
        [XmlElement("noticeText")]
        public string NoticeText { get; set; }

        [Obsolete("File Dependencies is deprecated, use Relationships instead")]
        public List<string> FileDependencies { get; set; }
    }
}
