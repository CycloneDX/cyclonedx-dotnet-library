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
using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Models;
using ImpactAnalysisState = CycloneDX.Models.Vulnerabilities.ImpactAnalysisState;

namespace CycloneDX.Json.Converters
{

    public class ImpactAnalysisStateConverter : JsonConverter<ImpactAnalysisState>
    {
        public override ImpactAnalysisState Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var stateString = reader.GetString();

            ImpactAnalysisState state;
            var success = Enum.TryParse<ImpactAnalysisState>(stateString.Replace("_", ""), ignoreCase: true, out state);
            if (success)
            {
                return state;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ImpactAnalysisState value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }
}
