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
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_2
{
    public class ExternalDocumentRef
    {
        /// <summary>
        /// externalDocumentId is a string containing letters, numbers, ., - and/or + which uniquely identifies an external document within this document.
        /// </summary>
        [XmlElement("externalDocumentId")]
        public string ExternalDocumentId { get; set; }

        /// <summary>
        /// A Checksum is value that allows the contents of a file to be authenticated. Even small changes to the content of the file will change its checksum. This class allows the results of a variety of checksum and cryptographic message digest algorithms to be represented.
        /// </summary>
        [XmlElement("checksum")]
        public Checksum Checksum { get; set; } = new Checksum();

        /// <summary>
        /// SPDX ID for SpdxDocument.  A propoerty containing an SPDX document.
        /// </summary>
        [XmlElement("spdxDocument")]
        public string SpdxDocument { get; set; }
    }
}
