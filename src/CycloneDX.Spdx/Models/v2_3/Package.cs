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
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public class Package
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
        /// This field provides a place for the SPDX data creator to record acknowledgements that may be required to be communicated in some contexts. This is not meant to include theactual complete license text (see licenseConculded and licenseDeclared), and may or may not include copyright notices (see also copyrightText). The SPDX data creator may use this field to record other acknowledgements, such as particular clauses from license texts, which may be necessary or desirable to reproduce.
        /// </summary>
        [XmlElement("attributionTexts")]
        public List<string> AttributionTexts { get; set; }

        /// <summary>
        /// Provides a place for recording the actual date the package was built.
        /// </summary>
        [XmlElement("builtDate")]
        public DateTime? BuiltDate { get; set; }

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
        /// Provides a detailed description of the package.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// The URI at which this package is available for download. Private (i.e., not publicly reachable) URIs are acceptable as values of this property. The values http://spdx.org/rdf/terms#none and http://spdx.org/rdf/terms#noassertion may be used to specify that the package is not downloadable or that no attempt was made to determine its download location, respectively.
        /// </summary>
        [XmlElement("downloadLocation")]
        public string DownloadLocation { get; set; }

        /// <summary>
        /// An External Reference allows a Package to reference an external source of additional information, metadata, enumerations, asset identifiers, or downloadable content believed to be relevant to the Package.
        /// </summary>
        [XmlElement("externalRefs")]
        public List<ExternalRef> ExternalRefs { get; set; }

        /// <summary>
        /// Indicates whether the file content of this package has been available for or subjected to analysis when creating the SPDX document. If false indicates packages that represent metadata or URI references to a project, product, artifact, distribution or a component. If set to false, the package must not contain any files.
        /// </summary>
        [XmlElement("filesAnalyzed")]
        public bool? FilesAnalyzed { get; set; }
        
        [XmlElement("homepage")]
        public string Homepage { get; set; }

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
        /// License expression for licenseDeclared.  The licensing that the creators of the software in the package, or the packager, have declared. Declarations by the original software creator should be preferred, if they exist.
        /// </summary>
        [XmlElement("licenseDeclared")]
        public string LicenseDeclared { get; set; }

        /// <summary>
        /// The licensing information that was discovered directly within the package. There will be an instance of this property for each distinct value of alllicenseInfoInFile properties of all files contained in the package.
        /// </summary>
        [XmlElement("licenseInfoFromFiles")]
        public List<string> LicenseInfoFromFiles { get; set; }

        /// <summary>
        /// Identify name of this SpdxElement.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name and, optionally, contact information of the person or organization that originally created the package. Values of this property must conform to the agent and tool syntax.
        /// </summary>
        [XmlElement("originator")]
        public string Originator { get; set; }

        /// <summary>
        /// The base name of the package file name. For example, zlib-1.2.5.tar.gz.
        /// </summary>
        [XmlElement("packageFileName")]
        public string PackageFileName { get; set; }

        /// <summary>
        /// A manifest based verification code (the algorithm is defined in section 4.7 of the full specification) of the SPDX Item. This allows consumers of this data and/or database to determine if an SPDX item they have in hand is identical to the SPDX item from which the data was produced. This algorithm works even if the SPDX document is included in the SPDX item.
        /// </summary>
        [XmlElement("packageVerificationCode")]
        public PackageVerificationCode PackageVerificationCode { get; set; }

        /// <summary>
        /// Provides information about the primary purpose of the identified package.
        /// </summary>
        [XmlElement("primaryPackagePurpose")]
        [JsonConverter(typeof(UnderscoreConverter<PrimaryPackagePurposeType>))]
        public PrimaryPackagePurposeType? PrimaryPackagePurpose { get; set; }

        /// <summary>
        /// Indicates that a particular file belongs to a package.
        /// </summary>
        [XmlElement("hasFiles")]
        public List<string> HasFiles { get; set; }

        /// <summary>
        /// Provides a place for recording the date the package was released.
        /// </summary>
        [XmlElement("releaseDate")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Allows the producer(s) of the SPDX document to describe how the package was acquired and/or changed from the original source.
        /// </summary>
        [XmlElement("sourceInfo")]
        public string SourceInfo { get; set; }

        /// <summary>
        /// Provides a short description of the package.
        /// </summary>
        [XmlElement("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// The name and, optionally, contact information of the person or organization who was the immediate supplier of this package to the recipient. The supplier may be different than originator when the software has been repackaged. Values of this property must conform to the agent and tool syntax.
        /// </summary>
        [XmlElement("supplier")]
        public string Supplier { get; set; }

       /// <summary>
        /// Provides a place for recording the end of the support period for a package from the supplier.
        /// </summary>
        [XmlElement("validUntilDate")]
        public DateTime? ValidUntilDate { get; set; }

        /// <summary>
        /// Provides an indication of the version of the package that is described by this SpdxDocument.
        /// </summary>
        [XmlElement("versionInfo")]
        public string VersionInfo { get; set; }

 
    }
}
