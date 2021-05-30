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
// Copyright (c) Steve Springett. All Rights Reserved.

using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

using CycloneDX;

namespace CycloneDX.Json
{

    public static class Serializer
    {
        private static JsonSerializerOptions _options_v1_3;
        private static JsonSerializerOptions _options_v1_2;
        
        public static string Serialize(Models.v1_3.Bom bom)
        {
            Contract.Requires(bom != null);

            if (_options_v1_3 == null) _options_v1_3 = Utils.GetJsonSerializerOptions_v1_3();

            var jsonBom = JsonSerializer.Serialize(bom, _options_v1_3);

            return jsonBom;
        }
        
        public static string Serialize(Models.v1_2.Bom bom)
        {
            Contract.Requires(bom != null);

            if (_options_v1_2 == null) _options_v1_2 = Utils.GetJsonSerializerOptions_v1_2();

            var jsonBom = JsonSerializer.Serialize(bom, _options_v1_2);

            return jsonBom;
        }
    }
}
