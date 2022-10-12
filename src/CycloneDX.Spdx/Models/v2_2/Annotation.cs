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
    public class Annotation
    {
        /// <summary>
        /// Identify when the comment was made. This is to be specified according to the combined date and time in the UTC format, as specified in the ISO 8601 standard.
        /// </summary>
        [XmlElement("annotationDate")]
        public DateTime AnnotationDate { get; set; }

        /// <summary>
        /// Type of the annotation.
        /// </summary>
        [XmlElement("annotationType")]
        public AnnotationType AnnotationType { get; set; }

        /// <summary>
        /// This field identifies the person, organization or tool that has commented on a file, package, or the entire document.
        /// </summary>
        [XmlElement("annotator")]
        public string Annotator { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }
    }
}
