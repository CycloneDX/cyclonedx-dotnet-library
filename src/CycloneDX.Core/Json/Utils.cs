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
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;

namespace CycloneDX.Json
{
    /// <summary>
    /// JSON utility methods.
    /// </summary>
    public static class Utils
    {
        public static JsonSerializerOptions GetBaseJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }
        /// <summary>
        /// Returns <c>JsonSerializerOptions</c> required to serialize and
        /// deserialize CycloneDX JSON documents.
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = GetBaseJsonSerializerOptions();
            
            options.Converters.Add(new UnderscoreEnumConverter<Composition.AggregateType>());
            options.Converters.Add(new HyphenEnumConverter<Component.ComponentScope>());
            options.Converters.Add(new HyphenEnumConverter<Component.Classification>());
            options.Converters.Add(new HyphenEnumConverter<Licensing.LicenseType>());
            options.Converters.Add(new HyphenEnumConverter<DataFlowDirection>());
            options.Converters.Add(new DateTimeConverter());
            options.Converters.Add(new DependencyConverter());
            options.Converters.Add(new HyphenEnumConverter<ExternalReference.ExternalReferenceType>());
            options.Converters.Add(new KeepCaseHyphenEnumConverter<Hash.HashAlgorithm>());
            options.Converters.Add(new UnderscoreEnumConverter<Issue.IssueClassification>());
            options.Converters.Add(new HyphenEnumConverter<Patch.PatchClassification>());

            options.Converters.Add(new UnderscoreEnumConverter<ImpactAnalysisJustification>());
            options.Converters.Add(new UnderscoreEnumConverter<ImpactAnalysisState>());
            options.Converters.Add(new UnderscoreEnumConverter<Response>());
            
            options.Converters.Add(new HyphenEnumConverter<Workspace.AccessModeType>());
            options.Converters.Add(new HyphenEnumConverter<Lifecycles.LifecyclePhase>());

            options.Converters.Add(new EnvironmentVarChoiceConverter());
            options.Converters.Add(new ToolChoicesConverter());
            
            options.Converters.Add(new HyphenEnumConverter<EvidenceMethods.EvidenceTechnique>());
            options.Converters.Add(new ScoreMethodConverter());
            options.Converters.Add(new HyphenEnumConverter<Severity>());
            options.Converters.Add(new HyphenEnumConverter<Trigger.TriggerType>());
            options.Converters.Add(new HyphenEnumConverter<WorkflowTask.TaskType>());
            options.Converters.Add(new HyphenEnumConverter<WorkflowTaskType>());
            options.Converters.Add(new HyphenEnumConverter<Output.OutputType>());
            options.Converters.Add(new HyphenEnumConverter<ModelCard.ModelParameterApproachType>());
            options.Converters.Add(new UnderscoreEnumConverter<Status>());

            options.Converters.Add(new JsonStringEnumConverter());
            
            options.Converters.Add(new DataflowSourceDestinationConverter());
            
            return options;
        }
    }
}