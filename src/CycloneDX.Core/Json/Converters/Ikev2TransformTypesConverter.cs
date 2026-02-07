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
using System.Text.Json;
using System.Text.Json.Serialization;
using CycloneDX.Core.Models;

namespace CycloneDX.Json.Converters
{
    public class Ikev2TransformTypesConverter : JsonConverter<Ikev2TransformTypes>
    {
        public override Ikev2TransformTypes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new Ikev2TransformTypes();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "encr":
                        result.EncryptionAlgorithmsDetailed = ReadEncrArray(ref reader, options);
                        break;
                    case "prf":
                        result.PseudorandomFunctionsDetailed = ReadPrfArray(ref reader, options);
                        break;
                    case "integ":
                        result.IntegrityAlgorithmsDetailed = ReadIntegArray(ref reader, options);
                        break;
                    case "ke":
                        result.KeyExchangeMethodsDetailed = ReadKeArray(ref reader, options);
                        break;
                    case "esn":
                        result.ExtendedSequenceNumbers = reader.GetBoolean();
                        break;
                    case "auth":
                        result.AuthenticationMethodsDetailed = ReadAuthArray(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException();
        }

        private List<Ikev2Encr> ReadEncrArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var list = new List<Ikev2Encr>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                if (reader.TokenType == JsonTokenType.String)
                {
                    // Deprecated string format - treat as algorithm ref
                    list.Add(new Ikev2Encr { Algorithm = reader.GetString() });
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(JsonSerializer.Deserialize<Ikev2Encr>(ref reader, options));
                }
                else
                {
                    throw new JsonException();
                }
            }
            throw new JsonException();
        }

        private List<Ikev2Prf> ReadPrfArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var list = new List<Ikev2Prf>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                if (reader.TokenType == JsonTokenType.String)
                {
                    list.Add(new Ikev2Prf { Algorithm = reader.GetString() });
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(JsonSerializer.Deserialize<Ikev2Prf>(ref reader, options));
                }
                else
                {
                    throw new JsonException();
                }
            }
            throw new JsonException();
        }

        private List<Ikev2Integ> ReadIntegArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var list = new List<Ikev2Integ>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                if (reader.TokenType == JsonTokenType.String)
                {
                    list.Add(new Ikev2Integ { Algorithm = reader.GetString() });
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(JsonSerializer.Deserialize<Ikev2Integ>(ref reader, options));
                }
                else
                {
                    throw new JsonException();
                }
            }
            throw new JsonException();
        }

        private List<Ikev2Ke> ReadKeArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var list = new List<Ikev2Ke>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                if (reader.TokenType == JsonTokenType.String)
                {
                    list.Add(new Ikev2Ke { Algorithm = reader.GetString() });
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(JsonSerializer.Deserialize<Ikev2Ke>(ref reader, options));
                }
                else
                {
                    throw new JsonException();
                }
            }
            throw new JsonException();
        }

        private List<Ikev2Auth> ReadAuthArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            var list = new List<Ikev2Auth>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                if (reader.TokenType == JsonTokenType.String)
                {
                    list.Add(new Ikev2Auth { Algorithm = reader.GetString() });
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(JsonSerializer.Deserialize<Ikev2Auth>(ref reader, options));
                }
                else
                {
                    throw new JsonException();
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Ikev2TransformTypes value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.EncryptionAlgorithmsDetailed != null)
            {
                writer.WritePropertyName("encr");
                JsonSerializer.Serialize(writer, value.EncryptionAlgorithmsDetailed, options);
            }

            if (value.PseudorandomFunctionsDetailed != null)
            {
                writer.WritePropertyName("prf");
                JsonSerializer.Serialize(writer, value.PseudorandomFunctionsDetailed, options);
            }

            if (value.IntegrityAlgorithmsDetailed != null)
            {
                writer.WritePropertyName("integ");
                JsonSerializer.Serialize(writer, value.IntegrityAlgorithmsDetailed, options);
            }

            if (value.KeyExchangeMethodsDetailed != null)
            {
                writer.WritePropertyName("ke");
                JsonSerializer.Serialize(writer, value.KeyExchangeMethodsDetailed, options);
            }

            if (value.ExtendedSequenceNumbers.HasValue)
            {
                writer.WriteBoolean("esn", value.ExtendedSequenceNumbers.Value);
            }

            if (value.AuthenticationMethodsDetailed != null)
            {
                writer.WritePropertyName("auth");
                JsonSerializer.Serialize(writer, value.AuthenticationMethodsDetailed, options);
            }

            writer.WriteEndObject();
        }
    }
}
