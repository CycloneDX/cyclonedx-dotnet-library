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
using System.Globalization;
using CycloneDX.Models;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        /// <summary>
        /// Utility method to remove all properties from a BOM that are in the
        /// <c>internal</c> namespace.
        /// 
        /// This should be used on BOMs before sharing them externally to
        /// your organization.
        /// </summary>
        /// <param name="bom"></param>
        public static void RemoveInternalProperties(Bom bom)
        {
            RemoveInternalProperties(bom.Metadata?.Properties);

            CycloneDX.BomUtils.EnumerateAllComponents(bom, (c) => {
                if (c.Properties != null)
                {
                    RemoveInternalProperties(c.Properties);
                    RemoveInternalProperties(c.ReleaseNotes?.Properties);
                }
            });

            CycloneDX.BomUtils.EnumerateAllServices(bom, (s) => {
                RemoveInternalProperties(s.Properties);
                RemoveInternalProperties(s.ReleaseNotes?.Properties);
            });

        }

        private static void RemoveInternalProperties(List<Property> properties)
        {
            if (properties == null) return;
            var currentPosition = 0;
            while (currentPosition < properties.Count)
            {
                if (properties[currentPosition].Name.StartsWith("internal:", false, CultureInfo.InvariantCulture))
                {
                    properties.RemoveAt(currentPosition);
                }
                else
                {
                    currentPosition++;
                }
            }
        }
    }
}
