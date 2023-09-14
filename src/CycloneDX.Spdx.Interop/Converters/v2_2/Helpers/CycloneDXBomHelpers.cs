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
using System.Text.RegularExpressions;
using CycloneDX.Models;
using CycloneDX.Spdx.Models.v2_2;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class CycloneDXBomHelpers
    {
        public static void AddSpdxPackages(this Bom bom, SpdxDocument doc)
        {
            if (doc.Packages == null || doc.Packages.Count == 0) { return; }
            if (bom.Components == null) bom.Components = new List<Component>();
            foreach (var package in doc.Packages)
            {
                var component = new Component
                {
                    Type = Component.Classification.Library,
                    Name = package.Name,
                    Version = package.VersionInfo,
                    Copyright = package.CopyrightText,
                    Description = package.Description,
                    Properties = new List<Property>(),
                };
                component.Properties.AddSpdxElement(PropertyTaxonomy.SPDXID, package.SPDXID);
                component.Properties.AddSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION, package.Annotations);
                component.Properties.AddSpdxElement(PropertyTaxonomy.FILES_ANALYZED, package.FilesAnalyzed);
                component.Properties.AddSpdxElement(PropertyTaxonomy.LICENSE_COMMENTS, package.LicenseComments);
                component.Properties.AddSpdxElement(PropertyTaxonomy.LICENSE_CONCLUDED, package.LicenseConcluded);
                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_FILENAME, package.PackageFileName);
                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_VERIFICATION_CODE_VALUE, package.PackageVerificationCode?.PackageVerificationCodeValue);
                component.Properties.AddSpdxElements(PropertyTaxonomy.PACKAGE_VERIFICATION_CODE_EXCLUDED_FILE, package.PackageVerificationCode?.PackageVerificationCodeExcludedFiles);
                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_SOURCE_INFO, package.SourceInfo);
                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_SUMMARY, package.Summary);
                component.Properties.AddSpdxElement(PropertyTaxonomy.COMMENT, package.Comment);

                if (package.LicenseInfoFromFiles != null && package.LicenseInfoFromFiles.Count > 0)
                {
                    if (component.Evidence == null) component.Evidence = new Evidence();
                    if (component.Evidence.Licenses == null) component.Evidence.Licenses = new List<LicenseChoice>();
                    foreach (var licenseInfo in package.LicenseInfoFromFiles)
                    {
                        if (licenseInfo.StartsWith("LicenseRef-")
                            && doc.HasExtractedLicensingInfos != null
                            && doc.HasExtractedLicensingInfos.Exists(l => l.LicenseId == licenseInfo))
                        {
                            var license = doc.HasExtractedLicensingInfos.First(l => l.LicenseId == licenseInfo);
                            component.Evidence.Licenses.Add(new LicenseChoice
                            {
                                License = new License
                                {
                                    Name = license.Name,
                                    Text = new AttachedText
                                    {
                                        ContentType = "text/plain",
                                        Encoding = "base64",
                                        Content = license.ExtractedText.Base64Encode(),
                                    },
                                    Url = license.SeeAlsos?.FirstOrDefault(),
                                }
                            });
                        }
                        else if (licenseInfo.StartsWith("LicenseRef-") || licenseInfo.StartsWith("DocumentRef-"))
                        {
                            component.Evidence.Licenses.Add(new LicenseChoice
                            {
                                License = new License
                                {
                                    Name = licenseInfo,
                                }
                            });
                        }
                        else if (licenseInfo == "NONE" || licenseInfo == "NOASSERTION")
                        {
                            // don't do anything for this case
                        }
                        else
                        {
                            component.Evidence.Licenses.Add(new LicenseChoice
                            {
                                License = new License
                                {
                                    Id = licenseInfo,
                                }
                            });
                        }
                    }
                }

                if (package.LicenseDeclared == "NOASSERTION")
                {
                    component.Properties.AddSpdxElement(PropertyTaxonomy.LICENSE_DECLARED, package.LicenseDeclared);
                }
                else if (package.LicenseDeclared == "NONE")
                {
                    component.Licenses = new List<LicenseChoice>();
                }
                else
                {
                    component.Licenses = new List<LicenseChoice> { new LicenseChoice { Expression = package.LicenseDeclared } };
                }

                if (package.Originator != null)
                {
                    if (package.Originator == "NOASSERTION")
                    {
                        component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR, package.Originator);
                    }
                    else
                    {
                        var originatorRegex = new Regex(@"(Person|Organization): (?<name>.*) \((?<email>.*)\)");
                        var originatorMatch = originatorRegex.Match(package.Originator);
                        if (originatorMatch.Success)
                        {
                            component.Author = originatorMatch.Groups["name"].ToString();
                            if (package.Originator.ToLowerInvariant().StartsWith("organization:"))
                            {
                                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR_ORGANIZATION, component.Author);
                            }
                            component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_ORIGINATOR_EMAIL, originatorMatch.Groups["email"].ToString());
                        }
                    }
                }

                if (package.Supplier != null)
                {
                    if (package.Supplier == "NOASSERTION")
                    {
                        component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_SUPPLIER, package.Supplier);
                    }
                    else
                    {
                        var supplierRegex = new Regex(@"(Person|Organization): (?<name>.*) \((?<email>.*)\)");
                        var supplierMatch = supplierRegex.Match(package.Supplier);
                        if (supplierMatch.Success)
                        {
                            component.Supplier = new OrganizationalEntity
                            {
                                Name = supplierMatch.Groups["name"].ToString(),
                                Contact = new List<OrganizationalContact>
                                {
                                    new OrganizationalContact
                                    {
                                        Email = supplierMatch.Groups["email"].ToString()
                                    }
                                },
                            };
                            if (package.Supplier.ToLowerInvariant().StartsWith("organization:"))
                            {
                                component.Properties.AddSpdxElement(PropertyTaxonomy.PACKAGE_SUPPLIER_ORGANIZATION, component.Supplier.Name);
                            }
                        }
                    }
                }

                component.AddSpdxAttributionTexts(package.AttributionTexts);
                component.AddSpdxChecksums(package.Checksums);
                component.AddSpdxExternalRefs(package.ExternalRefs);
                
                if (package.DownloadLocation != null)
                {
                    if (component.ExternalReferences == null) component.ExternalReferences = new List<ExternalReference>();
                    component.ExternalReferences.Add(new ExternalReference
                    {
                        Type = ExternalReference.ExternalReferenceType.Distribution,
                        Url = package.DownloadLocation,
                    });
                    component.Properties.AddSpdxElement(PropertyTaxonomy.DOWNLOAD_LOCATION, package.DownloadLocation);
                }

                if (package.Homepage != null)
                {
                    if (component.ExternalReferences == null) component.ExternalReferences = new List<ExternalReference>();
                    component.ExternalReferences.Add(new ExternalReference
                    {
                        Type = ExternalReference.ExternalReferenceType.Website,
                        Url = package.Homepage,
                    });
                    component.Properties.AddSpdxElement(PropertyTaxonomy.HOMEPAGE, package.Homepage);
                }

                //TODO HasFile

                bom.Components.Add(component);
            }
        }

    }
}
