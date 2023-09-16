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
using System.Text.RegularExpressions;
using CycloneDX.Models;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class Creators
    {
        public static List<string> GetSpdxCreators(this Bom bom)
        {
            var creators = new List<string>();

            if (bom.Metadata?.Tools?.Tools != null)
            {
                foreach (var tool in bom.Metadata.Tools.Tools)
                {
                    creators.Add($"Tool: {tool.Name}-{tool.Version}");
                }
            }
            
            if (bom.Metadata?.Authors != null)
            {
                var orgs = bom.Metadata.Properties?.GetSpdxElements(PropertyTaxonomy.CREATION_INFO_LICENSE_CREATORS_ORGANIZATIONS) ?? new List<string>();
                foreach (var author in bom.Metadata.Authors)
                {
                    if (orgs.Contains(author.Name))
                    {
                        creators.Add($"Organization: {author.Name} ({author.Email})");
                    }
                    else
                    {
                        creators.Add($"Person: {author.Name} ({author.Email})");
                    }
                }
            }

            return creators.Count == 0 ? null : creators;
        }

        public static void AddSpdxCreators(this Bom bom, List<string> creators)
        {
            if (creators != null)
            {
                var toolRegex = new Regex(@"Tool: (?<name>.*)-(?<version>.*)");
                var nonToolRegex = new Regex(@"(Person|Organization): (?<name>.*) \((?<email>.*)\)");
                foreach (var creator in creators)
                {
                    var toolMatch = toolRegex.Match(creator);
                    if (toolMatch.Success)
                    {
                        if (bom.Metadata == null) bom.Metadata = new Metadata();
                        #pragma warning disable 618
                        if (bom.Metadata?.Tools?.Tools == null)
                            bom.Metadata.Tools = new ToolChoices { Tools = new List<Tool>() };
                        bom.Metadata.Tools.Tools.Add(new Tool {
                            Name = toolMatch.Groups["name"].ToString(),
                            Version = toolMatch.Groups["version"].ToString(),
                        });
                        #pragma warning restore 618
                    }
                    else
                    {
                        var nonToolMatch = nonToolRegex.Match(creator);
                        if (nonToolMatch.Success)
                        {
                            if (bom.Metadata.Authors == null) bom.Metadata.Authors = new List<OrganizationalContact>();
                            bom.Metadata.Authors.Add(new OrganizationalContact
                            {
                                Name = nonToolMatch.Groups["name"].ToString(),
                                Email = nonToolMatch.Groups["email"].ToString(),
                            });
                            if (creator.StartsWith("Organization:"))
                            {
                                bom.Metadata.Properties.AddSpdxElement(PropertyTaxonomy.CREATION_INFO_LICENSE_CREATORS_ORGANIZATIONS, nonToolMatch.Groups["name"].ToString());
                            }
                        }
                    }
                }
            }
        }    
    }
}
