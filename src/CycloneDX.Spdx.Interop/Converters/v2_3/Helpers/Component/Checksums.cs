﻿// This file is part of CycloneDX Library for .NET
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
using System.Collections.Generic;
using System.Linq;
using CycloneDX.Models;
using CycloneDX.Spdx.Models.v2_3;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class Checksums
    {
        public static List<Checksum> GetSpdxChecksums(this Component component)
        {
            var checksums = new List<Checksum>();

            if (component.Hashes != null && component.Hashes.Count > 0)
            {
                foreach (var hash in component.Hashes)
                {
                    switch (hash.Alg)
                    {
                        case Hash.HashAlgorithm.SHA_1:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA1,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA_256:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA256,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA_384:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA384,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA_512:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA512,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.MD5:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.MD5,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA3_256:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA3_256,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA3_384:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA3_384,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.SHA3_512:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA3_512,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.BLAKE2b_256:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.BLAKE2b_256,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.BLAKE2b_384:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.BLAKE2b_384,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.BLAKE2b_512:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.BLAKE2b_512,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        case Hash.HashAlgorithm.BLAKE3:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.BLAKE3,
                                ChecksumValue = hash.Content,
                            });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(hash.Alg), $"Unsupported hash algorithm: {hash.Alg}");    
                    }
                }
            }

            if (component.Properties != null && component.Properties.Exists(p => p.Name.StartsWith(PropertyTaxonomy.CHECKSUM)))
            {
                foreach (var checksum in component.Properties.Where(p => p.Name.StartsWith(PropertyTaxonomy.CHECKSUM)))
                {
                    switch (checksum.Name)
                    {
                        case PropertyTaxonomy.CHECKSUM_SHA224:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.SHA224,
                                ChecksumValue = checksum.Value,
                            });
                            break;
                        case PropertyTaxonomy.CHECKSUM_MD2:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.MD2,
                                ChecksumValue = checksum.Value,
                            });
                            break;
                        case PropertyTaxonomy.CHECKSUM_MD4:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.MD4,
                                ChecksumValue = checksum.Value,
                            });
                            break;
                        case PropertyTaxonomy.CHECKSUM_MD6:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.MD6,
                                ChecksumValue = checksum.Value,
                            });
                            break;
                        case PropertyTaxonomy.CHECKSUM_ADLER32:
                            checksums.Add(new Checksum
                            {
                                Algorithm = ChecksumAlgorithm.ADLER32,
                                ChecksumValue = checksum.Value,
                            });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(checksum.Name), $"Unsupported hash algorithm: {checksum.Name}");
                    }
                }
            }

            return checksums.Count == 0 ? null : checksums;
        }

        public static void AddSpdxChecksums(this Component component, List<Checksum> checksums)
        {
            if (checksums != null && checksums.Count > 0)
            {
                if (component.Properties == null) { component.Properties = new List<Property>(); }
                if (component.Hashes == null) { component.Hashes = new List<Hash>(); }
                foreach (var checksum in checksums)
                {
                    switch (checksum.Algorithm)
                    {
                        case ChecksumAlgorithm.SHA1:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA_1,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.SHA224:
                            component.Properties.AddSpdxElement(PropertyTaxonomy.CHECKSUM_SHA224, checksum.ChecksumValue);
                            break;
                        case ChecksumAlgorithm.SHA256:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA_256,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.SHA384:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA_384,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.SHA512:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA_512,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.MD2:
                            component.Properties.AddSpdxElement(PropertyTaxonomy.CHECKSUM_MD2, checksum.ChecksumValue);
                            break;
                        case ChecksumAlgorithm.MD4:
                            component.Properties.AddSpdxElement(PropertyTaxonomy.CHECKSUM_MD4, checksum.ChecksumValue);
                            break;
                        case ChecksumAlgorithm.MD5:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.MD5,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.MD6:
                            component.Properties.AddSpdxElement(PropertyTaxonomy.CHECKSUM_MD6, checksum.ChecksumValue);
                            break;
                        case ChecksumAlgorithm.SHA3_256:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA3_256,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.SHA3_384:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA3_384,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.SHA3_512:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.SHA3_512,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.BLAKE2b_256:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.BLAKE2b_256,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.BLAKE2b_384:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.BLAKE2b_384,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.BLAKE2b_512:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.BLAKE2b_512,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.BLAKE3:
                            component.Hashes.Add(new Hash
                            {
                                Alg = Hash.HashAlgorithm.BLAKE3,
                                Content = checksum.ChecksumValue,
                            });
                            break;
                        case ChecksumAlgorithm.ADLER32:
                            component.Properties.AddSpdxElement(PropertyTaxonomy.CHECKSUM_ADLER32, checksum.ChecksumValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(checksum.Algorithm), $"Unsupported checksum algorithm: {checksum.Algorithm}");
                    }
                }
            }
        }    
    }
}