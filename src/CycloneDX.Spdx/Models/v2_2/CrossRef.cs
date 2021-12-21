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
    public class CrossRef
    {
        /// <summary>
        /// True if the License SeeAlso URL points to a Wayback archive
        /// </summary>
        public bool IsWayBackLink { get; set; }

        /// <summary>
        /// Status of a License List SeeAlso URL reference if it refers to a website that matches the license text.
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The ordinal order of this element within a list
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// URL Reference
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicate a URL is still a live accessible location on the public internet
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// True if the URL is a valid well formed URL
        /// </summary>
        public bool IsValid { get; set; }
    }
}
