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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        public static Dictionary<string, List<Component>> MultipleComponentVersions(Bom bom)
        {
            var result = new Dictionary<string, List<Component>>();

            var componentCache = new Dictionary<string, List<Component>>();

            foreach (var component in bom.Components)
            {
                var componentIdentifier = ComponentAnalysisIdentifier(component);
                if (!componentCache.ContainsKey(componentIdentifier))
                {
                    componentCache[componentIdentifier] = new List<Component>();
                }
                componentCache[componentIdentifier].Add(component);
            }

            foreach (var componentEntry in componentCache)
            {
                if (componentEntry.Value.Count > 1)
                {
                    var firstVersion = componentEntry.Value.First().Version;
                    foreach (var component in componentEntry.Value)
                    {
                        if (component.Version != firstVersion)
                        {
                            result[componentEntry.Key] = componentEntry.Value;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
