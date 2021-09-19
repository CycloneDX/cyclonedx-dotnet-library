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
    /// <summary>
    /// Contains all JSON deserialization methods.
    /// </summary>
    public static class Deserializer
    {
        private static JsonSerializerOptions _options_v1_3;
        private static JsonSerializerOptions _options_v1_2;

        /// <summary>
        /// Dserializes any supported CycloneDX JSON document from a stream.
        /// 
        /// Note: To be able to support deserializing any specification
        /// version, the stream is completely read into memory first.
        /// 
        /// If the CycloneDX specification version in use is known, one of the
        /// version specific deserialization methods should be used.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static async Task<Models.v1_3.Bom> DeserializeAsync(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            using (var stream = new MemoryStream())
            {
                // first need to make a copy, some streams aren't replayable
                await jsonStream.CopyToAsync(stream).ConfigureAwait(false);

                try
                {
                    stream.Position = 0;
                    return await DeserializeAsync_v1_3(stream).ConfigureAwait(false);
                }
                catch (JsonException) {}

                stream.Position = 0;
                return new Models.v1_3.Bom(await DeserializeAsync_v1_2(stream).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Deserializes any supported CycloneDX JSON document from a string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deserializes a CycloneDX XML v1.3 document from a stream.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static async Task<Models.v1_3.Bom> DeserializeAsync_v1_3(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            if (_options_v1_3 is null) _options_v1_3 = Utils.GetJsonSerializerOptions_v1_3();
            var bom = await JsonSerializer.DeserializeAsync<Models.v1_3.Bom>(jsonStream, _options_v1_3).ConfigureAwait(false);
            return bom;
        }
        
        /// <summary>
        /// Deserializes a CycloneDX XML v1.3 document from a string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Models.v1_3.Bom Deserialize_v1_3(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            if (_options_v1_3 is null) _options_v1_3 = Utils.GetJsonSerializerOptions_v1_3();
            var bom = JsonSerializer.Deserialize<Models.v1_3.Bom>(jsonString, _options_v1_3);
            return bom;
        }

        /// <summary>
        /// Deserializes a CycloneDX XML v1.2 document from a stream.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static async Task<Models.v1_2.Bom> DeserializeAsync_v1_2(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            if (_options_v1_2 is null) _options_v1_2 = Utils.GetJsonSerializerOptions_v1_2();
            var bom = await JsonSerializer.DeserializeAsync<Models.v1_2.Bom>(jsonStream, _options_v1_2).ConfigureAwait(false);
            return bom;
        }

        /// <summary>
        /// Deserializes a CycloneDX XML v1.2 document from a string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Models.v1_2.Bom Deserialize_v1_2(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            if (_options_v1_2 is null) _options_v1_2 = Utils.GetJsonSerializerOptions_v1_2();
            var bom = JsonSerializer.Deserialize<Models.v1_2.Bom>(jsonString, _options_v1_2);
            return bom;
        }
    }
}
