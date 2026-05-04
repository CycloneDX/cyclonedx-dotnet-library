// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

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
        private static readonly JsonSerializerOptions _defaultOptions = Utils.GetJsonSerializerOptions(useUnsafeRelaxedJsonEscaping: false);
        private static readonly JsonSerializerOptions _unsafeRelaxedOptions = Utils.GetJsonSerializerOptions(useUnsafeRelaxedJsonEscaping: true);

        public static JsonSerializerOptions SerializerOptionsForHash { get => _defaultOptions; }

        // Two pre-built, immutable JsonSerializerOptions are cached at startup - one for
        // default escaping, one for unsafe/relaxed. This avoids the massive per-call cost of
        // reconstructing options (see PR #437). Callers should prefer the explicit parameter
        // on Serialize(); the global Utils.UseUnsafeRelaxedJsonEscaping fallback is kept for
        // backward compatibility but is not thread-safe and should be phased out.
        #pragma warning disable 612, 618 // Intentional use of obsolete UseUnsafeRelaxedJsonEscaping for backward compat
        private static JsonSerializerOptions GetOptions(bool? unsafeRelaxedJsonEscaping = null)
        {
            switch (unsafeRelaxedJsonEscaping)
            {
                case true:  return _unsafeRelaxedOptions;
                case false: return _defaultOptions;
                // Fallback: no explicit choice — honour the legacy global flag.
                // Once all consumers migrate to the explicit parameter this
                // path (and Utils.UseUnsafeRelaxedJsonEscaping) can be removed.
                default:    return Utils.UseUnsafeRelaxedJsonEscaping ? _unsafeRelaxedOptions : _defaultOptions;
            }
        }
        #pragma warning restore 612, 618

        /// <summary>
        /// Serializes a CycloneDX BOM writing the output to a stream.
        /// Uses <see cref="Utils.UseUnsafeRelaxedJsonEscaping"/> for escaping behavior.
        /// </summary>
        public static Task SerializeAsync(Bom bom, Stream outputStream)
        {
            return SerializeAsync(bom, outputStream, null);
        }

        /// <summary>
        /// Serializes a CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        /// <param name="unsafeRelaxedJsonEscaping">Use relaxed JSON escaping. When null, falls back to <see cref="Utils.UseUnsafeRelaxedJsonEscaping"/>.</param>
        /// <returns></returns>
        public static async Task SerializeAsync(Bom bom, Stream outputStream, bool? unsafeRelaxedJsonEscaping)
        {            
            Contract.Requires(bom != null && outputStream != null);
            await JsonSerializer.SerializeAsync<Bom>(outputStream, BomUtils.GetBomForSerialization(bom), GetOptions(unsafeRelaxedJsonEscaping)).ConfigureAwait(false);
        }

        /// <summary>
        /// Serializes a CycloneDX BOM returning the output as a string.
        /// Uses <see cref="Utils.UseUnsafeRelaxedJsonEscaping"/> for escaping behavior.
        /// </summary>
        public static string Serialize(Bom bom)
        {
            return Serialize(bom, null);
        }

        /// <summary>
        /// Serializes a CycloneDX BOM returning the output as a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="unsafeRelaxedJsonEscaping">Use relaxed JSON escaping. When null, falls back to <see cref="Utils.UseUnsafeRelaxedJsonEscaping"/>.</param>
        /// <returns></returns>
        public static string Serialize(Bom bom, bool? unsafeRelaxedJsonEscaping)
        {
            Contract.Requires(bom != null);
            var jsonBom = JsonSerializer.Serialize(BomUtils.GetBomForSerialization(bom), GetOptions(unsafeRelaxedJsonEscaping));
            return jsonBom;
        }

        internal static string Serialize(Component component)
        {
            Contract.Requires(component != null);
            return JsonSerializer.Serialize(component, GetOptions());
        }

        internal static string Serialize(Dependency dependency)
        {
            Contract.Requires(dependency != null);
            return JsonSerializer.Serialize(dependency, GetOptions());
        }

        internal static string Serialize(Lifecycles lifecycles)
        {
            Contract.Requires(lifecycles != null);
            return JsonSerializer.Serialize(lifecycles, getOption());
        }

        internal static string Serialize(Service service)
        {
            Contract.Requires(service != null);
            return JsonSerializer.Serialize(service, GetOptions());
        }

        #pragma warning disable 618
        internal static string Serialize(Tool tool)
        {
            Contract.Requires(tool != null);
            return JsonSerializer.Serialize(tool, GetOptions());
        }
        #pragma warning restore 618

        internal static string Serialize(Models.Vulnerabilities.Vulnerability vulnerability)
        {
            Contract.Requires(vulnerability != null);
            return JsonSerializer.Serialize(vulnerability, GetOptions());
        }

        internal static string Serialize(Models.Composition composition)
        {
            Contract.Requires(composition != null);
            return JsonSerializer.Serialize(composition, GetOptions());
        }

        internal static string Serialize(Models.ExternalReference externalReference)
        {
            Contract.Requires(externalReference != null);
            return JsonSerializer.Serialize(externalReference, GetOptions());
        }

        internal static string Serialize(Models.Standard standard)
        {
            Contract.Requires(standard != null);
            return JsonSerializer.Serialize(standard, GetOptions());
        }

        internal static string Serialize(Models.OrganizationalEntity organization)
        {
            Contract.Requires(organization != null);
            return JsonSerializer.Serialize(organization, GetOptions());
        }

        internal static string Serialize(Models.Claim obj)
        {
            Contract.Requires(obj != null);
            return JsonSerializer.Serialize(obj, GetOptions());
        }
        internal static string Serialize(Models.Assessor obj)
        {
            Contract.Requires(obj != null);
            return JsonSerializer.Serialize(obj, GetOptions());
        }
        internal static string Serialize(Models.Attestation obj)
        {
            Contract.Requires(obj != null);
            return JsonSerializer.Serialize(obj, GetOptions());
        }
    }
}
