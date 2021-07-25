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

    public static class Serializer
    {
        public static void Serialize(Models.v1_3.Bom bom, Stream outputStream)
        {
            Contract.Requires(outputStream != null);
            Contract.Requires(bom != null);
            
            ProtoBuf.Serializer.Serialize(outputStream, bom);
            outputStream.Position = 0;
        }

        [Obsolete("Serialize(Stream, Models.v1_3.Bom) is deprecated, use Serialize(Models.v1_3.Bom, Stream) instead.")]
        public static void Serialize(Stream outputStream, Models.v1_3.Bom bom)
        {
            Serialize(bom, outputStream);
        }

        public static byte[] Serialize(Models.v1_3.Bom bom)
        {
            var ms = new MemoryStream();
            Serialize(bom, ms);
            return ms.ToArray();
        }
    }
}