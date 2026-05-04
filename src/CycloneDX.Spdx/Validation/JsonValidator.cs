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
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using CycloneDX.Spdx.Models;

namespace CycloneDX.Spdx.Validation
{
    public static class JsonValidator
    {
        private static readonly JsonSchema _spdxSchema;

        static JsonValidator()
        {
            // Pre-load the SPDX schema once to avoid concurrent JsonSchema.FromText
            // calls registering schemas in the global registry in parallel.
            var assembly = typeof(JsonValidator).GetTypeInfo().Assembly;
            using (var schemaStream = assembly.GetManifestResourceStream("CycloneDX.Spdx.Schemas.spdx-2.3.schema.json"))
            using (var schemaStreamReader = new StreamReader(schemaStream))
            {
                _spdxSchema = JsonSchema.FromText(schemaStreamReader.ReadToEnd());
            }
        }

        /// <summary>
        /// Validate the stream contents represent a valid SPDX JSON document.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="schemaVersion"></param>
        /// <returns></returns>
        public static async Task<ValidationResult> ValidateAsync(Stream jsonStream)
        {
            var jsonDocument = await JsonDocument.ParseAsync(jsonStream).ConfigureAwait(false);
            return Validate(_spdxSchema, jsonDocument);
        }

        /// <summary>
        /// Validate the string contents represent a valid SPDX JSON document.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ValidationResult Validate(string jsonString)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonString);
                return Validate(_spdxSchema, jsonDocument);
            }
            catch (JsonException exc)
            {
                return new ValidationResult
                {
                    Valid = false,
                    Messages = new List<string> { exc.Message }
                };
            }
        }

        private static ValidationResult Validate(JsonSchema schema, JsonDocument jsonDocument)
        {
            var validationMessages = new List<string>();
            var validationOptions = new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true
            };

            var result = schema.Evaluate(jsonDocument.RootElement, validationOptions);

            if (!result.IsValid)
            {
                validationMessages.Add("Validation failed:");
                // because we requested the results as a flat list
                // there will be no nested results
                foreach (var detail in result.Details)
                {
                    if (detail.Errors != null)
                    {
                        foreach (var error in detail.Errors)
                        {
                            validationMessages.Add(error.Value);
                        }
                        validationMessages.Add(detail.SchemaLocation.ToString());
                        validationMessages.Add($"On instance: {detail.InstanceLocation}:");
                        validationMessages.Add(detail.InstanceLocation.Evaluate(jsonDocument.RootElement).ToString());
                    }
                }
            }

            return new ValidationResult
            {
                Valid = validationMessages.Count == 0,
                Messages = validationMessages
            };
        }
    }
}
