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

using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Json.Converters;

namespace CycloneDX.Json
{
    /// <summary>
    /// JSON utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns <c>JsonSerializerOptions</c> required to serialize and
        /// deserialize CycloneDX JSON documents.
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            options.Converters.Add(new AggregateTypeConverter());
            options.Converters.Add(new ComponentScopeConverter());
            options.Converters.Add(new ComponentTypeConverter());
            options.Converters.Add(new DataFlowDirectionConverter());
            options.Converters.Add(new DateTimeConverter());
            options.Converters.Add(new DependencyConverter());
            options.Converters.Add(new ExternalReferenceTypeConverter());
            options.Converters.Add(new HashAlgorithmConverter());
            options.Converters.Add(new IssueClassificationConverter());
            options.Converters.Add(new LicenseConverter());
            options.Converters.Add(new PatchClassificationConverter());

            options.Converters.Add(new ImpactAnalysisJustificationConverter());
            options.Converters.Add(new ImpactAnalysisStateConverter());
            options.Converters.Add(new ResponseConverter());

            options.Converters.Add(new ToolChoicesConverter());

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}