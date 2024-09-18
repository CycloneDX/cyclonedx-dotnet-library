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

    public class EvidenceIdentityListConverter : JsonConverter<EvidenceIdentityList>
    {
        public override EvidenceIdentityList Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var serializerOptions = Utils.GetJsonSerializerOptions();                
                var identity = JsonSerializer.Deserialize<EvidenceIdentity>(ref reader, serializerOptions);
                return new EvidenceIdentityList { Identities = new List<EvidenceIdentity> { identity } };
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var evidenceIdentityList = new EvidenceIdentityList();
                evidenceIdentityList.Identities = JsonSerializer.Deserialize<List<EvidenceIdentity>>(ref reader, options);
                return evidenceIdentityList;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            EvidenceIdentityList value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            Contract.Requires(value != null);

            if (value.Identities?.Count != 1)
            {
                writer.WriteStartArray();
                if (value.Identities != null)
                {
                    foreach (var identity in value.Identities)
                    {
                        JsonSerializer.Serialize(writer, identity, options);
                    }
                }
                writer.WriteEndArray();
            }
            else
            {
                JsonSerializer.Serialize(writer, value.Identities[0], options);
            }
        }
    }
}
