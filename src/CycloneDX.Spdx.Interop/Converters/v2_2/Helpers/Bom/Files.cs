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
    public static class Files
    {
        public static List<File> GetSpdxFiles(this Bom bom)
        {
            List<File> files = null;
            if (bom.Components != null && bom.Components.Exists(c => c.Type == Component.Classification.File))
            {
                files = new List<File>();
                foreach (var component in bom.Components.Where(c => c.Type == Component.Classification.File))
                {
                    var file = new File
                    {
                        FileName = component.Name,
                        CopyrightText = component.Copyright ?? "NOASSERTION",
                        SPDXID = component.Properties?.GetSpdxElement(PropertyTaxonomy.SPDXID),
                        Comment = component.Properties?.GetSpdxElement(PropertyTaxonomy.COMMENT),
                        Annotations = component.Properties?.GetSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION),
                        LicenseComments = component.Properties?.GetSpdxElement(PropertyTaxonomy.LICENSE_COMMENTS),
                        LicenseConcluded = component.Properties?.GetSpdxElement(PropertyTaxonomy.LICENSE_CONCLUDED) ?? "NOASSERTION",
                        AttributionTexts = component.GetSpdxAttributionTexts(),
                        FileContributors = component.Properties?.GetSpdxElements( PropertyTaxonomy.FILE_CONTRIBUTOR),
                        NoticeText = component.Properties?.GetSpdxElement(PropertyTaxonomy.FILE_NOTICE_TEXT),
                    };

                    if (file.SPDXID == null)
                    {
                        if (component.BomRef == null)
                        {
                            file.SPDXID = "SPDXRef-File-" + (files.Count + 1).ToString();
                        }
                        else
                        {
                            file.SPDXID = $"SPDXRef-{component.BomRef}";
                        }
                    }

                    if (component.Properties != null && component.Properties.Exists(p => p.Name == PropertyTaxonomy.FILE_TYPE))
                    {
                        file.FileTypes = new List<FileType>();
                        foreach (var fileType in component.Properties.Where(p => p.Name == PropertyTaxonomy.FILE_TYPE))
                        {
                            var fileTypeEnum = (FileType)Enum.Parse(typeof(FileType), fileType.Value);
                            file.FileTypes.Add(fileTypeEnum);
                        }
                    }

                    file.Checksums = component.GetSpdxChecksums();

                    files.Add(file);
                }
            }
            return files;
        }

        public static void AddSpdxFiles(this Bom bom, List<File> files)
        {
            if (files != null && files.Count > 0)
            {
                if (bom.Components == null) bom.Components = new List<Component>();
                foreach (var file in files)
                {
                    var component = new Component
                    {
                        Type = Component.Classification.File,
                        Name = file.FileName,
                        Copyright = file.CopyrightText,
                        Properties = new List<Property>(),
                    };

                    component.Properties.AddSpdxElement(PropertyTaxonomy.SPDXID, file.SPDXID);
                    component.Properties.AddSpdxElement(PropertyTaxonomy.COMMENT, file.Comment);
                    component.Properties.AddSpdxElements(PropertyTaxonomy.FILE_TYPE, file.FileTypes);
                    component.Properties.AddSpdxElements<Models.v2_2.Annotation>(PropertyTaxonomy.ANNOTATION, file.Annotations);
                    component.Properties.AddSpdxElement(PropertyTaxonomy.LICENSE_COMMENTS, file.LicenseComments);
                    component.Properties.AddSpdxElement(PropertyTaxonomy.LICENSE_CONCLUDED, file.LicenseConcluded);
                    component.Properties.AddSpdxElements(PropertyTaxonomy.FILE_CONTRIBUTOR, file.FileContributors);
                    component.Properties.AddSpdxElement(PropertyTaxonomy.FILE_NOTICE_TEXT, file.NoticeText);

                    component.AddSpdxAttributionTexts(file.AttributionTexts);
                    component.AddSpdxChecksums(file.Checksums);

                    bom.Components.Add(component);
                }
            }
        }    
    }
}
