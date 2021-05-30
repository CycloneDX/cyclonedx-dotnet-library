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

using System.Text.Json;

namespace CycloneDX.Json
{
    public static class Utils
    {
        public static JsonSerializerOptions GetJsonSerializerOptions_v1_3()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            options.Converters.Add(new Converters.v1_3.AggregateTypeConverter());
            options.Converters.Add(new Converters.v1_3.ComponentTypeConverter());
            options.Converters.Add(new Converters.v1_3.DataFlowConverter());
            options.Converters.Add(new Converters.v1_3.DateTimeConverter());
            options.Converters.Add(new Converters.v1_3.DependencyConverter());
            options.Converters.Add(new Converters.v1_3.ExternalReferenceTypeConverter());
            options.Converters.Add(new Converters.v1_3.HashAlgorithmConverter());
            options.Converters.Add(new Converters.v1_3.IssueClassificationConverter());
            options.Converters.Add(new Converters.v1_3.LicenseConverter());
            options.Converters.Add(new Converters.v1_3.PatchClassificationConverter());
            return options;
        }

        public static JsonSerializerOptions GetJsonSerializerOptions_v1_2()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            options.Converters.Add(new Converters.v1_2.ComponentTypeConverter());
            options.Converters.Add(new Converters.v1_2.DataFlowConverter());
            options.Converters.Add(new Converters.v1_2.DateTimeConverter());
            options.Converters.Add(new Converters.v1_2.DependencyConverter());
            options.Converters.Add(new Converters.v1_2.ExternalReferenceTypeConverter());
            options.Converters.Add(new Converters.v1_2.HashAlgorithmConverter());
            options.Converters.Add(new Converters.v1_2.IssueClassificationConverter());
            options.Converters.Add(new Converters.v1_2.LicenseConverter());
            options.Converters.Add(new Converters.v1_2.PatchClassificationConverter());
            return options;
        }
    }
}