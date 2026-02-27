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
using CycloneDX.Models;

namespace CycloneDX.Json
{
    /// <summary>
    /// Contains all JSON validation methods.
    /// </summary>
    public static class Validator
    {
        static Validator()
        {
            // I think the global schema registry is not thread safe
            // well, I'm pretty sure, it's the only thing I can think of that would explain the sporadic test failures
            // might as well just do it once on initialisation
            var assembly = typeof(Validator).GetTypeInfo().Assembly;
            using (var spdxStream = assembly.GetManifestResourceStream("CycloneDX.Core.Schemas.spdx.schema.json"))
            using (var spdxStreamReader = new StreamReader(spdxStream))
            {
                var spdxSchema = JsonSchema.FromText(spdxStreamReader.ReadToEnd());
                SchemaRegistry.Global.Register(new Uri("file://spdx.schema.json"), spdxSchema);
            }
            using (var jsfStream = assembly.GetManifestResourceStream("CycloneDX.Core.Schemas.jsf-0.82.schema.json"))
            using (var jsfStreamReader = new StreamReader(jsfStream))
            {
                var jsfSchema = JsonSchema.FromText(jsfStreamReader.ReadToEnd());
                SchemaRegistry.Global.Register(new Uri("file://jsf-0.82.schema.json"), jsfSchema);
            }
            using (var cryptoDefsStream = assembly.GetManifestResourceStream("CycloneDX.Core.Schemas.cryptography-defs.schema.json"))
            using (var cryptoDefsStreamReader = new StreamReader(cryptoDefsStream))
            {
                var cryptoDefsSchema = JsonSchema.FromText(cryptoDefsStreamReader.ReadToEnd());
                SchemaRegistry.Global.Register(new Uri("http://cyclonedx.org/schema/cryptography-defs.schema.json"), cryptoDefsSchema);
            }
        }

        /// <summary>
        /// Validate the stream contents represent a valid CycloneDX JSON document.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="specificationVersion"></param>
        /// <returns></returns>
        public static async Task<ValidationResult> ValidateAsync(Stream jsonStream, SpecificationVersion specificationVersion)
        {
            if (specificationVersion < SpecificationVersion.v1_2)
            {
                throw new Exceptions.UnsupportedFormatSpecificationVersionException($"JSON format is not supported by schema version {specificationVersion}");
            }

            var schemaVersionString = SchemaVersionResourceFilenameString(specificationVersion);
            var assembly = typeof(Validator).GetTypeInfo().Assembly;
            
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-{schemaVersionString}.schema.json"))
            {
                var jsonSchema = await JsonSchema.FromStream(schemaStream).ConfigureAwait(false);
                var jsonDocument = await JsonDocument.ParseAsync(jsonStream).ConfigureAwait(false);
                return Validate(jsonSchema, jsonDocument, schemaVersionString);
            }
        }

        /// <summary>
        /// Validate the string contents represent a valid CycloneDX JSON document.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ValidationResult Validate(string jsonString)
        {
            JsonDocument jsonDocument = null;
            try
            {
                jsonDocument = JsonDocument.Parse(jsonString);
            }
            catch (JsonException exc)
            {
                return new ValidationResult
                {
                    Valid = false,
                    Messages = new List<string> { exc.Message }
                };
            }

            SpecificationVersion? specificationVersion = null;
            foreach (var properties in jsonDocument.RootElement.EnumerateObject())
            {
                if (properties.Name == "specVersion")
                {
                    var specVersion = properties.Value.GetString();
                    if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_7))
                    {
                        specificationVersion = SpecificationVersion.v1_7;
                    }
                    else if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_6))
                    {
                        specificationVersion = SpecificationVersion.v1_6;
                    }
                    else if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_5))
                    {
                        specificationVersion = SpecificationVersion.v1_5;
                    }
                    else if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_4))
                    {
                        specificationVersion = SpecificationVersion.v1_4;
                    }
                    else if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_3))
                    {
                        specificationVersion = SpecificationVersion.v1_3;
                    }
                    else if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_2))
                    {
                        specificationVersion = SpecificationVersion.v1_2;
                    }
                    // no need to test against earlier versions as they don't support JSON
                }
            }

            if (specificationVersion == null)
            {
                return new ValidationResult
                {
                    Valid = false,
                    Messages = new List<string> { "specVersion missing or unknown." }
                };
            }

            return Validate(jsonString, specificationVersion.Value);
        }

        /// <summary>
        /// Validate the string contents represent a valid CycloneDX JSON document.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="specificationVersion"></param>
        /// <returns></returns>
        public static ValidationResult Validate(string jsonString, SpecificationVersion specificationVersion)
        {
            if (specificationVersion < SpecificationVersion.v1_2)
            {
                throw new Exceptions.UnsupportedFormatSpecificationVersionException($"JSON format is not supported by schema version {specificationVersion}");
            }

            var schemaVersionString = SchemaVersionResourceFilenameString(specificationVersion);
            var assembly = typeof(Validator).GetTypeInfo().Assembly;
            
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-{schemaVersionString}.schema.json"))
            using (var schemaStreamReader = new StreamReader(schemaStream))
            {
                var jsonSchema = JsonSchema.FromText(schemaStreamReader.ReadToEnd());
                try
                {
                    var jsonDocument = JsonDocument.Parse(jsonString);
                    return Validate(jsonSchema, jsonDocument, schemaVersionString);
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
        }

        private static ValidationResult Validate(JsonSchema schema, JsonDocument jsonDocument, string schemaVersionString)
        {
            var validationMessages = new List<string>();
            var validationOptions = new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true
            };

            var result = schema.Evaluate(jsonDocument.RootElement, validationOptions);

            if (result.IsValid)
            {
                foreach (var properties in jsonDocument.RootElement.EnumerateObject())
                {
                    if (properties.Name == "specVersion")
                    {
                        var specVersion = properties.Value.GetString();
                        if (specVersion != schemaVersionString)
                        {
                            validationMessages.Add($"Incorrect schema version: expected {schemaVersionString} actual {specVersion}");
                        }
                    }
                }
            }
            else
            {
                validationMessages.Add("Validation failed:");
                // because we requested the results as a flat list
                // there will be no nested results
                foreach (var detail in result.Details)
                {
                    // avoid misleading error messages:
                    // check if any ancestor is valid
                    var currentItem = detail;
                    while (currentItem.Parent != null)
                    {
                        currentItem = currentItem.Parent;
                        if (currentItem.IsValid)
                        {
                            break;
                        }
                    }
                    if (currentItem.IsValid)
                    {
                        continue;
                    }

                    if (detail.HasErrors)
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

        private static string SchemaVersionResourceFilenameString(SpecificationVersion schemaVersion) => schemaVersion.ToString().Substring(1).Replace('_', '.');
    }
}
