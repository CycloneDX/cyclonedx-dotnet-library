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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using CycloneDX;
using CycloneDX.Models;

namespace CycloneDX.Json
{
    public static class Validator
    {
        public static async Task<ValidationResult> Validate(Stream jsonStream, SchemaVersion schemaVersion)
        {
            if (schemaVersion == SchemaVersion.v1_0 || schemaVersion == SchemaVersion.v1_1)
            {
                throw new Exceptions.UnsupportedSchemaVersionException($"JSON format is not supported by schema version {schemaVersion}");
            }

            var validationMessages = new List<string>();

            var schemaVersionString = schemaVersion.ToString().Substring(1).Replace('_', '.');
            var assembly = typeof(Validator).GetTypeInfo().Assembly;
            
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-{schemaVersionString}.schema.json"))
            using (var spdxStream = assembly.GetManifestResourceStream("CycloneDX.Core.Schemas.spdx.schema.json"))
            {
                var schema = await JsonSchema.FromStream(schemaStream);
                var spdxSchema = await JsonSchema.FromStream(spdxStream);

                SchemaRegistry.Global.Register(new Uri("file://spdx.schema.json"), spdxSchema);

                var jsonDocument = await JsonDocument.ParseAsync(jsonStream);
                var validationOptions = new ValidationOptions
                {
                    OutputFormat = OutputFormat.Detailed,
                    RequireFormatValidation = true
                };

                var result = schema.Validate(jsonDocument.RootElement, validationOptions);

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
                    validationMessages.Add($"Validation failed: {result.Message}");
                    validationMessages.Add(result.SchemaLocation.ToString());

                    if (result.NestedResults != null)
                    {
                        var nestedResults = new Queue<ValidationResults>(result.NestedResults);

                        while (nestedResults.Count > 0)
                        {
                            var nestedResult = nestedResults.Dequeue();

                            if (
                                !string.IsNullOrEmpty(nestedResult.Message)
                                && nestedResult.NestedResults != null
                                && nestedResult.NestedResults.Count > 0)
                            {
                                validationMessages.Add($"{nestedResult.InstanceLocation}: {nestedResult.Message}");
                            }
                            
                            if (nestedResult.NestedResults != null)
                            {
                                foreach (var newNestedResult in nestedResult.NestedResults)
                                {
                                    nestedResults.Enqueue(newNestedResult);
                                }
                            }
                        }
                    }
                }
            }

            return new ValidationResult
            {
                Valid = validationMessages.Count == 0,
                Messages = validationMessages
            };
        }
        
        public static async Task<ValidationResult> Validate(string jsonString, SchemaVersion schemaVersion)
        {
            using (var ms = new MemoryStream())
            {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                await ms.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                ms.Position = 0;
                return await Validate(ms, schemaVersion);
            }
        }
    }
}
