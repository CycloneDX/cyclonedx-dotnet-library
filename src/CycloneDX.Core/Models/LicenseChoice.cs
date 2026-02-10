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

        [JsonPropertyName("expressionDetails")]
        [XmlIgnore]
        [ProtoMember(6)]
        public List<ExpressionDetail> ExpressionDetails { get; set; }
        public bool ShouldSerializeExpressionDetails() { return ExpressionDetails?.Count > 0; }

        [JsonPropertyName("licensing")]
        [XmlIgnore]
        [ProtoMember(7)]
        public Licensing Licensing { get; set; }

        [JsonPropertyName("properties")]
        [XmlIgnore]
        [ProtoMember(8)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

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
                XmlSerializer licensingSerializer = new XmlSerializer(typeof(Licensing), reader.NamespaceURI);
                XmlSerializer attachedTextSerializer = new XmlSerializer(typeof(AttachedText), new XmlRootAttribute("text") { Namespace = reader.NamespaceURI });

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
                    if (reader.LocalName == "expression-detailed")
                    {
                        string bomRef = reader.GetAttribute("bom-ref");
                        string expression = reader.GetAttribute("expression");
                        LicenseAcknowledgementEnumeration? acknowledgement = null;
                        if (reader.GetAttribute("acknowledgement") != null)
                        {
                            var acknowledgementStr = reader.GetAttribute("acknowledgement");
                            LicenseAcknowledgementEnumeration acknowledgementNonNull;
                            if (Enum.TryParse<LicenseAcknowledgementEnumeration>(acknowledgementStr, true, out acknowledgementNonNull))
                            {
                                acknowledgement = acknowledgementNonNull;
                            }
                        }

                        bool isEmptyExprDetailed = reader.IsEmptyElement;
                        reader.ReadStartElement();

                        List<ExpressionDetail> details = null;
                        Licensing licensing = null;
                        List<Property> properties = null;

                        if (!isEmptyExprDetailed)
                        {
                            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                            {
                                if (reader.LocalName == "details")
                                {
                                    if (details == null) { details = new List<ExpressionDetail>(); }
                                    var detail = new ExpressionDetail();
                                    detail.LicenseIdentifier = reader.GetAttribute("license-identifier");
                                    detail.BomRef = reader.GetAttribute("bom-ref");

                                    bool isEmptyDetail = reader.IsEmptyElement;
                                    reader.ReadStartElement();

                                    if (!isEmptyDetail)
                                    {
                                        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                                        {
                                            if (reader.LocalName == "text")
                                            {
                                                detail.Text = (AttachedText)attachedTextSerializer.Deserialize(reader);
                                            }
                                            else if (reader.LocalName == "url")
                                            {
                                                reader.ReadStartElement();
                                                detail.Url = reader.ReadContentAsString();
                                                reader.ReadEndElement();
                                            }
                                            else
                                            {
                                                reader.Skip();
                                            }
                                        }
                                        reader.ReadEndElement();
                                    }

                                    details.Add(detail);
                                }
                                else if (reader.LocalName == "licensing")
                                {
                                    licensing = (Licensing)licensingSerializer.Deserialize(reader);
                                }
                                else if (reader.LocalName == "properties")
                                {
                                    reader.ReadStartElement();
                                    properties = new List<Property>();
                                    var propertySerializer = new XmlSerializer(typeof(Property), reader.NamespaceURI);
                                    while (reader.LocalName == "property")
                                    {
                                        var prop = (Property)propertySerializer.Deserialize(reader);
                                        properties.Add(prop);
                                    }
                                    reader.ReadEndElement();
                                }
                                else
                                {
                                    reader.Skip();
                                }
                            }
                            reader.ReadEndElement();
                        }

                        Licenses.Add(new LicenseChoice
                        {
                            Expression = expression,
                            BomRef = bomRef,
                            Acknowledgement = acknowledgement,
                            ExpressionDetails = details,
                            Licensing = licensing,
                            Properties = properties
                        });
                        finished = false;
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
                    if (license.Expression != null && (license.ExpressionDetails != null || license.Licensing != null || license.Properties != null))
                    {
                        // expression-detailed element
                        writer.WriteStartElement("expression-detailed");
                        if (license.BomRef != null)
                        {
                            writer.WriteAttributeString("bom-ref", license.BomRef);
                        }
                        if (license.Acknowledgement.HasValue)
                        {
                            writer.WriteAttributeString("acknowledgement", license.Acknowledgement.Value.ToString().ToLower());
                        }
                        writer.WriteAttributeString("expression", license.Expression);

                        if (license.ExpressionDetails != null)
                        {
                            XmlSerializer attachedTextSerializer = new XmlSerializer(typeof(AttachedText), new XmlRootAttribute("text") { Namespace = defaultNamespace });
                            foreach (var detail in license.ExpressionDetails)
                            {
                                writer.WriteStartElement("details");
                                if (detail.LicenseIdentifier != null)
                                {
                                    writer.WriteAttributeString("license-identifier", detail.LicenseIdentifier);
                                }
                                if (detail.BomRef != null)
                                {
                                    writer.WriteAttributeString("bom-ref", detail.BomRef);
                                }
                                if (detail.Text != null)
                                {
                                    attachedTextSerializer.Serialize(writer, detail.Text, new XmlSerializerNamespaces());
                                }
                                if (detail.Url != null)
                                {
                                    writer.WriteElementString("url", defaultNamespace, detail.Url);
                                }
                                writer.WriteEndElement();
                            }
                        }

                        if (license.Licensing != null)
                        {
                            XmlSerializer licensingSerializer = new XmlSerializer(typeof(Licensing), defaultNamespace);
                            licensingSerializer.Serialize(writer, license.Licensing, new XmlSerializerNamespaces());
                        }

                        if (license.Properties != null)
                        {
                            writer.WriteStartElement("properties");
                            XmlSerializer propertySerializer = new XmlSerializer(typeof(Property), defaultNamespace);
                            foreach (var prop in license.Properties)
                            {
                                propertySerializer.Serialize(writer, prop, new XmlSerializerNamespaces());
                            }
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else if (license.Expression != null)
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
