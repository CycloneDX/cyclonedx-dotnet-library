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
using CycloneDX.Exceptions;

namespace CycloneDX
{
    /// <summary>
    /// Utility class for official CycloneDX Media Types
    /// </summary>
    public static class MediaTypes
    {
        /// <summary>
        /// The officially IANA registered media type for CycloneDX XML files.
        /// </summary>
        /// <returns><c>"application/vnd.cyclonedx+xml"</c></returns>
        public static string Xml => "application/vnd.cyclonedx+xml";

        /// <summary>
        /// The officially IANA registered media type for CycloneDX JSON files.
        /// </summary>
        /// <returns><c>"application/vnd.cyclonedx+json"</c></returns>
        public static string Json => "application/vnd.cyclonedx+json";

        /// <summary>
        /// The official media type for CycloneDX Protocol Buffer files.
        /// </summary>
        /// <returns><c>"application/x.vnd.cyclonedx+protobuf"</c></returns>
        public static string Protobuf => "application/x.vnd.cyclonedx+protobuf";

        /// <summary>
        /// Utility method to return the CycloneDX media type with the specification version specified.
        /// </summary>
        /// <param name="format">The CycloneDX format. i.e. <c>Format.Xml</c></param>
        /// <param name="specificationVersion">The CycloneDX specification version. i.e. <c>SpecificationVersion.v1_3</c></param>
        public static string GetMediaType(SerializationFormat format, SpecificationVersion specificationVersion)
        {
            var mediaType = GetMediaType(format);

            var version = "1.5";
            if (specificationVersion == SpecificationVersion.v1_4)
            {
                version = "1.4";
            }
            else if (specificationVersion == SpecificationVersion.v1_3)
            {
                version = "1.3";
            }
            else if (specificationVersion == SpecificationVersion.v1_2)
            {
                if (format == SerializationFormat.Protobuf)
                {
                    throw new UnsupportedFormatSpecificationVersionException("Protocol Buffers format is only supported from v1.3.");
                }
                version = "1.2";
            }
            else if (specificationVersion == SpecificationVersion.v1_1)
            {
                if (format == SerializationFormat.Json)
                {
                    throw new UnsupportedFormatSpecificationVersionException("JSON format is only supported from v1.2.");
                }
                if (format == SerializationFormat.Protobuf)
                {
                    throw new UnsupportedFormatSpecificationVersionException("Protocol Buffers format is only supported from v1.3.");
                }
                version = "1.1";
            }
            else if (specificationVersion == SpecificationVersion.v1_0)
            {
                if (format == SerializationFormat.Json)
                {
                    throw new UnsupportedFormatSpecificationVersionException("JSON format is only supported from v1.2.");
                }
                if (format == SerializationFormat.Protobuf)
                {
                    throw new UnsupportedFormatSpecificationVersionException("Protocol Buffers format is only supported from v1.3.");
                }
                version = "1.0";
            }
            
            return $"{mediaType}; version={version}";
        }

        /// <summary>
        /// Utility method to return the CycloneDX media type for a supported <c>Format</c>.
        /// </summary>
        /// <param name="format">The CycloneDX format. i.e. <c>Format.Xml</c></param>
        public static string GetMediaType(SerializationFormat format)
        {
            var mediaType = Xml;
            if (format == SerializationFormat.Json)
            {
                mediaType = Json;
            }
            else if (format == SerializationFormat.Protobuf)
            {
                mediaType = Protobuf;
            }

            return $"{mediaType}";
        }
    }
}