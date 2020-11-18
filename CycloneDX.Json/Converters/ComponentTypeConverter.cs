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
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using ComponentType = CycloneDX.Models.v1_2.Component.ComponentType;

namespace CycloneDX.Json
{

    public class ComponentTypeConverter : JsonConverter<ComponentType>
    {
        public override ComponentType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var componentTypeString = reader.GetString();

            if (componentTypeString == "operating-system")
            {
                return ComponentType.OperationSystem;
            }
            else
            {
                ComponentType componentType;
                var success = Enum.TryParse<ComponentType>(componentTypeString, ignoreCase: true, out componentType);
                if (success)
                {
                    return componentType;
                }
                else
                {
                    throw new JsonException();
                }
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ComponentType value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (value == ComponentType.OperationSystem)
            {
                writer.WriteStringValue("operating-system");
            }
            else
            {
                writer.WriteStringValue(value.ToString().ToLowerInvariant());
            }
        }
    }
}
