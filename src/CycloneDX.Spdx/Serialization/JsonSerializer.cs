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

using CycloneDX.Spdx.Models.v2_3;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CycloneDX.Spdx.Serialization
{
    public static class JsonSerializer
    {
        // this is so we can "cache" the serialization options
        // instantiating a new instance each time tanks serialization performance
        private static JsonSerializerOptions _options_v2_3;

        public static JsonSerializerOptions GetJsonSerializerOptions_v2_3()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }


        public static string Serialize(Models.v2_3.SpdxDocument document)
        {
            if (_options_v2_3 is null) { _options_v2_3 = GetJsonSerializerOptions_v2_3(); }
            return System.Text.Json.JsonSerializer.Serialize(document, _options_v2_3);
        }

        public static async Task SerializeAsync(Models.v2_3.SpdxDocument document, Stream outputStream)
        {
            Contract.Requires(document != null && outputStream != null);
            if (_options_v2_3 is null) { _options_v2_3 = GetJsonSerializerOptions_v2_3(); }
            await System.Text.Json.JsonSerializer.SerializeAsync(outputStream, document, _options_v2_3).ConfigureAwait(false);
        }

        public static Models.v2_3.SpdxDocument Deserialize(string document)
        {
            if (_options_v2_3 is null) { _options_v2_3 = GetJsonSerializerOptions_v2_3(); }
            return System.Text.Json.JsonSerializer.Deserialize<Models.v2_3.SpdxDocument>(document, _options_v2_3);
            return System.Text.Json.JsonSerializer.Deserialize<Models.v2_3.SpdxDocument>(document, _options_v2_3);
        }

        public static async Task<Models.v2_3.SpdxDocument> DeserializeAsync(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            if (_options_v2_3 is null) { _options_v2_3 = GetJsonSerializerOptions_v2_3(); }
            var doc = await System.Text.Json.JsonSerializer.DeserializeAsync<Models.v2_3.SpdxDocument>(jsonStream, _options_v2_3).ConfigureAwait(false);
            return doc;
        }
    }
}
