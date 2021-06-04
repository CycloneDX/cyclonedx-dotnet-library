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
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    class ListMergeHelper<T>
    {
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            if (list1 == null) return list2;
            if (list2 == null) return list1;

            var result = new List<T>(list1);
            result.AddRange(list2);

            return result;
        }
    }

    public static partial class CycloneDXUtils
    {
        public static Bom Merge(Bom sbom1, Bom sbom2)
        {
            var result = new Bom();

            var toolsMerger = new ListMergeHelper<Tool>();
            var tools = toolsMerger.Merge(sbom1.Metadata?.Tools, sbom2.Metadata?.Tools);
            if (tools != null)
            {
                result.Metadata = new Metadata
                {
                    Tools = tools
                };
            }

            var componentsMerger = new ListMergeHelper<Component>();
            result.Components = componentsMerger.Merge(sbom1.Components, sbom2.Components);

            var servicesMerger = new ListMergeHelper<Service>();
            result.Services = servicesMerger.Merge(sbom1.Services, sbom2.Services);

            var extRefsMerger = new ListMergeHelper<ExternalReference>();
            result.ExternalReferences = extRefsMerger.Merge(sbom1.ExternalReferences, sbom2.ExternalReferences);

            var dependenciesMerger = new ListMergeHelper<Dependency>();
            result.Dependencies = dependenciesMerger.Merge(sbom1.Dependencies, sbom2.Dependencies);

            return result;
        }
    }
}
