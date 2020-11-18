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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExternalReferenceType = CycloneDX.Models.v1_2.ExternalReference.ExternalReferenceType;

namespace CycloneDX.Json.v1_2.Converters
{

    public class ExternalReferenceTypeConverter : JsonConverter<ExternalReferenceType>
    {
        public override ExternalReferenceType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var externalReferenceTypeString = reader.GetString().Replace("-", "");

            ExternalReferenceType externalReferenceType;
            var success = Enum.TryParse<ExternalReferenceType>(externalReferenceTypeString, ignoreCase: true, out externalReferenceType);
            if (success)
            {
                return externalReferenceType;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ExternalReferenceType value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);

            var s = value.ToString();
            var sb = new StringBuilder();
            for (var i=0; i<s.Length; i++)
            {
                if (i != 0 && s[i] == char.ToUpperInvariant(s[i]))
                {
                    sb.Append('-');
                }
                sb.Append(char.ToLowerInvariant(s[i]));
            }
            
            writer.WriteStringValue(sb.ToString());
        }
    }
}
