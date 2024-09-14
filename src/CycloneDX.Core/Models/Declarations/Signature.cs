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
using System.Collections.Generic;
using System.Text;

namespace CycloneDX.Models
{
    public class Signature
    {
        public string Algorithm { get; set; }
        public string KeyId { get; set; }
        public PublicKey PublicKey { get; set; }
        public List<string> CertificatePath { get; set; }
        public List<string> Excludes { get; set; }
        public string Value { get; set; }
    }

    public enum KeyTypeIndicator
    {
        EC,
        OKP,
        RSA,
    }

    public class PublicKey
    {
        public KeyTypeIndicator Kty { get; set; }
        // curve
        public string Crv { get; set; }
        // curve point x
        public string X { get; set; }
        // curve point y
        public string Y { get; set; }
        // RSA modulus
        public string N { get; set; }
        // RSA exponent
        public string E { get; set; }

    }
}
