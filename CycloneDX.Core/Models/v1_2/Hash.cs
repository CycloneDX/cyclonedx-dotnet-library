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

using System.Xml.Serialization;

namespace CycloneDX.Models.v1_2
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
            [XmlEnum(Name = "SHA3-384")]
            SHA3_384,
            [XmlEnum(Name = "SHA3-512")]
            SHA3_512,
            [XmlEnum(Name = "BLAKE2b-256")]
            BLAKE2b_256,
            [XmlEnum(Name = "BLAKE2b-384")]
            BLAKE2b_384,
            [XmlEnum(Name = "BLAKE2b-512")]
            BLAKE2b_512,
            BLAKE3,
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

        public Hash(v1_3.Hash hash)
        {
            Alg = (HashAlgorithm)((int)hash.Alg - 1);
            Content = hash.Content;
        }
    }
}