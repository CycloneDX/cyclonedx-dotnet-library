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
using System.Text.Json;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("hash")]
    [ProtoContract]
    public class Hash : IEquatable<Hash>
#if NET8_0_OR_GREATER
        , IMergeable<Hash>, IEquivalent<Hash>
#endif
    {
        [ProtoContract]
        public enum HashAlgorithm
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "MD5")]
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
            [XmlEnum(Name = "BLAKE3")]
            BLAKE3,
            [XmlEnum(Name = "Streebog-256")]
            Streebog_256,
            [XmlEnum(Name = "Streebog-512")]
            Streebog_512,
        }

        [XmlAttribute("alg")]
        [ProtoMember(1, IsRequired=true)]
        public HashAlgorithm Alg { get; set; }
        
        [XmlText]
        [ProtoMember(2)]
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Hash;
            if (other == null)
            {
                return false;
            }

            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(other, Json.Serializer.SerializerOptionsForHash);
        }

        public bool Equals(Hash obj)
        {
            return obj != null && JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(obj, Json.Serializer.SerializerOptionsForHash);
        }

        public override int GetHashCode()
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash).GetHashCode();
        }

#if NET8_0_OR_GREATER
        public bool Equivalent(Hash obj, MergeStrategy strategy)
        {
            return obj != null && Alg == obj.Alg;
        }

        /// <summary>
        /// Two hashes of the same algorithm should carry the same content;
        /// if one side is missing content (e.g. partially populated by a
        /// producer), fill it in from the other. A genuine content mismatch
        /// for the same algorithm is a real conflict, not something this
        /// merge silently resolves.
        /// </summary>
        public bool MergeWith(Hash obj, MergeStrategy strategy)
        {
            if (obj is null)
            {
                return false;
            }
            if (Equals(obj))
            {
                return true;
            }
            if (!Equivalent(obj, strategy))
            {
                return false;
            }

            if (Content is null && !(obj.Content is null))
            {
                Content = obj.Content;
                return true;
            }

            return Content == obj.Content;
        }
#endif
    }
}