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

namespace CycloneDX.Spdx.Models.v2_2
{
    public class SpdxDocument
    {
        /// <summary>
        /// Uniquely identify any element in an SPDX document which may be referenced by other elements.
        /// </summary>
        [JsonPropertyName("SPDXID")]
        public string SPDXID { get; set; }

        /// <summary>
        /// Provide a reference number that can be used to understand how to parse and interpret the rest of the file. It will enable both future changes to the specification and to support backward compatibility. The version number consists of a major and minor version indicator. The major field will be incremented when incompatible changes between versions are made (one or more sections are created, modified or deleted). The minor field will be incremented when backwards compatible changes are made.
        /// </summary>
        public string SpdxVersion { get; } = "SPDX-2.2";

        /// <summary>
        /// One instance is required for each SPDX file produced. It provides the necessary information for forward and backward compatibility for processing tools.
        /// </summary>
        public CreationInfo CreationInfo { get; set; } = new CreationInfo();

        /// <summary>
        /// Identify name of this SpdxElement.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// License expression for dataLicense.  Compliance with the SPDX specification includes populating the SPDX fields therein with data related to such fields ("SPDX-Metadata"). The SPDX specification contains numerous fields where an SPDX document creator may provide relevant explanatory text in SPDX-Metadata. Without opining on the lawfulness of "database rights" (in jurisdictions where applicable), such explanatory text is copyrightable subject matter in most Berne Convention countries. By using the SPDX specification, or any portion hereof, you hereby agree that any copyright rights (as determined by your jurisdiction) in any SPDX-Metadata, including without limitation explanatory text, shall be subject to the terms of the Creative Commons CC0 1.0 Universal license. For SPDX-Metadata not containing any copyright rights, you hereby agree and acknowledge that the SPDX-Metadata is provided to you "as-is" and without any representations or warranties of any kind concerning the SPDX-Metadata, express, implied, statutory or otherwise, including without limitation warranties of title, merchantability, fitness for a particular purpose, non-infringement, or the absence of latent or other defects, accuracy, or the presence or absence of errors, whether or not discoverable, all to the greatest extent permissible under applicable law.
        /// </summary>
        public string DataLicense { get; } = "CC0-1.0";

        public string Comment { get; set; }

        /// <summary>
        /// Identify any external SPDX documents referenced within this SPDX document.
        /// </summary>
        public List<ExternalDocumentRef> ExternalDocumentRefs { get; set; }

        /// <summary>
        /// Indicates that a particular ExtractedLicensingInfo was defined in the subject SpdxDocument.
        /// </summary>
        public List<ExtractedLicensingInfo> HasExtractedLicensingInfos { get; set; }

        /// <summary>
        /// Provide additional information about an SpdxElement.
        /// </summary>
        public List<Annotation> Annotations { get; set; }

        /// <summary>
        /// Reviewed
        /// </summary>
        [Obsolete("Revieweds is deprecated, use Annotation instead")]
        public List<ReviewInformation> Revieweds { get; set; }

        /// <summary>
        /// The URI provides an unambiguous mechanism for other SPDX documents to reference SPDX elements within this SPDX document.
        /// </summary>
        public string DocumentNamespace { get; set; }

        /// <summary>
        /// Packages, files and/or Snippets described by this SPDX document
        /// </summary>
        public List<string> DocumentDescribes { get; set; }

        /// <summary>
        /// Packages referenced in the SPDX document
        /// </summary>
        public List<Package> Packages { get; set; }

        /// <summary>
        /// Files referenced in the SPDX document
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Snippets referenced in the SPDX document
        /// </summary>
        public List<Snippet> Snippets { get; set; }

        /// <summary>
        /// Relationships referenced in the SPDX document
        /// </summary>
        public List<Relationship> Relationships { get; set; }
    }
}
