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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Models;

namespace CycloneDX.Json.Converters
{

    public class ToolChoicesConverter : JsonConverter<ToolChoices>
    {
        public override ToolChoices Read(
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
                // need to remove _this_ converter from the options to prevent recursion below
                var serializerOptions = Utils.GetJsonSerializerOptions();
                for (var i = 0; i < serializerOptions.Converters.Count; i++)
                {
                    if (serializerOptions.Converters[i].CanConvert(typeof(ToolChoices)))
                    {
                        serializerOptions.Converters.RemoveAt(i);
                        break;
                    }
                }
                var toolChoices = JsonSerializer.Deserialize<ToolChoices>(ref reader, serializerOptions);
                return toolChoices;
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var toolChoices = new ToolChoices();
                #pragma warning disable 618
                toolChoices.Tools = JsonSerializer.Deserialize<List<Tool>>(ref reader, options);
                #pragma warning restore 618
                return toolChoices;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ToolChoices value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            Contract.Requires(value != null);

            if (value.Tools != null)
            {
                writer.WriteStartArray();
                foreach (var tool in value.Tools)
                {
                    JsonSerializer.Serialize(writer, tool, options);
                }
                writer.WriteEndArray();
            }
            else if (value.Components != null || value.Services != null)
            {
                writer.WriteStartObject();
                if (value.Components != null)
                {
                    writer.WritePropertyName("components");
                    JsonSerializer.Serialize(writer, value.Components, options);
                }
                if (value.Services != null)
                {
                    writer.WritePropertyName("services");
                    JsonSerializer.Serialize(writer, value.Services, options);
                }
                writer.WriteEndObject();                
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
