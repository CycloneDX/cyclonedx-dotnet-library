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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CycloneDX;
using CycloneDX.Models;

namespace CycloneDX.Json
{
    /// <summary>
    /// Contains all JSON serialization methods.
    /// </summary>
    public static partial class Serializer
    {
        private static JsonSerializerOptions _options = Utils.GetJsonSerializerOptions();

        /// <summary>
        /// Serializes a CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        public static async Task SerializeAsync(Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null && outputStream != null);
            await JsonSerializer.SerializeAsync<Bom>(outputStream, BomUtils.GetBomForSerialization(bom), _options).ConfigureAwait(false);
        }

        /// <summary>
        /// Serializes a CycloneDX BOM returning the output as a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Bom bom)
        {
            Contract.Requires(bom != null);
            var jsonBom = JsonSerializer.Serialize(BomUtils.GetBomForSerialization(bom), _options);
            return jsonBom;
        }

        internal static string Serialize(Component component)
        {
            Contract.Requires(component != null);
            return JsonSerializer.Serialize(component, _options);
        }

        internal static string Serialize(Dependency dependency)
        {
            Contract.Requires(dependency != null);
            return JsonSerializer.Serialize(dependency, _options);
        }

        internal static string Serialize(Service service)
        {
            Contract.Requires(service != null);
            return JsonSerializer.Serialize(service, _options);
        }

        #pragma warning disable 618
        internal static string Serialize(Tool tool)
        {
            Contract.Requires(tool != null);
            return JsonSerializer.Serialize(tool, _options);
        }
        #pragma warning restore 618

        internal static string Serialize(Models.Vulnerabilities.Vulnerability vulnerability)
        {
            Contract.Requires(vulnerability != null);
            return JsonSerializer.Serialize(vulnerability, _options);
        }
    }
}
