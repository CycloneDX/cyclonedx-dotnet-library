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
using System.Xml.Serialization;

namespace CycloneDX.Spdx.Models.v2_2
{
    public class ReviewInformation
    {
        /// <summary>
        /// The name and, optionally, contact information of the person who performed the review. Values of this property must conform to the agent and tool syntax.
        /// </summary>
        [XmlElement("reviewer")]
        public string Reviewer { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The date and time at which the SpdxDocument was reviewed. This value must be in UTC and have 'Z' as its timezone indicator.
        /// </summary>
        [XmlElement("reviewDate")]
        public DateTime ReviewDate { get; set; }

    }
}
