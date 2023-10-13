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

using static CycloneDX.SpecificationVersion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class ResourceReferenceChoice : BomEntity, IXmlSerializable, IBomEntityWithRefLinkType_String_Ref
    {
        private static XmlSerializer _extRefSerializer;
        private static XmlSerializer GetExternalReferenceSerializer()
        {
            if (_extRefSerializer == null)
            {
                _extRefSerializer = new XmlSerializer(typeof(ExternalReference), new XmlRootAttribute("externalReference"));
            }

            return _extRefSerializer;
        }
        
        [ProtoMember(1)]
        public string Ref { get; set; }

        [ProtoMember(2)]
        public ExternalReference ExternalReference { get; set; }
        
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            if (reader.LocalName == "ref")
            {
                var valueString = reader.ReadElementContentAsString();
                this.Ref = valueString;
            }
            else if (reader.LocalName == "externalReference")
            {
                var extRef = (ExternalReference)GetExternalReferenceSerializer().Deserialize(reader);
                this.ExternalReference = extRef;
            }
            reader.ReadEndElement();
        }
        
        public void WriteXml(XmlWriter writer) {
            if (this.Ref != null)
            {
                writer.WriteElementString("ref", this.Ref);
            }
            else if (this.ExternalReference != null)
            {
                GetExternalReferenceSerializer().Serialize(writer, this.ExternalReference);
            }
        }

        private static readonly ImmutableDictionary<PropertyInfo, ImmutableList<Type>> RefLinkConstraints_StringRef_AnyBomEntity =
        new Dictionary<PropertyInfo, ImmutableList<Type>>
        {
            { typeof(ResourceReferenceChoice).GetProperty("Ref", typeof(string)), RefLinkConstraints_AnyBomEntity }
        }.ToImmutableDictionary();

        public ImmutableDictionary<PropertyInfo, ImmutableList<Type>> GetRefLinkConstraints(SpecificationVersion specificationVersion)
        {
            // TODO: switch/case for CDX spec newer than 1.5 where this type got introduced
            if (specificationVersion == v1_5)
            {
                return RefLinkConstraints_StringRef_AnyBomEntity;
            }
            return null;
        }
    }
}
