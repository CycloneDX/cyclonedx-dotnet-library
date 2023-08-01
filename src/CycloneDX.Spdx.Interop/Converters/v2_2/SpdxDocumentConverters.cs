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
using CycloneDX.Models;
using CycloneDX.Spdx.Models.v2_2;
using CycloneDX.Spdx.Interop.Helpers;

namespace CycloneDX.Spdx.Interop
{
    public static class SpdxDocumentConverters
    {
        public static SpdxDocument ToSpdx(this Bom bom)
        {
            var doc = new SpdxDocument()
            {
                CreationInfo = new CreationInfo(),
            };

            // document
            doc.SPDXID = bom?.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.SPDXID) ?? "SPDXRef-DOCUMENT";
            doc.Comment = bom?.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.COMMENT);

            doc.Name = bom?.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.DOCUMENT_NAME);
            if (doc.Name == null)
            {
                if (bom.Metadata?.Component?.Name != null)
                {
                    doc.Name = bom.Metadata.Component.Name;
                    if (bom.Metadata.Component.Version != null) { doc.Name += $"-{bom.Metadata.Component.Version}"; }
                    if (bom.Metadata.Component.Group != null) { doc.Name = $"{bom.Metadata.Component.Group} {doc.Name}"; }
                }
                else
                {
                    doc.Name = "CycloneDX BOM";
                }
            }
            
            doc.DocumentNamespace = bom?.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.DOCUMENT_NAMESPACE);
            if (doc.DocumentNamespace == null)
            {
                string docId;
                if (string.IsNullOrEmpty(bom.SerialNumber))
                {
                    docId = Guid.NewGuid().ToString();
                }
                else if (bom.SerialNumber.StartsWith("urn:uuid:", StringComparison.InvariantCulture))
                {
                    docId = bom.SerialNumber.Remove(0, 9);
                }
                else
                {
                    docId = bom.SerialNumber;
                }
                doc.DocumentNamespace = $"http://spdx.org/spdxdocs/{doc.Name}-{docId}";
            }

            // creation info
            doc.CreationInfo.Comment = bom.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.CREATION_INFO_COMMENT) ?? "This SPDX document has been converted from CycloneDX format.";
            doc.CreationInfo.Created = bom.Metadata?.Timestamp != null ? bom.Metadata.Timestamp.Value : DateTime.UtcNow;
            doc.CreationInfo.Creators = bom.GetSpdxCreators();
            doc.CreationInfo.LicenseListVersion = bom.Metadata?.Properties?.GetSpdxElement(PropertyTaxonomy.CREATION_INFO_LICENSE_LIST_VERSION);

            doc.ExternalDocumentRefs = bom.Metadata?.Properties?.GetSpdxElements<ExternalDocumentRef>(PropertyTaxonomy.DOCUMENT_EXTERNAL_DOCUMENT_REF);
            doc.Annotations = bom.Metadata?.Properties?.GetSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION);
            doc.DocumentDescribes = bom.Metadata?.Properties?.GetSpdxElements(PropertyTaxonomy.DOCUMENT_DESCRIBES);

            doc.AddCycloneDXComponents(bom);
            doc.Files = bom.GetSpdxFiles();
            //TODO HasExtractedLicensingInfos
            //TODO relationships, assemblies, dependency graph, etc

            return doc;
        }

        public static Bom ToCycloneDX(this SpdxDocument doc)
        {
            var bom = new Bom()
            {
                Metadata = new Metadata
                {
                    Properties = new List<Property>(),
                }
            };

            // document
            bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.SPDXID, doc.SPDXID);
            bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.DOCUMENT_SPDX_VERSION, doc.SpdxVersion);
            bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.COMMENT, doc.Comment);
            bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.DOCUMENT_NAME, doc.Name);
            bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.DOCUMENT_NAMESPACE, doc.DocumentNamespace);

            // creation info
            if (doc.CreationInfo != null)
            {
                bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.CREATION_INFO_COMMENT, doc.CreationInfo.Comment);
                bom.Metadata.Timestamp = doc.CreationInfo.Created;
                bom.AddSpdxCreators(doc.CreationInfo.Creators);
                bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.CREATION_INFO_LICENSE_LIST_VERSION, doc.CreationInfo.LicenseListVersion);
            }

            bom.Metadata.Properties.AddSpdxElements<ExternalDocumentRef>(PropertyTaxonomy.DOCUMENT_EXTERNAL_DOCUMENT_REF, doc.ExternalDocumentRefs);
            bom.Metadata.Properties.AddSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION, doc.Annotations);
            bom.Metadata.Properties.AddSpdxElements(PropertyTaxonomy.DOCUMENT_DESCRIBES, doc.DocumentDescribes);

            bom.AddSpdxPackages(doc);
            bom.AddSpdxFiles(doc.Files);

            return bom;
        }


    }
}
