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

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class DatasetChoices : List<DatasetChoice>, IXmlSerializable
    {
        internal SpecificationVersion SpecVersion { get; set; }

        public DatasetChoices()
        {
            SpecVersion = SpecificationVersionHelpers.CurrentVersion; 
        }

        private XmlSerializer GetDatasetSerializer(string namespaceUri)
        {
            var rootAttr = new XmlRootAttribute("dataset")
            {
                Namespace = namespaceUri
            };
            return new XmlSerializer(typeof(Data), rootAttr);
        }

        public System.Xml.Schema.XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            string namespaceUri = reader.NamespaceURI; 

            while (reader.LocalName == "ref" || reader.LocalName == "dataset")
            {
                if (reader.LocalName == "ref")
                {
                    var valueString = reader.ReadElementContentAsString();
                    this.Add(new DatasetChoice { Ref = valueString });
                }
                else if (reader.LocalName == "dataset")
                {
                    var serializer = GetDatasetSerializer(namespaceUri);
                    var dataset = (Data)serializer.Deserialize(reader);
                    this.Add(new DatasetChoice { DataSet = dataset });
                }
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            string namespaceUri = SpecificationVersionHelpers.XmlNamespace(SpecVersion);
            foreach (var datasetChoice in this)
            {
                if (datasetChoice.Ref != null)
                {
                    writer.WriteElementString("ref", namespaceUri, datasetChoice.Ref);
                }
                else if (datasetChoice.DataSet != null)
                {
                    var serializer = GetDatasetSerializer(namespaceUri);
                    serializer.Serialize(writer, datasetChoice.DataSet);
                }
            }
        }
    }
}