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
    public class Relationship
    {
        /// <summary>
        /// Id to which the SPDX element is related
        /// </summary>
        public string SpdxElementId { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// SPDX ID for SpdxElement.  A related SpdxElement.
        /// </summary>
        public string RelatedSpdxElement { get; set; }

        /// <summary>
        /// Describes the type of relationship between two SPDX elements.
        /// </summary>
        public RelationshipType RelationshipType { get; set; }
    }
}
