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


    public class SignatureChoiceConverter : JsonConverter<SignatureChoice>
    {
        public override SignatureChoice Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var signatureChoice = new SignatureChoice();
                var doc = JsonDocument.ParseValue(ref reader);
                if (doc.RootElement.TryGetProperty("signers", out var signersValue))
                {
                    var signers = signersValue.Deserialize<List<Signature>>(options);
                    signatureChoice.Signers = signers;
                }
                else if (doc.RootElement.TryGetProperty("chain", out var chainValue))
                {
                    var chain = chainValue.Deserialize<List<Signature>>(options);
                    signatureChoice.Chain = chain;
                }
                else
                {
                    var signature = doc.Deserialize<Signature>(options);
                    signatureChoice.Signature = signature;
                }
                return signatureChoice;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            SignatureChoice value,
            JsonSerializerOptions options)
        {
            Contract.Requires(writer != null);
            Contract.Requires(value != null);

            if (value != null)
            {

                if (value.Signers != null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("signers");
                    JsonSerializer.Serialize(writer, value.Signers, options);
                    writer.WriteEndObject();
                }
                if (value.Chain != null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("chain");
                    JsonSerializer.Serialize(writer, value.Chain, options);
                    writer.WriteEndObject();
                }
                if (value.Signature != null) 
                {
                    JsonSerializer.Serialize(writer, value.Signature, options);
                }
            }
        }
    }
}
