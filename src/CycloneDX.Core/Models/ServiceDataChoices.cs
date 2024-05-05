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
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class ServiceDataChoices : IXmlSerializable
    {
        internal SpecificationVersion SpecVersion { get; set; }

        public List<DataClassification> DataClassifications { get; set; }
        
        public List<DataFlow> DataFlows { get; set; }

        public ServiceDataChoices()
        {
            SpecVersion = SpecificationVersionHelpers.CurrentVersion;
        }
        
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            SpecVersion = SpecificationVersionHelpers.Version(SpecificationVersionHelpers.XmlNamespaceSpecificationVersion(reader.NamespaceURI));
            while (reader.LocalName == "classification" || reader.LocalName == "dataflow")
            {
                if (reader.LocalName == "classification")
                {
                    if (this.DataClassifications == null) this.DataClassifications = new List<DataClassification>();
                    var serializer = Xml.Serializer.GetElementSerializer<DataClassification>(SpecVersion, "classification");
                    var dataClassification = (DataClassification)serializer.Deserialize(reader);
                    this.DataClassifications.Add(dataClassification);
                }
                if (reader.LocalName == "dataflow")
                {
                    if (this.DataFlows == null) this.DataFlows = new List<DataFlow>();
                    var serializer = Xml.Serializer.GetElementSerializer<DataFlow>(SpecVersion, "dataflow");
                    var dataflow = (DataFlow)serializer.Deserialize(reader);
                    this.DataFlows.Add(dataflow);
                }
            }
            reader.ReadEndElement();
        }
        
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (this.DataClassifications != null)
            {
                var serializer = Xml.Serializer.GetElementSerializer<DataClassification>(SpecVersion, "classification");
                foreach (var dc in this.DataClassifications)
                    serializer.Serialize(writer, dc);
            }

            if (this.DataFlows != null)
            {
                var serializer = Xml.Serializer.GetElementSerializer<DataFlow>(SpecVersion, "dataflow");
                foreach (var df in this.DataFlows)
                    serializer.Serialize(writer, df);
            }
        }

        public bool ShouldSerialize()
        {
            return DataClassifications?.Count > 0 || DataFlows?.Count > 0;
        }
    }
}
