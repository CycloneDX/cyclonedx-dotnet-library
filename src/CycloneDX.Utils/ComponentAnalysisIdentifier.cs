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

using CycloneDX.Models;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        /// <summary>
        /// Utility method to generate a component identifier that can be
        /// used when analysing two or more BOMs.
        /// 
        /// In general, a component <c>BomRef</c> should be used to identify
        /// a specific component within a BOM. However, it is not very
        /// useful when trying to compare components across BOMs. For example,
        /// when trying to identify components that have changed versions
        /// between two BOMs.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static string ComponentAnalysisIdentifier(Component component)
        {
            var componentIdentifier = $"{component.Group}:{component.Name}";
            if (componentIdentifier.StartsWith(":")) { componentIdentifier = componentIdentifier.Substring(1); }
            return componentIdentifier;
        }
    }
}
