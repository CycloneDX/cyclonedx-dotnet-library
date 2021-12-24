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

namespace CycloneDX.Spdx
{
    public static class PropertyTaxonomy
    {
        public const string SPDXID = "spdx:spdxid";
        public const string ANNOTATION = "spdx:annotation";
        public const string FILES_ANALYZED = "spdx:files-analyzed";
        public const string DOWNLOAD_LOCATION = "spdx:download-location";
        public const string HOMEPAGE = "spdx:homepage";
        public const string LICENSE_COMMENTS = "spdx:license-comments";
        public const string LICENSE_CONCLUDED = "spdx:license-concluded";
        public const string LICENSE_DECLARED = "spdx:license-declared";
        public const string LICENSE_INFO_FROM_FILE = "spdx:license-info-from-file";
        public const string PACKAGE_ORIGINATOR = "spdx:package:originator";
        public const string PACKAGE_ORIGINATOR_ORGANIZATION = "spdx:package:originator:organization";
        public const string PACKAGE_ORIGINATOR_EMAIL = "spdx:package:originator:email";
        public const string PACKAGE_SUPPLIER = "spdx:package:supplier";
        public const string PACKAGE_SUPPLIER_ORGANIZATION = "spdx:package:supplier:organization";
        public const string PACKAGE_FILENAME = "spdx:package:file-name";
        public const string PACKAGE_VERIFICATION_CODE_VALUE = "spdx:package:verification-code:value";
        public const string PACKAGE_VERIFICATION_CODE_EXCLUDED_FILE = "spdx:package:verification-code:excluded-file";
        public const string PACKAGE_SOURCE_INFO = "spdx:package:source-info";
        public const string PACKAGE_SUMMARY = "spdx:package:summary";
        public const string FILE_TYPE = "spdx:file:type";
        public const string FILE_CONTRIBUTOR = "spdx:file:contributor";
        public const string FILE_NOTICE_TEXT = "spdx:file:notice-text";
        public const string CHECKSUM = "spdx:checksum";
        public const string CHECKSUM_SHA224 = "spdx:checksum:sha224";
        public const string CHECKSUM_MD2 = "spdx:checksum:md2";
        public const string CHECKSUM_MD4 = "spdx:checksum:md4";
        public const string CHECKSUM_MD6 = "spdx:checksum:md6";
        public const string DOCUMENT_SPDX_VERSION = "spdx:document:spdx-version";
        public const string DOCUMENT_DATA_LICENSE = "spdx:document:data-license";
        public const string DOCUMENT_NAME = "spdx:document:name";
        public const string COMMENT = "spdx:comment";
        public const string DOCUMENT_NAMESPACE = "spdx:document:document-namespace";
        public const string DOCUMENT_EXTERNAL_DOCUMENT_REF = "spdx:document:external-document-ref";
        public const string DOCUMENT_DESCRIBES = "spdx:document:describes";
        public const string CREATION_INFO_COMMENT = "spdx:creation-info:comment";
        public const string CREATION_INFO_LICENSE_LIST_VERSION = "spdx:creation-info:license-list-version";
        public const string CREATION_INFO_LICENSE_CREATORS_ORGANIZATIONS = "spdx:creation-info:creators-organization";
        public const string EXTERNAL_REFERENCE = "spdx:external-reference";
        public const string EXTERNAL_REFERENCE_SECURITY = "spdx:external-reference:security";
        public const string EXTERNAL_REFERENCE_SECURITY_CPE22 = "spdx:external-reference:security:cpe22";
        public const string EXTERNAL_REFERENCE_SECURITY_CPE23 = "spdx:external-reference:security:cpe23";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER = "spdx:external-reference:package-manager";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER_MAVEN_CENTRAL = "spdx:external-reference:package-manager:maven-central";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER_NPM = "spdx:external-reference:package-manager:npm";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER_NUGET = "spdx:external-reference:package-manager:nuget";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER_PURL = "spdx:external-reference:package-manager:purl";
        public const string EXTERNAL_REFERENCE_PACKAGE_MANAGER_BOWER = "spdx:external-reference:package-manager:bower";
        public const string EXTERNAL_REFERENCE_PERSISTENT_ID = "spdx:external-reference:persistent-id";
        public const string EXTERNAL_REFERENCE_PERSISTENT_ID_SWH = "spdx:external-reference:persistent-id:swh";
        public const string EXTERNAL_REFERENCE_OTHER = "spdx:external-reference:other";
    }
}
