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
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public class DiffItem<T>
    {
        public List<T> Added { get; set; } = new List<T>();
        public List<T> Removed { get; set; } = new List<T>();
        public List<T> Unchanged { get; set; } = new List<T>();
    }

    public static partial class CycloneDXUtils
    {
        public static Dictionary<string, DiffItem<Component>> ComponentVersionDiff(Bom fromBom, Bom toBom)
        {
            var result = new Dictionary<string, DiffItem<Component>>();

            // make a copy of components that are still to be processed
            var fromComponents = new List<Component>(fromBom.Components);
            var toComponents = new List<Component>(toBom.Components);
            
            // unchanged component versions
            // loop over the toBom and fromBom Components list as we will be modifying the fromComponents list
            foreach (var fromComponent in fromBom.Components)
            {
                // if component version is in both SBOMs
                if (toBom.Components.Count(toComponent =>
                        toComponent.Group == fromComponent.Group
                        && toComponent.Name == fromComponent.Name
                        && toComponent.Version == fromComponent.Version
                    ) > 0)
                {
                    var componentIdentifier = ComponentAnalysisIdentifier(fromComponent);

                    if (!result.ContainsKey(componentIdentifier))
                    {
                        result.Add(componentIdentifier, new DiffItem<Component>());
                    }

                    result[componentIdentifier].Unchanged.Add(fromComponent);

                    fromComponents.RemoveAll(c => c.Group == fromComponent.Group && c.Name == fromComponent.Name && c.Version == fromComponent.Version);
                    toComponents.RemoveAll(c => c.Group == fromComponent.Group && c.Name == fromComponent.Name && c.Version == fromComponent.Version);
                }
            }

            // added component versions
            foreach (var component in new List<Component>(toComponents))
            {
                var componentIdentifier = ComponentAnalysisIdentifier(component);
                if (!result.ContainsKey(componentIdentifier))
                {
                    result.Add(componentIdentifier, new DiffItem<Component>());
                }

                result[componentIdentifier].Added.Add(component);
            }

            // removed components versions
            foreach (var component in new List<Component>(fromComponents))
            {
                var componentIdentifier = ComponentAnalysisIdentifier(component);
                if (!result.ContainsKey(componentIdentifier))
                {
                    result.Add(componentIdentifier, new DiffItem<Component>());
                }

                result[componentIdentifier].Removed.Add(component);
            }

            return result;
        }
    }
}
