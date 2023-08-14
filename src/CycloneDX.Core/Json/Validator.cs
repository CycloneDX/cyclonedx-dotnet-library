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
                    if (specVersion == SchemaVersionResourceFilenameString(SpecificationVersion.v1_4))
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

        /// <summary>
        /// Merge two dictionaries whose values are lists of JsonElements,
        /// adding all entries from list in dict2 for the same key as in
        /// dict1 (or adds a new entry for a new key). Manipulates a COPY
        /// of dict1, then returns this copy.
        /// </summary>
        /// <param name="dict1">Dict with lists as values</param>
        /// <param name="dict2">Dict with lists as values</param>
        /// <returns>Copy of dict1+dict2</returns>
        private static Dictionary<string, List<JsonElement>> addDictList(
            Dictionary<string, List<JsonElement>> dict1,
            Dictionary<string, List<JsonElement>> dict2)
        {
            if (dict2 == null || dict2.Count == 0)
            {
                return dict1;
            }

            if (dict1 == null || dict1.Count == 0)
            {
                return dict2;
            }

            foreach (KeyValuePair<string, List<JsonElement>> KVP in dict2)
            {
                if (dict1.ContainsKey(KVP.Key))
                {
                    // NOTE: Possibly different object, but same string representation!
                    dict1[KVP.Key].AddRange(KVP.Value);
                }
                else
                {
                    dict1.Add(KVP.Key, KVP.Value);
                }
            }

            return dict1;
        }

        /// <summary>
        /// Iterate through the JSON document to find JSON objects whose property names
        /// match the one we seek, and add such hits to returned list. Recurse and repeat.
        /// </summary>
        /// <param name="element">A JsonElement, starting from JsonDocument.RootElement
        ///    for the original caller, probably. Then used to recurse.
        /// </param>
        /// <param name="name">The property name we seek.</param>
        /// <returns>A Dictionary with distinct values of string representation of the
        ///    seeked JsonElement as keys, and a List of actual JsonElement objects as
        ///    mapped values.
        /// </returns>
        private static Dictionary<string, List<JsonElement>> findNamedElements(JsonElement element, string name)
        {
            Dictionary<string, List<JsonElement>> hits = new Dictionary<string, List<JsonElement>>();
            Dictionary<string, List<JsonElement>> nestedHits = null;

            // Can we iterate further?
            switch (element.ValueKind) {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        if (property.Name == name) {
                            string key = property.Value.ToString();
                            if (!(hits.ContainsKey(key)))
                            {
                                hits.Add(key, new List<JsonElement>());
                            }
                            hits[key].Add(property.Value);
                        }

                        // Note: Here we can recurse into same property that
                        // we've just listed, if it is not of a simple kind.
                        nestedHits = findNamedElements(property.Value, name);
                        hits = addDictList(hits, nestedHits);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (JsonElement nestedElem in element.EnumerateArray())
                    {
                        nestedHits = findNamedElements(nestedElem, name);
                        hits = addDictList(hits, nestedHits);
                    }
                    break;

                default:
                    // No-op for simple types: these values per se have no name
                    // to learn, and we can not iterate deeper into them.
                    break;
            }

            return hits;
        }

        private static ValidationResult Validate(JsonSchema schema, JsonDocument jsonDocument, string schemaVersionString)
        {
            var validationMessages = new List<string>();
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

                // The JSON Schema, at least the ones defined by CycloneDX
                // and handled by current parser in dotnet ecosystem, can
                // not specify or check the uniqueness requirement for the
                // "bom-ref" assignments in the overall document (e.g. in
                // "metadata/component" and list of "components", as well
                // as in "services" and "vulnerabilities", as of CycloneDX
                // spec v1.4), so this is checked separately here if the
                // document seems structurally intact otherwise.
                // Note that this is not a problem for the XML schema with
                // its explicit <xs:unique name="bom-ref"> constraint.
                Dictionary<string, List<JsonElement>> bomRefs = findNamedElements(jsonDocument.RootElement, "bom-ref");
                foreach (KeyValuePair<string, List<JsonElement>> KVP in bomRefs) {
                    if (KVP.Value != null && KVP.Value.Count != 1) {
                        validationMessages.Add($"'bom-ref' value of {KVP.Key}: expected 1 mention, actual {KVP.Value.Count}");
                    }
                }
            }
            else
            {
                validationMessages.Add($"Validation failed: {result.Message}");
                validationMessages.Add(result.SchemaLocation.ToString());
                validationMessages.Add($"On instance: {result.InstanceLocation}:");
                validationMessages.Add(result.InstanceLocation.Evaluate(jsonDocument.RootElement).ToString());

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

            return new ValidationResult
            {
                Valid = validationMessages.Count == 0,
                Messages = validationMessages
            };
        }

        private static string SchemaVersionResourceFilenameString(SpecificationVersion schemaVersion) => schemaVersion.ToString().Substring(1).Replace('_', '.');
    }
}
