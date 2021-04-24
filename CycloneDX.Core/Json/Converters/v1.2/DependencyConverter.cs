// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dependency = CycloneDX.Models.v1_2.Dependency;

namespace CycloneDX.Json.Converters.v1_2
{

    public class DependencyConverter : JsonConverter<Dependency>
    {
        public override Dependency Read(
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
                return new Dependency
                {
                    Ref = reader.GetString()
                };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var dependency = new Dependency();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dependency;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    var propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName == "ref")
                    {
                        dependency.Ref = reader.GetString();
                    }
                    else if (propertyName == "dependsOn")
                    {
                        dependency.Dependencies = new List<Dependency>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                break;
                            }
                            else if (reader.TokenType == JsonTokenType.String)
                            {
                                dependency.Dependencies.Add(new Dependency
                                {
                                    Ref = reader.GetString()
                                });
                            }
                            else if (reader.TokenType == JsonTokenType.StartArray)
                            {
                                // this happens the first time through
                            }
                            else
                            {
                                throw new JsonException();
                            }
                        }
                    }
                }
                throw new JsonException();
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dependency value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            writer.WriteStartObject();

            writer.WritePropertyName("ref");
            writer.WriteStringValue(value.Ref);

            if (value.Dependencies != null)
            {
                writer.WritePropertyName("dependsOn");
                writer.WriteStartArray();
                foreach (var dependency in value.Dependencies)
                {
                    writer.WriteStringValue(dependency.Ref);
                }
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
