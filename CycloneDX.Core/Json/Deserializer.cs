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
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CycloneDX.Json
{

    public static class Deserializer
    {
        private static JsonSerializerOptions _options_v1_3;
        private static JsonSerializerOptions _options_v1_2;

        [Obsolete("Deserialize(Stream) is deprecated and will be removed in a future version. Use DeserializeAsync(Stream) instead.")]
        public static Models.v1_3.Bom Deserialize(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            var ms = new MemoryStream();
            jsonStream.CopyTo(ms);
            var jsonString = Encoding.UTF8.GetString(ms.ToArray());
            return Deserialize(jsonString);
        }

        public static async Task<Models.v1_3.Bom> DeserializeAsync(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            try
            {
                return await DeserializeAsync_v1_3(jsonStream);
            }
            catch (JsonException) {}

            return new Models.v1_3.Bom(await DeserializeAsync_v1_2(jsonStream));
        }

        public static Models.v1_3.Bom Deserialize(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            try
            {
                return Deserialize_v1_3(jsonString);
            }
            catch (JsonException) {}

            return new Models.v1_3.Bom(Deserialize_v1_2(jsonString));
        }

        [Obsolete("Deserialize_v1_3(Stream) is deprecated and will be removed in a future version. Use DeserializeAsync_v1_3(Stream) instead.")]
        public static Models.v1_3.Bom Deserialize_v1_3(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            var ms = new MemoryStream();
            jsonStream.CopyTo(ms);
            var jsonString = Encoding.UTF8.GetString(ms.ToArray());
            var bom = Deserialize_v1_3(jsonString);
            return bom;
        }
        
        public static async Task<Models.v1_3.Bom> DeserializeAsync_v1_3(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            if (_options_v1_3 is null) _options_v1_3 = Utils.GetJsonSerializerOptions_v1_3();
            var bom = await JsonSerializer.DeserializeAsync<Models.v1_3.Bom>(jsonStream, _options_v1_3);
            return bom;
        }
        

        public static Models.v1_3.Bom Deserialize_v1_3(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            if (_options_v1_3 is null) _options_v1_3 = Utils.GetJsonSerializerOptions_v1_3();
            var bom = JsonSerializer.Deserialize<Models.v1_3.Bom>(jsonString, _options_v1_3);
            return bom;
        }

        [Obsolete("Deserialize_v1_2(Stream) is deprecated and will be removed in a future version. Use DeserializeAsync_v1_2(Stream) instead.")]
        public static Models.v1_2.Bom Deserialize_v1_2(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            var ms = new MemoryStream();
            jsonStream.CopyTo(ms);
            var jsonString = Encoding.UTF8.GetString(ms.ToArray());
            var bom = Deserialize_v1_2(jsonString);
            return bom;
        }
        
        public static async Task<Models.v1_2.Bom> DeserializeAsync_v1_2(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            if (_options_v1_2 is null) _options_v1_2 = Utils.GetJsonSerializerOptions_v1_2();
            var bom = await JsonSerializer.DeserializeAsync<Models.v1_2.Bom>(jsonStream, _options_v1_2);
            return bom;
        }

        public static Models.v1_2.Bom Deserialize_v1_2(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            if (_options_v1_2 is null) _options_v1_2 = Utils.GetJsonSerializerOptions_v1_2();
            var bom = JsonSerializer.Deserialize<Models.v1_2.Bom>(jsonString, _options_v1_2);
            return bom;
        }
    }
}
