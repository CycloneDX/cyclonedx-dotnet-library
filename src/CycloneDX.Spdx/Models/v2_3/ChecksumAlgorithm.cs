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
// SPDX_License_Identifier: Apache_2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_3
{
    public enum ChecksumAlgorithm
    {
        [XmlEnum("SHA256")]
        SHA256,
        [XmlEnum("SHA1")]
        SHA1,
        [XmlEnum("SHA384")]
        SHA384,
        [XmlEnum("MD2")]
        MD2,
        [XmlEnum("MD4")]
        MD4,
        [XmlEnum("SHA512")]
        SHA512,
        [XmlEnum("MD6")]
        MD6,
        [XmlEnum("MD5")]
        MD5,
        [XmlEnum("SHA224")]
        SHA224,
        [XmlEnum("SHA3-256")]
        SHA3_256,
        [XmlEnum("SHA3-384")]
        SHA3_384,
        [XmlEnum("SHA3-512")]
        SHA3_512,
        [XmlEnum("BLAKE2b-256")]
        BLAKE2b_256,
        [XmlEnum("BLAKE2b-384")]
        BLAKE2b_384,
        [XmlEnum("BLAKE2b-512")]
        BLAKE2b_512,
        [XmlEnum("BLAKE3")]
        BLAKE3,
        [XmlEnum("ADLER32")]
        ADLER32,
    }
}
