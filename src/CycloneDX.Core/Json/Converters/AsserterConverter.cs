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
    public class AsserterConverter : JsonConverter<Asserter>
    {
        public override Asserter Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return new Asserter { Ref = reader.GetString() };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var doc = JsonDocument.ParseValue(ref reader);
                // Discriminate: organizationalEntity has "url" or "contact" arrays
                // organizationalContact has "email" or "phone" but not "url" array or "contact" array
                if (doc.RootElement.TryGetProperty("url", out _) || doc.RootElement.TryGetProperty("contact", out _))
                {
                    var org = doc.Deserialize<OrganizationalEntity>(options);
                    return new Asserter { Organization = org };
                }
                else
                {
                    var individual = doc.Deserialize<OrganizationalContact>(options);
                    return new Asserter { Individual = individual };
                }
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            Asserter value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (value.Ref != null)
            {
                writer.WriteStringValue(value.Ref);
            }
            else if (value.Organization != null)
            {
                JsonSerializer.Serialize(writer, value.Organization, options);
            }
            else if (value.Individual != null)
            {
                JsonSerializer.Serialize(writer, value.Individual, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
