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
using System.Text.Json;
using System.Threading.Tasks;
using CycloneDX.Models;

namespace CycloneDX.Json
{
    public static partial class Serializer
    {
        /// <summary>
        /// Dserializes CycloneDX JSON document from a stream.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static async Task<Bom> DeserializeAsync(Stream jsonStream)
        {
            Contract.Requires(jsonStream != null);
            return await JsonSerializer.DeserializeAsync<Bom>(jsonStream, _options).ConfigureAwait(false);
        }

        /// <summary>
        /// Deserializes CycloneDX JSON document from a string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Bom Deserialize(string jsonString)
        {
            Contract.Requires(!string.IsNullOrEmpty(jsonString));
            return JsonSerializer.Deserialize<Bom>(jsonString, _options);
        }
    }
}
