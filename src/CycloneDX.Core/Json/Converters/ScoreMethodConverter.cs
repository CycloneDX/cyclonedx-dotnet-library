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
using CycloneDX.Models.Vulnerabilities;

namespace CycloneDX.Json.Converters
{

    public class ScoreMethodConverter : JsonConverter<ScoreMethod>
    {
        public override ScoreMethod Read(
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

            ScoreMethod enumValue;
            var success = Enum.TryParse<ScoreMethod>(enumString.Replace(".", ""), ignoreCase: true, out enumValue);
            if (success)
            {
                return enumValue;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ScoreMethod value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            if (value == ScoreMethod.Other)
            {
                writer.WriteStringValue("other");
            }
            else if (value == ScoreMethod.CVSSV31)
            {
                writer.WriteStringValue("CVSSv3.1");
            }
            else if (value.ToString().StartsWith("CVSSV"))
            {
                writer.WriteStringValue("CVSSv" + value.ToString().Substring(5));
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
