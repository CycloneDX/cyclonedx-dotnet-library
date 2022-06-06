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

namespace CycloneDX.Spdx.Models.v2_2
{
    public enum RelationshipType
    {
        VARIANT_OF,
        COPY_OF,
        PATCH_FOR,
        TEST_DEPENDENCY_OF,
        CONTAINED_BY,
        DATA_FILE_OF,
        OPTIONAL_COMPONENT_OF,
        ANCESTOR_OF,
        GENERATES,
        CONTAINS,
        OPTIONAL_DEPENDENCY_OF,
        FILE_ADDED,
        DEV_DEPENDENCY_OF,
        DEPENDENCY_OF,
        BUILD_DEPENDENCY_OF,
        DESCRIBES,
        PREREQUISITE_FOR,
        HAS_PREREQUISITE,
        PROVIDED_DEPENDENCY_OF,
        DYNAMIC_LINK,
        DESCRIBED_BY,
        METAFILE_OF,
        DEPENDENCY_MANIFEST_OF,
        PATCH_APPLIED,
        RUNTIME_DEPENDENCY_OF,
        TEST_OF,
        TEST_TOOL_OF,
        DEPENDS_ON,
        FILE_MODIFIED,
        DISTRIBUTION_ARTIFACT,
        DOCUMENTATION_OF,
        GENERATED_FROM,
        STATIC_LINK,
        OTHER,
        BUILD_TOOL_OF,
        TEST_CASE_OF,
        PACKAGE_OF,
        AMENDS,
        DESCENDANT_OF,
        FILE_DELETED,
        EXPANDED_FROM_ARCHIVE,
        DEV_TOOL_OF,
        EXAMPLE_OF,
    }
}
