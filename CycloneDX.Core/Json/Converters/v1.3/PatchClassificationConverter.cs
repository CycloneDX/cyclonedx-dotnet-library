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
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using PatchClassification = CycloneDX.Models.v1_3.Patch.PatchClassification;

namespace CycloneDX.Json.Converters.v1_3
{

    public class PatchClassificationConverter : JsonConverter<PatchClassification>
    {
        public override PatchClassification Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var patchTypeString = reader.GetString();

            if (patchTypeString == "cherry-pick")
            {
                return PatchClassification.CherryPick;
            }
            else
            {
                PatchClassification patchType;
                var success = Enum.TryParse<PatchClassification>(patchTypeString, ignoreCase: true, out patchType);
                if (success)
                {
                    return patchType;
                }
                else
                {
                    throw new JsonException();
                }
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            PatchClassification value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (value == PatchClassification.CherryPick)
            {
                writer.WriteStringValue("cherry-pick");
            }
            else
            {
                writer.WriteStringValue(value.ToString().ToLowerInvariant());
            }
        }
    }
}
