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

    public class RefObject {
        public string Ref { get; set; }
    }

    public class DatasetChoicesConverter : JsonConverter<DatasetChoices>
    {
        public override DatasetChoices Read(
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
                reader.Read();
                var datasetChoices = new DatasetChoices();
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    var doc = JsonDocument.ParseValue(ref reader);
                    if (doc.RootElement.TryGetProperty("type", out var typeValue))
                    {
                        var data = doc.Deserialize<Data>(options);
                        datasetChoices.Add(new DatasetChoice { DataSet = data });
                    }
                    else
                    {
                        var reference = doc.Deserialize<RefObject>(options);
                        datasetChoices.Add(new DatasetChoice { Ref = reference.Ref });
                    }
                    reader.Read();
                }
                return datasetChoices;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            DatasetChoices value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            Contract.Requires(value != null);

            writer.WriteStartArray();
            if (value != null)
            {
                foreach (var datasetChoice in value)
                {
                    if (datasetChoice.DataSet != null)
                    {
                        JsonSerializer.Serialize(writer, datasetChoice.DataSet, options);
                    }
                    if (datasetChoice.Ref != null)
                    {
                        JsonSerializer.Serialize(writer, new RefObject { Ref = datasetChoice.Ref }, options);
                    }
                }
            }
            writer.WriteEndArray();

        }
    }
}
