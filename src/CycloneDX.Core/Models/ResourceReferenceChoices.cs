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
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class ResourceReferenceChoices : List<ResourceReferenceChoice>, IEquatable<ResourceReferenceChoices>, IXmlSerializable
    {
        private string _elementName = "resourceReference";

        public override bool Equals(object obj)
        {
            return Equals(obj as ResourceReferenceChoices);
        }

        public bool Equals(ResourceReferenceChoices obj)
        {
            return obj != null &&
                (object.ReferenceEquals(this, obj) ||
                this.Equals(obj));
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.LocalName == _elementName)
            {
                var resRefChoice = new ResourceReferenceChoice();
                resRefChoice.ReadXml(reader);
                this.Add(resRefChoice);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var resRefChoice in this)
            {
                writer.WriteStartElement(_elementName);
                resRefChoice.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

    }
}
