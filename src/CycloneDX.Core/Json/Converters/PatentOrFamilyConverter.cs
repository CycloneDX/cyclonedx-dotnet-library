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

using System;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Models;

namespace CycloneDX.Json.Converters
{
    public class PatentOrFamilyConverter : JsonConverter<PatentOrFamily>
    {
        public override PatentOrFamily Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var doc = JsonDocument.ParseValue(ref reader);
                // Discriminate: Patent has "patentNumber", PatentFamily has "familyId"
                if (doc.RootElement.TryGetProperty("familyId", out _))
                {
                    var family = doc.Deserialize<PatentFamily>(options);
                    return new PatentOrFamily { PatentFamily = family };
                }
                else
                {
                    var patent = doc.Deserialize<Patent>(options);
                    return new PatentOrFamily { Patent = patent };
                }
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            PatentOrFamily value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (value.Patent != null)
            {
                JsonSerializer.Serialize(writer, value.Patent, options);
            }
            else if (value.PatentFamily != null)
            {
                JsonSerializer.Serialize(writer, value.PatentFamily, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
