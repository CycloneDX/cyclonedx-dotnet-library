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
// Copyright (c) Patrick Dwyer. All Rights Reserved.

using System;

namespace CycloneDX
{
    public static class MediaTypes
    {
        public static string Xml => "application/vnd.cyclonedx+xml";
        public static string Json => "application/vnd.cyclonedx+json";
        public static string Protobuf => "application/x.vnd.cyclonedx+protobuf";

        public static string GetMediaType(Format format, SchemaVersion? schemaVersion = null)
        {
            var mediaType = Xml;
            if (format == Format.Json) mediaType = Json;
            if (format == Format.Protobuf) mediaType = Protobuf;

            var version = "1.3";
            if (schemaVersion == SchemaVersion.v1_2) version = "1.2";
            if (schemaVersion == SchemaVersion.v1_1) version = "1.1";
            if (schemaVersion == SchemaVersion.v1_0) version = "1.0";
            
            return schemaVersion.HasValue ? $"{mediaType}; version={version}" : mediaType;
        }
    }
}