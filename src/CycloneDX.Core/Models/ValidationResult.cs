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

namespace CycloneDX.Models
{
    /// <summary>
    /// The return type for all validation methods.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// <c>true</c> if the document has been successfully validated.
        /// </summary>
        /// <value></value>
        public bool Valid { get; set; }
        /// <summary>
        /// When validation fails, has one or more messages detailing why validation failed.
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public IEnumerable<string> Messages { get; set; } = new List<string>();
    }
}