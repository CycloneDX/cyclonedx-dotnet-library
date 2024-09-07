﻿// This file is part of CycloneDX Library for .NET
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
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class Note : IEquatable<Note>
    {
        [XmlElement("locale")]
        [ProtoMember(1)]
        public string Locale { get; set; }

        [XmlElement("text")]
        [ProtoMember(2)]
        public AttachedText Text { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Note);
        }

        public bool Equals(Note obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this.Locale, obj.Locale) ||
                this.Locale.Equals(obj.Locale, StringComparison.InvariantCultureIgnoreCase)) &&
                (object.ReferenceEquals(this.Text, obj.Text) ||
                this.Text.Equals(obj.Text));
        }
    }
}
