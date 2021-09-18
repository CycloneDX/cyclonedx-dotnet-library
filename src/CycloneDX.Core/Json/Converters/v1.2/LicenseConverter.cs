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

using License = CycloneDX.Models.v1_2.License;

namespace CycloneDX.Json.Converters.v1_2
{

    public class LicenseConverter : JsonConverter<License>
    {
        public override License Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var license = new License();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return license;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();

                if (propertyName == "text")
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartObject
                        && reader.TokenType != JsonTokenType.Null)
                    {
                        throw new JsonException();
                    }

                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        license.Text = new CycloneDX.Models.v1_2.AttachedText();

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                            {
                                break;
                            }

                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException();
                            }

                            var textPropertyName = reader.GetString();
                            reader.Read();
                            var textPropertyValue = reader.GetString();

                            if (textPropertyName == "content")
                                license.Text.Content = textPropertyValue;
                            else if (textPropertyName == "contentType")
                                license.Text.ContentType = textPropertyValue;
                            else if (textPropertyName == "encoding")
                                license.Text.Encoding = textPropertyValue;
                            else
                                throw new JsonException($"Invalid property name: {textPropertyName}");
                        }
                    }
                }
                else
                {
                    reader.Read();
                    var propertyValue = reader.GetString();

                    if (propertyName == "id")
                        license.Id = propertyValue;
                    else if (propertyName == "name")
                        license.Name = propertyValue;
                    else if (propertyName == "url")
                        license.Url = propertyValue;
                    else
                        throw new JsonException($"Invalid property name: {propertyName}");
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            License value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            Contract.Requires(value != null);

            writer.WriteStartObject();

            if (!string.IsNullOrEmpty(value.Id))
            {
                writer.WritePropertyName("id");
                writer.WriteStringValue(value.Id);
            }
            else if (!string.IsNullOrEmpty(value.Name))
            {
                writer.WritePropertyName("name");
                writer.WriteStringValue(value.Name);
            }

            if (value.Text != null)
            {
                writer.WritePropertyName("text");
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(value.Text.ContentType))
                {
                    writer.WritePropertyName("contentType");
                    writer.WriteStringValue(value.Text.ContentType);
                }
                if (!string.IsNullOrEmpty(value.Text.Encoding))
                {
                    writer.WritePropertyName("encoding");
                    writer.WriteStringValue(value.Text.Encoding);
                }
                if (!string.IsNullOrEmpty(value.Text.Content))
                {
                    writer.WritePropertyName("content");
                    writer.WriteStringValue(value.Text.Content);
                }
                writer.WriteEndObject();
            }

            if (!string.IsNullOrEmpty(value.Url))
            {
                writer.WritePropertyName("url");
                writer.WriteStringValue(value.Url);
            }

            writer.WriteEndObject();
        }
    }
}
