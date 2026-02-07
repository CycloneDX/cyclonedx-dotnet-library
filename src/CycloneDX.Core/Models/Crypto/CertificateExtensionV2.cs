// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Core.Models
{
    /// <summary>
    /// Proto enum matching CertificateExtensions.CommonExtensionName
    /// </summary>
    [ProtoContract]
    public enum CommonExtensionNameEnum
    {
        [XmlEnum(Name = "unspecified")]
        Unspecified = 0,
        [XmlEnum(Name = "basicConstraints")]
        BasicConstraints = 1,
        [XmlEnum(Name = "keyUsage")]
        KeyUsage = 2,
        [XmlEnum(Name = "extendedKeyUsage")]
        ExtendedKeyUsage = 3,
        [XmlEnum(Name = "subjectAlternativeName")]
        SubjectAlternativeName = 4,
        [XmlEnum(Name = "authorityKeyIdentifier")]
        AuthorityKeyIdentifier = 5,
        [XmlEnum(Name = "subjectKeyIdentifier")]
        SubjectKeyIdentifier = 6,
        [XmlEnum(Name = "authorityInformationAccess")]
        AuthorityInformationAccess = 7,
        [XmlEnum(Name = "certificatePolicies")]
        CertificatePolicies = 8,
        [XmlEnum(Name = "crlDistributionPoints")]
        CrlDistributionPoints = 9,
        [XmlEnum(Name = "signedCertificateTimestamp")]
        SignedCertificateTimestamp = 10,
    }

    internal static class CommonExtensionNameHelper
    {
        internal static CommonExtensionNameEnum FromString(string name)
        {
            switch (name)
            {
                case "basicConstraints": return CommonExtensionNameEnum.BasicConstraints;
                case "keyUsage": return CommonExtensionNameEnum.KeyUsage;
                case "extendedKeyUsage": return CommonExtensionNameEnum.ExtendedKeyUsage;
                case "subjectAlternativeName": return CommonExtensionNameEnum.SubjectAlternativeName;
                case "authorityKeyIdentifier": return CommonExtensionNameEnum.AuthorityKeyIdentifier;
                case "subjectKeyIdentifier": return CommonExtensionNameEnum.SubjectKeyIdentifier;
                case "authorityInformationAccess": return CommonExtensionNameEnum.AuthorityInformationAccess;
                case "certificatePolicies": return CommonExtensionNameEnum.CertificatePolicies;
                case "crlDistributionPoints": return CommonExtensionNameEnum.CrlDistributionPoints;
                case "signedCertificateTimestamp": return CommonExtensionNameEnum.SignedCertificateTimestamp;
                default: return CommonExtensionNameEnum.Unspecified;
            }
        }

        internal static string ToString(CommonExtensionNameEnum value)
        {
            switch (value)
            {
                case CommonExtensionNameEnum.BasicConstraints: return "basicConstraints";
                case CommonExtensionNameEnum.KeyUsage: return "keyUsage";
                case CommonExtensionNameEnum.ExtendedKeyUsage: return "extendedKeyUsage";
                case CommonExtensionNameEnum.SubjectAlternativeName: return "subjectAlternativeName";
                case CommonExtensionNameEnum.AuthorityKeyIdentifier: return "authorityKeyIdentifier";
                case CommonExtensionNameEnum.SubjectKeyIdentifier: return "subjectKeyIdentifier";
                case CommonExtensionNameEnum.AuthorityInformationAccess: return "authorityInformationAccess";
                case CommonExtensionNameEnum.CertificatePolicies: return "certificatePolicies";
                case CommonExtensionNameEnum.CrlDistributionPoints: return "crlDistributionPoints";
                case CommonExtensionNameEnum.SignedCertificateTimestamp: return "signedCertificateTimestamp";
                default: return null;
            }
        }
    }

    /// <summary>
    /// Certificate extension for XML/JSON serialization.
    /// For protobuf, this maps to Extension { oneof { CommonExtension, CustomExtension } }
    /// </summary>
    [ProtoContract]
    public class CertificateExtensionV2
    {
        [XmlElement("commonExtensionName")]
        public string CommonExtensionName { get; set; }

        [XmlElement("commonExtensionValue")]
        public string CommonExtensionValue { get; set; }

        [XmlElement("customExtensionName")]
        public string CustomExtensionName { get; set; }

        [XmlElement("customExtensionValue")]
        public string CustomExtensionValue { get; set; }

        // Protobuf: oneof extension_type { CommonExtension=1; CustomExtension=2; }
        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(1)]
        public CertExtCommon CommonExtensionProto
        {
            get
            {
                if (CommonExtensionName == null && CommonExtensionValue == null) return null;
                return new CertExtCommon
                {
                    Name = CommonExtensionNameHelper.FromString(CommonExtensionName),
                    Value = CommonExtensionValue
                };
            }
            set
            {
                if (value == null) { CommonExtensionName = null; CommonExtensionValue = null; return; }
                CommonExtensionName = CommonExtensionNameHelper.ToString(value.Name);
                CommonExtensionValue = value.Value;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(2)]
        public CertExtCustom CustomExtensionProto
        {
            get
            {
                if (CustomExtensionName == null && CustomExtensionValue == null) return null;
                return new CertExtCustom { Name = CustomExtensionName, Value = CustomExtensionValue };
            }
            set
            {
                if (value == null) { CustomExtensionName = null; CustomExtensionValue = null; return; }
                CustomExtensionName = value.Name;
                CustomExtensionValue = value.Value;
            }
        }
    }

    /// <summary>
    /// Proto: message CommonExtension { CommonExtensionName name = 1; string value = 2; }
    /// </summary>
    [ProtoContract]
    public class CertExtCommon
    {
        [ProtoMember(1)]
        public CommonExtensionNameEnum Name { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }
    }

    /// <summary>
    /// Proto: message CustomExtension { string name = 1; optional string value = 2; }
    /// </summary>
    [ProtoContract]
    public class CertExtCustom
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }
    }

    /// <summary>
    /// Protobuf wrapper for CertificateExtensions.
    /// Proto: message CertificateExtensions { repeated Extension extensions = 1; }
    /// </summary>
    [ProtoContract]
    public class CertificateExtensionsProto
    {
        [ProtoMember(1)]
        public List<CertificateExtensionV2> Extensions { get; set; }
    }
}
