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

using System.Xml.Serialization;

namespace CycloneDX.Models.v1_0
{
    [XmlType("hash")]
    public class Hash
    {
        public enum HashAlgorithm
        {
            MD5,
            [XmlEnum(Name = "SHA-1")]
            SHA_1,
            [XmlEnum(Name = "SHA-256")]
            SHA_256,
            [XmlEnum(Name = "SHA-384")]
            SHA_384,
            [XmlEnum(Name = "SHA-512")]
            SHA_512,
            [XmlEnum(Name = "SHA3-256")]
            SHA3_256,
            [XmlEnum(Name = "SHA3-512")]
            SHA3_512,
        }

        [XmlAttribute("alg")]
        public HashAlgorithm Alg { get; set; }
        
        [XmlText]
        public string Content { get; set; }

        public Hash() {}

        public Hash(v1_1.Hash hash)
        {
            Alg = (HashAlgorithm)(int)hash.Alg;
            Content = hash.Content;
        }
    }
}