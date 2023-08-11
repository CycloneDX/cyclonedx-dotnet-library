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

using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("hash")]
    [ProtoContract]
    public class Hash : BomEntity
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
        }

        [XmlAttribute("alg")]
        [ProtoMember(1, IsRequired=true)]
        public HashAlgorithm Alg { get; set; }
        
        [XmlText]
        [ProtoMember(2)]
        public string Content { get; set; }

        public bool Equivalent(Hash obj)
        {
            return (!(obj is null) && this.Alg == obj.Alg);
        }

        public bool MergeWith(Hash obj)
        {
            try
            {
                // Basic checks for null, type compatibility,
                // equality and non-equivalence; throws for
                // the hard stuff to implement in the catch:
                return base.MergeWith(obj);
            }
            catch (BomEntityConflictException)
            {
                // Note: Alg is non-nullable so no check for that
                if (this.Content is null && !(obj.Content is null))
                {
                    this.Content = obj.Content;
                    return true;
                }

                if (this.Content != obj.Content)
                {
                    throw new BomEntityConflictException($"Two Hash objects with same Alg='{this.Alg}' and different Content: '{this.Content}' vs. '{obj.Content}'");
                }

                // All known properties merged or were equal/equivalent
                return true;
            }

            // Should not get here
        }
    }
}