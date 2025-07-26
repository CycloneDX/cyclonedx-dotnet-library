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
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{


    [ProtoContract]
    public class LicenseChoice
    {
        [XmlElement("license")]
        [ProtoMember(1)]
        public License License { get; set; }

        [XmlElement("expression")]
        [ProtoMember(2)]
        public string Expression { get; set; }

        [XmlAttribute("bom-ref")]
        [JsonPropertyName("bom-ref")]
        [ProtoMember(4)]
        public string BomRef { get; set; }


        [XmlAttribute("acknowledgement")]
        [ProtoMember(3)]
        public LicenseAcknowledgementEnumeration? Acknowledgement { get; set; }
        public bool ShouldSerializeAcknowledgement() { return Acknowledgement.HasValue; }

    }

    // This is a workaround to serialize licenses correctly
    public class LicenseChoiceList : IXmlSerializable
    {
        public LicenseChoiceList(List<LicenseChoice> licenses)
        {
            Licenses = licenses;
        }

        public LicenseChoiceList() { }

        public List<LicenseChoice> Licenses { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                XmlSerializer licenseSerializer = new XmlSerializer(typeof(License), reader.NamespaceURI);

                Licenses = new List<LicenseChoice>();

                bool finished = false;
                while (!finished)
                {
                    finished = true;
                    if (licenseSerializer.CanDeserialize(reader))
                    {
                        var license = (License)licenseSerializer.Deserialize(reader);
                        Licenses.Add(new LicenseChoice { License = license });
                        finished = false;
                    }
                    if (reader.LocalName == "expression")
                    {
                        string bomRef = null;
                        LicenseAcknowledgementEnumeration? acknowledgement = null;
                        if (reader.GetAttribute("bom-ref") != null)
                        {
                            bomRef = reader.GetAttribute("bom-ref");
                        }
                        if (reader.GetAttribute("acknowledgement") != null)
                        {
                            var acknowledgementStr = reader.GetAttribute("acknowledgement");
                            LicenseAcknowledgementEnumeration acknowledgementNonNull;
                            if (Enum.TryParse<LicenseAcknowledgementEnumeration>(acknowledgementStr, true, out acknowledgementNonNull))
                            {
                                acknowledgement = acknowledgementNonNull;
                            }
                        }
                        reader.ReadStartElement();
                        var expression = reader.ReadContentAsString();
                        Licenses.Add(new LicenseChoice { Expression = expression, BomRef = bomRef, Acknowledgement = acknowledgement });                        
                        finished = false;
                        reader.ReadEndElement();
                    }


                }

                reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            if (Licenses != null)
            {
                // todo: is there a way to feed in the namespace with having to introduce WriterToNamespace?
                string defaultNamespace;
                defaultNamespace = CycloneDX.Xml.Serializer.GetNamespace(writer);
                XmlSerializer licenseSerializer = new XmlSerializer(typeof(License), defaultNamespace);

                foreach (var license in Licenses)
                {
                    if (license.License != null)
                    {
                        licenseSerializer.Serialize(writer, license.License, new XmlSerializerNamespaces());
                    }
                    if (license.Expression != null)
                    {
                        writer.WriteStartElement("expression");
                        if (license.BomRef != null)
                        {
                            writer.WriteAttributeString("bom-ref", license.BomRef);
                        }
                        if (license.Acknowledgement.HasValue)
                        {
                            writer.WriteAttributeString("acknowledgement", license.Acknowledgement.Value.ToString().ToLower());
                        }
                        writer.WriteString(license.Expression);
                        writer.WriteEndElement();
                    }
                }
            }

        }
    }
}
