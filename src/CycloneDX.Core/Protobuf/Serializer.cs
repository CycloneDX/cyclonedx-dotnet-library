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
using System.IO;
using ProtoBuf;
using CycloneDX;

namespace CycloneDX.Protobuf
{
    /// <summary>
    /// Contains all Protobuf serialization methods.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes a v1.3 CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v1_3.Bom bom, Stream outputStream)
        {
            Contract.Requires(outputStream != null);
            Contract.Requires(bom != null);
            
            ProtoBuf.Serializer.Serialize(outputStream, bom);
        }

        /// <summary>
        /// Serializes a v1.3 CycloneDX BOM returning the output as a byte array.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static byte[] Serialize(Models.v1_3.Bom bom)
        {
            var ms = new MemoryStream();
            Serialize(bom, ms);
            return ms.ToArray();
        }
    }
}