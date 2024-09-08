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
using System.Xml;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    public class EvidenceTools : List<string>, ICloneable, IXmlSerializable
    {
        private readonly string _elementName = "tool";

        public object Clone()
        {
            return new EvidenceTools()
            {
                //to do determine how to handle this
            };
        }

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.LocalName == _elementName)
            {
                this.Add(reader.GetAttribute("ref"));
                reader.Read();
            }
            reader.ReadEndElement();
        }
        
        public void WriteXml(XmlWriter writer) {
            foreach (var bomref in this)
            {
                writer.WriteStartElement(_elementName);
                writer.WriteAttributeString("ref", bomref);
                writer.WriteEndElement();
            }
        }
    }
}
