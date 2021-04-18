﻿// This file is part of the CycloneDX Tool for .NET
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

namespace CycloneDX.Json.v1_3.Converters
{

    public class DateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var valueString = reader.GetString();
            var value = DateTime.Parse(valueString);
            return value;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime? value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            writer.WriteStringValue(value?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }
}