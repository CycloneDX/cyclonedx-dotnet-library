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
using CycloneDX.Spdx.Models.v2_3;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class ExternalRefs
    {
        public static List<ExternalRef> GetSpdxExternalRefs(this Component component)
        {
            if (component.Properties == null)
            {
                return null;
            }
            var extRefs = new List<ExternalRef>();
            foreach (var extRefProp in component.Properties.Where(p => p.Name.StartsWith(PropertyTaxonomy.EXTERNAL_REFERENCE)))
            {
                var extRef = new ExternalRef();
                if (extRefProp.Name.StartsWith(PropertyTaxonomy.EXTERNAL_REFERENCE_OTHER))
                {
                    extRef.ReferenceCategory = ExternalRefCategory.OTHER;
                    extRef.ReferenceType = extRefProp.Name.Substring(PropertyTaxonomy.EXTERNAL_REFERENCE_OTHER.Length + 1);
                }
                else {
                    switch (extRefProp.Name)
                    {
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_SECURITY_CPE22:
                            extRef.ReferenceCategory = ExternalRefCategory.SECURITY;
                            extRef.ReferenceType = "cpe22Type";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_SECURITY_CPE23:
                            extRef.ReferenceCategory = ExternalRefCategory.SECURITY;
                            extRef.ReferenceType = "cpe23Type";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_MAVEN_CENTRAL:
                            extRef.ReferenceCategory = ExternalRefCategory.PACKAGE_MANAGER;
                            extRef.ReferenceType = "maven-central";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_NPM:
                            extRef.ReferenceCategory = ExternalRefCategory.PACKAGE_MANAGER;
                            extRef.ReferenceType = "npm";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_NUGET:
                            extRef.ReferenceCategory = ExternalRefCategory.PACKAGE_MANAGER;
                            extRef.ReferenceType = "nuget";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_BOWER:
                            extRef.ReferenceCategory = ExternalRefCategory.PACKAGE_MANAGER;
                            extRef.ReferenceType = "bower";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_PURL:
                            extRef.ReferenceCategory = ExternalRefCategory.PACKAGE_MANAGER;
                            extRef.ReferenceType = "purl";
                            break;
                        case PropertyTaxonomy.EXTERNAL_REFERENCE_PERSISTENT_ID_SWH:
                             extRef.ReferenceCategory = ExternalRefCategory.PERSISTENT_ID;
                             extRef.ReferenceType = "swh";
                             break;
                        default:
                            break;
                    }
                    //TODO and write corresponding code in AddExternalRefsToCDX
                }
                if (extRef.ReferenceType != null)
                {
                    var parts = extRefProp.Value.Split(' ');
                    extRef.ReferenceLocator = parts[0];
                    if (parts.Length > 1)
                    {
                        extRef.Comment = String.Join(" ", extRefProp.Value.Split(' ').Skip(1).ToList());
                    }
                    extRefs.Add(extRef);
                }
            }
            return extRefs.Count == 0 ? null : extRefs;
        }

        public static void AddSpdxExternalRefs(this Component component, List<ExternalRef> externalRefs)
        {
            if (externalRefs != null && externalRefs.Count > 0)
            {
                if (component.Properties == null) { component.Properties = new List<Property>(); }
                foreach (var extRef in externalRefs)
                {
                    string refPropName = null;
                    if (extRef.ReferenceCategory == ExternalRefCategory.SECURITY)
                    {
                        if(extRef.ReferenceType == "cpe22Type") { refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_SECURITY_CPE22; }
                        else if(extRef.ReferenceType == "cpe23Type"){ refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_SECURITY_CPE23; } 
                    }
                    else if (extRef.ReferenceCategory == ExternalRefCategory.PACKAGE_MANAGER)
                    {
                        switch (extRef.ReferenceType)
                        {
                            case "maven-central":
                                refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_MAVEN_CENTRAL;
                                break;
                            case "npm":
                                refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_NPM;
                                break;
                            case "nuget":
                                refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_NUGET;
                                break;
                            case "bower":
                                refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_BOWER;
                                break;
                            case "purl":
                                refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PACKAGE_MANAGER_PURL;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (extRef.ReferenceCategory == ExternalRefCategory.PERSISTENT_ID) { refPropName = PropertyTaxonomy.EXTERNAL_REFERENCE_PERSISTENT_ID_SWH; }
                    else if (extRef.ReferenceCategory == ExternalRefCategory.OTHER)
                    {
                        refPropName = $"{PropertyTaxonomy.EXTERNAL_REFERENCE_OTHER}:{extRef.ReferenceType}";
                    }
                    
                    if (refPropName != null)
                    {
                        var refPropValue = extRef.ReferenceLocator;
                        if (extRef.Comment != null)
                        {
                            refPropValue = $"{extRef.ReferenceLocator} {extRef.Comment}";
                        }
                        component.Properties.AddSpdxElement(refPropName, refPropValue);
                    }
                }
            }
        }    
    }
}
