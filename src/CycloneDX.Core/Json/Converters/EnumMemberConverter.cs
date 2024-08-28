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
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CycloneDX.Json.Converters
{

    // Decorate enum values with the EnumMember to specialize how it is serialized
    // If unspecified, it behaves as the HyphenEnumConverter
    public class EnumMemberConverter<T> : JsonConverter<T> where T: struct, System.Enum
    {

        private readonly Dictionary<T, string> _valueToString = new Dictionary<T, string>();
        private readonly Dictionary<string, T> _stringToValue = new Dictionary<string, T>();

        public EnumMemberConverter()
        {
            var enumType = typeof(T);
            var enumValues = Enum.GetValues(enumType).Cast<T>();

            foreach (var value in enumValues)
            {
                var enumMember = enumType.GetMember(value.ToString())[0];
                var attr = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                  .Cast<EnumMemberAttribute>().FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }

                _valueToString.Add(value, attr.Value);
                _stringToValue.Add(attr.Value, value);
            }
        }

        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var enumString = reader.GetString();

            if (_stringToValue.TryGetValue(enumString, out var value))
            {
                return value;
            }
            else
            {
                var success = Enum.TryParse<T>(enumString.Replace("-", "_"), ignoreCase: true, out var enumValue);
                if (success)
                {
                    return enumValue;
                }
                else
                {
                    throw new JsonException();
                }
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            if (_valueToString.TryGetValue(value, out var stringValue))
            {
                writer.WriteStringValue(stringValue);
            }
            else
            {
                writer.WriteStringValue(value.ToString().ToLowerInvariant().Replace("_", "-"));
            }
        }
    }
}
