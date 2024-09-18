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

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    public enum ImplementationPlatform
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
        [XmlEnum("generic")]
        Generic,
        [XmlEnum("x86_32")]
        [EnumMember(Value = "x86_32")]
        X86_32,
        [XmlEnum("x86_64")]
        [EnumMember(Value = "x86_64")]
        X86_64,
        [XmlEnum("armv7-a")]
        [EnumMember(Value = "armv7-a")]
        Armv7A,
        [XmlEnum("armv7-m")]
        [EnumMember(Value = "armv7-m")]
        Armv7M,
        [XmlEnum("armv8-a")]
        [EnumMember(Value = "armv8-a")]
        Armv8A,
        [XmlEnum("armv8-m")]
        [EnumMember(Value = "armv8-m")]
        Armv8M,
        [XmlEnum("armv9-a")]
        [EnumMember(Value = "armv9-a")]
        Armv9A,
        [XmlEnum("armv9-m")]
        [EnumMember(Value = "armv9-m")]
        Armv9M,
        [XmlEnum("s390x")]
        S390X,
        [XmlEnum("ppc64")]
        Ppc64,
        [XmlEnum("ppc64le")]
        Ppc64le
    }
}
