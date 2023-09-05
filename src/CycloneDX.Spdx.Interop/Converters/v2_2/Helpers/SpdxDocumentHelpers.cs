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
using System.Linq;
using CycloneDX.Models;
using CycloneDX.Spdx.Models.v2_2;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class SpdxDocumentHelpers
    {
        public static bool IsSpdxPackageSupportedComponentType(Component component)
        {
            return component.Type == Component.Classification.Application
                || component.Type == Component.Classification.Firmware
                || component.Type == Component.Classification.Framework
                || component.Type == Component.Classification.Library
                || component.Type == Component.Classification.Operating_System
                || component.Type == Component.Classification.Container;
        }

        public static void AddCycloneDXComponents(this SpdxDocument doc, Bom bom)
        {
            if (bom.Components == null || bom.Components.Count == 0) { return; }
            doc.Packages = doc.Packages ?? new List<Package>();
            foreach (var component in bom.Components.Where(c => IsSpdxPackageSupportedComponentType(c)))
            {
                var package = new Package
                {
                    Name = component.Name,
                    VersionInfo = component.Version,
                    Description = component.Description,
                    CopyrightText = component.Copyright ?? "NOASSERTION",
                };
                package.SPDXID = component.Properties?.GetSpdxElement(PropertyTaxonomy.SPDXID);
                if (package.SPDXID == null)
                {
                    if (component.BomRef == null)
                    {
                        package.SPDXID = "SPDXRef-Package-" + (doc.Packages.Count + 1).ToString();
                    }
                    else
                    {
                        package.SPDXID = $"SPDXRef-{component.BomRef}";
                    }
                }
                package.Annotations = component.Properties?.GetSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION);
                package.FilesAnalyzed = component.Properties?.GetSpdxElement<bool?>(PropertyTaxonomy.FILES_ANALYZED);
                package.LicenseComments = component.Properties?.GetSpdxElement(PropertyTaxonomy.LICENSE_COMMENTS);
                package.LicenseConcluded = component.Properties?.GetSpdxElement(PropertyTaxonomy.LICENSE_CONCLUDED) ?? "NOASSERTION";
                package.PackageFileName = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_FILENAME);
                var packageVerificationCode = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_VERIFICATION_CODE_VALUE);
                if (packageVerificationCode != null)
                {
                    package.PackageVerificationCode = new PackageVerificationCode
                    {
                        PackageVerificationCodeValue = packageVerificationCode,
                        PackageVerificationCodeExcludedFiles = component.Properties?.GetSpdxElements(PropertyTaxonomy.PACKAGE_VERIFICATION_CODE_EXCLUDED_FILE),
                    };
                }
                package.SourceInfo = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_SOURCE_INFO);
                package.Summary = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_SUMMARY);
                package.Comment = component.Properties?.GetSpdxElement(PropertyTaxonomy.COMMENT);

                // LicenseInfoFromFiles
                if (component.Evidence?.Licenses != null && component.Evidence.Licenses.Count > 0)
                {
                    if (package.LicenseInfoFromFiles == null) package.LicenseInfoFromFiles = new List<string>();
                    foreach (var license in component.Evidence.Licenses)
                    {
                        if (license.Expression != null)
                        {
                            // TODO revisit this after some sleep
                            // at first glance it doesn't look like expressions are handled in ExtractedLicensingInfo
                        }
                        else if (license.License != null)
                        {
                            if (license.License.Id != null)
                            {
                                package.LicenseInfoFromFiles.Add(license.License.Id);
                            }
                            else
                            {
                                if (doc.HasExtractedLicensingInfos == null) doc.HasExtractedLicensingInfos = new List<ExtractedLicensingInfo>();
                                var extLicInfo = new ExtractedLicensingInfo
                                {
                                    LicenseId = $"LicenseRef-{doc.HasExtractedLicensingInfos.Count + 1}",
                                    Name = license.License.Name,
                                    SeeAlsos = license.License.Url == null ? null : new List<string> { license.License.Url },
                                    ExtractedText = license.License.Text?.Content?.Base64Decode(),
                                };
                                doc.HasExtractedLicensingInfos.Add(extLicInfo);
                                package.LicenseInfoFromFiles.Add(extLicInfo.LicenseId);
                            }
                        }
                    }
                }
                if (package.LicenseInfoFromFiles == null || package.LicenseInfoFromFiles.Count == 0)
                {
                    package.LicenseInfoFromFiles = new List<string> { "NOASSERTION" };
                }

                // LicenseDeclared
                package.LicenseDeclared = component.Properties?.GetSpdxElement(PropertyTaxonomy.LICENSE_DECLARED);
                if (component.Licenses == null || component.Licenses.Count == 0)
                {
                    package.LicenseDeclared = "NOASSERTION";
                }
                else
                {
                    if (component.Licenses.Count == 1)
                    {
                        package.LicenseDeclared = component.Licenses.First().Expression ?? component.Licenses.First().License.Id;
                    }
                    else
                    {
                        package.LicenseDeclared = "NOASSERTION";
                    }
                }

                // Package Originator
                package.Originator = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR) ?? "NOASSERTION";
                if (component.Author != null)
                {
                    if (component.Author == component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR_ORGANIZATION))
                    {
                        package.Originator = $"Organization: {component.Author} ({component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR_EMAIL) ?? ""})";
                    }
                    else
                    {
                        package.Originator = $"Person: {component.Author} ({component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR_EMAIL) ?? ""})";
                    }
                }

                package.Supplier = component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_SUPPLIER) ?? "NOASSERTION";
                if (component.Supplier != null)
                {
                    var supplierEmails = component.Supplier.Contact.Where(c => c.Email != null).ToList();
                    var supplierEmail = supplierEmails.Count > 0 ? supplierEmails.First().Email : "";
                    if (component.Supplier.Name == component.Properties?.GetSpdxElement(PropertyTaxonomy.PACKAGE_SUPPLIER_ORGANIZATION))
                    {
                        package.Supplier = $"Organization: {component.Supplier.Name} ({supplierEmail})";
                    }
                    else
                    {
                        package.Supplier = $"Person: {component.Supplier.Name} ({supplierEmail})";
                    }
                }

                package.AttributionTexts = component.GetSpdxAttributionTexts();

                package.Checksums = component.GetSpdxChecksums();
                package.ExternalRefs = component.GetSpdxExternalRefs();

                package.DownloadLocation = component.Properties?.GetSpdxElement(PropertyTaxonomy.DOWNLOAD_LOCATION) ?? "NOASSERTION";
                package.Homepage = component.Properties?.GetSpdxElement(PropertyTaxonomy.HOMEPAGE) ?? "NOASSERTION";

                //TODO HasFile

                doc.Packages.Add(package);
            }
        }
    }
}
