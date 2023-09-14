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
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using CycloneDX.Models;

namespace CycloneDX.Xml
{
    /// <summary>
    /// Contains all XML serialization methods.
    /// </summary>
    public static partial class Serializer
    {
        private static readonly Dictionary<SpecificationVersion, Dictionary<string, XmlSerializer>> _elementSerializers = new Dictionary<SpecificationVersion, Dictionary<string, XmlSerializer>>();
        private static readonly Dictionary<SpecificationVersion, XmlSerializer> _serializers = new Dictionary<SpecificationVersion, XmlSerializer>();

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
        };

        private static XmlAttributeOverrides GetOverrides(SpecificationVersion specificationVersion)
        {
            var overrideBuilder = new OverrideXml();

            overrideBuilder.Override<Bom>()
                .Attr(new XmlRootAttribute("bom") { Namespace = SpecificationVersionHelpers.XmlNamespace(specificationVersion) });

            return overrideBuilder.Commit();
        }

        private static XmlSerializer GetXmlSerializer(SpecificationVersion specificationVersion)
        {
            // This XmlSerializer caching is important, from the documentation:
            //
            // Dynamically Generated Assemblies
            //
            // To increase performance, the XML serialization infrastructure
            // dynamically generates assemblies to serialize and deserialize
            // specified types. The infrastructure finds and reuses those
            // assemblies. This behavior occurs only when using the following
            // constructors:
            //
            // XmlSerializer.XmlSerializer(Type)
            // XmlSerializer.XmlSerializer(Type, String)
            //
            // If you use any of the other constructors, multiple versions of
            // the same assembly are generated and never unloaded, which
            // results in a memory leak and poor performance. The easiest
            // solution is to use one of the previously mentioned two
            // constructors. Otherwise, you must cache the assemblies
            lock (_serializers)
            {
                if (!_serializers.ContainsKey(specificationVersion))
                {
                    var attributeOverrides = GetOverrides(specificationVersion);
                    _serializers[specificationVersion] = new XmlSerializer(typeof(Bom), attributeOverrides);
                }
                return _serializers[specificationVersion];
            }
        }
        
        /// <summary>
        /// Serializes a CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = GetXmlSerializer(bom.SpecVersion);
            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, BomUtils.GetBomForSerialization(bom));
            }
        }

        /// <summary>
        /// Serializes a CycloneDX BOM to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Bom bom)
        {
            Contract.Requires(bom != null);

            var serializer = GetXmlSerializer(bom.SpecVersion);
            using (var ms = new MemoryStream())
            {
                Serialize(bom, ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        internal static XmlSerializer GetElementSerializer<T>(SpecificationVersion specVersion, string elementName)
        {
            if (!_elementSerializers.ContainsKey(specVersion))
                _elementSerializers[specVersion] = new Dictionary<string, XmlSerializer>();
            var serKey = $"{typeof(T).FullName}:{elementName}";
            if (!_elementSerializers[specVersion].ContainsKey(serKey))
            {
                var rootAttr = new XmlRootAttribute(elementName);
                rootAttr.Namespace = SpecificationVersionHelpers.XmlNamespace(specVersion);
                _elementSerializers[specVersion][serKey] = new XmlSerializer(typeof(T), rootAttr);
            }

            return _elementSerializers[specVersion][serKey];
        }
    }
}
