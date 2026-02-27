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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Models;

namespace CycloneDX.Json.Converters
{
    public class PatentAssigneeConverter : JsonConverter<List<OrganizationalEntityOrContact>>
    {
        public override List<OrganizationalEntityOrContact> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var result = new List<OrganizationalEntityOrContact>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        var doc = JsonDocument.ParseValue(ref reader);
                        // Discriminate: organizationalEntity has "url" or "contact" arrays
                        if (doc.RootElement.TryGetProperty("url", out _) || doc.RootElement.TryGetProperty("contact", out _))
                        {
                            var org = doc.Deserialize<OrganizationalEntity>(options);
                            result.Add(new OrganizationalEntityOrContact { Organization = org });
                        }
                        else
                        {
                            var individual = doc.Deserialize<OrganizationalContact>(options);
                            result.Add(new OrganizationalEntityOrContact { Individual = individual });
                        }
                    }
                }
                return result;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            List<OrganizationalEntityOrContact> value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var item in value)
            {
                if (item.Organization != null)
                {
                    JsonSerializer.Serialize(writer, item.Organization, options);
                }
                else if (item.Individual != null)
                {
                    JsonSerializer.Serialize(writer, item.Individual, options);
                }
            }
            writer.WriteEndArray();
        }
    }
}
