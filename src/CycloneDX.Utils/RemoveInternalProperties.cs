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
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        public static void RemoveInternalProperties(Bom bom)
        {
            RemoveInternalProperties(bom.Metadata?.Properties);

            if (bom.Components != null)
            {
                var componentQueue = new Queue<Component>(bom.Components);
                while (componentQueue.Count > 0)
                {
                    var component = componentQueue.Dequeue();
                    RemoveInternalProperties(component.Properties);
                    if (component.Components != null)
                    {
                        foreach (var subComponent in component.Components)
                        {
                            componentQueue.Enqueue(subComponent);
                        }
                    }
                }
            }

            if (bom.Services != null) {
                foreach (var service in bom.Services)
                {
                    RemoveInternalProperties(service.Properties);
                }
            }
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
