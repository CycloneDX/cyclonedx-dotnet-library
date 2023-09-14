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

namespace CycloneDX
{
    public static class SpecificationVersionHelpers
    {
        public static readonly SpecificationVersion CurrentVersion = SpecificationVersion.v1_5;

        public static readonly string XmlNamespaceRegexString = @"http:\/\/cyclonedx\.org\/schema\/bom\/(?<SpecificationVersionString>1\.[0-5])";

        public static readonly Regex XmlNamespaceRegex = new Regex(XmlNamespaceRegexString);

        public static bool IsValidXmlNamespace(string xmlns)
        {
            var match = XmlNamespaceRegex.Match(xmlns);
            return match.Success;
        }

        public static string XmlNamespaceSpecificationVersion(string xmlns)
        {
            var match = XmlNamespaceRegex.Match(xmlns);
            if (match.Success)
            {
                return match.Groups["SpecificationVersionString"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static string XmlNamespace(SpecificationVersion specificationVersion) {
            return $"http://cyclonedx.org/schema/bom/{VersionString(specificationVersion)}";
        }

        public static string VersionString(SpecificationVersion specificationVersion)
        {
            return specificationVersion.ToString().Replace('_', '.').Trim('v');
        }

        public static SpecificationVersion Version(string specificationVersion)
        {
            if (Enum.TryParse($"v{specificationVersion.Replace('.', '_')}", out SpecificationVersion result))
            {
                return result;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"Unrecognised specification version string: {specificationVersion}");
            }
        }
    }
}