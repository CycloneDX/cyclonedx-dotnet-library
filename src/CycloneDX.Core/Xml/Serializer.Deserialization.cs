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
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CycloneDX.Models;

namespace CycloneDX.Xml
{
    public static partial class Serializer
    {
        /// <summary>
        /// Deserializes a CycloneDX XML document from a string.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Bom Deserialize(string xmlString)
        {
            Contract.Requires(xmlString != null);
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xmlString);
                writer.Flush();
                stream.Position = 0;
                return Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes a CycloneDX XML document from a stream.
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <returns></returns>
        public static Bom Deserialize(Stream xmlStream)
        {
            Contract.Requires(xmlStream != null);

            if (xmlStream.GetType() == typeof(MemoryStream))
            {
                return Deserialize((MemoryStream)xmlStream);
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    xmlStream.CopyTo(stream);
                    return Deserialize(stream);
                }
            }
        }

        private static Bom Deserialize(MemoryStream xmlStream)
        {
            Contract.Requires(xmlStream != null);

            xmlStream.Position = 0;
            string xmlns = null;

            using (var reader = XmlReader.Create(xmlStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        xmlns = reader.NamespaceURI;
                        break;
                    }
                }
            }

            xmlStream.Position = 0;

            if (xmlns != null && !SpecificationVersionHelpers.IsValidXmlNamespace(xmlns))
            {
                xmlns = null;
            }

            var serializer = new XmlSerializer(typeof(Bom), xmlns);
            var bom = (Bom)serializer.Deserialize(xmlStream);

            bom.SpecVersionString = SpecificationVersionHelpers.XmlNamespaceSpecificationVersion(xmlns);

            CleanupEmptyXmlArrays(bom);

            return bom;
        }

        private static void CleanupEmptyXmlArrays(Bom bom)
        {
            if (bom.Metadata?.Authors?.Count == 0) bom.Metadata.Authors = null;
            if (bom.Metadata?.Properties?.Count == 0) bom.Metadata.Properties = null;
            if (bom.Components?.Count == 0) bom.Components = null;
            if (bom.Services?.Count == 0) bom.Services = null;
            if (bom.ExternalReferences?.Count == 0) bom.ExternalReferences = null;
            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;
            if (bom.Compositions?.Count == 0) bom.Compositions = null;

            if (bom.Metadata?.Component != null)
                CleanupEmptyXmlArrays(bom.Metadata.Component);
            if (bom.Components != null)
            foreach (var component in bom.Components)
                CleanupEmptyXmlArrays(component);
            
            if (bom.Services != null)
            foreach (var service in bom.Services)
                CleanupEmptyXmlArrays(service);

            if (bom.Dependencies != null)
            foreach (var dependency in bom.Dependencies)
                if (dependency.Dependencies?.Count == 0) dependency.Dependencies = null;
            
            if (bom.Compositions != null)
                foreach (var composition in bom.Compositions)
                {
                    if (composition.Assemblies?.Count == 0) composition.Assemblies = null;
                    if (composition.Dependencies?.Count == 0) composition.Dependencies = null;
                }
        }

        private static void CleanupEmptyXmlArrays(Component component)
        {
            if (component.Hashes?.Count == 0) component.Hashes = null;
            if (component.ExternalReferences?.Count == 0) component.ExternalReferences = null;
            if (component.Components?.Count == 0) component.Components = null;
            if (component.Properties?.Count == 0) component.Properties = null;
            if (component.Evidence?.Copyright?.Count == 0) component.Evidence.Copyright = null;
            if (component.Evidence?.Licenses?.Count == 0) component.Evidence.Licenses = null;

            if (component.Components != null)
                foreach (var subComponent in component.Components)
                    CleanupEmptyXmlArrays(subComponent);
            
            if (component.Pedigree != null) CleanupEmptyXmlArrays(component.Pedigree);
        }

        private static void CleanupEmptyXmlArrays(Service service)
        {
            if (service.Data?.Count == 0) service.Data = null;
            if (service.ExternalReferences?.Count == 0) service.ExternalReferences = null;
            if (service.Properties?.Count == 0) service.Properties = null;
            if (service.Services?.Count == 0) service.Services = null;
            
            if (service.Services != null)
                foreach (var subComponent in service.Services)
                    CleanupEmptyXmlArrays(subComponent);
        }

        private static void CleanupEmptyXmlArrays(Pedigree pedigree)
        {
            if (pedigree.Commits?.Count == 0) pedigree.Commits = null;
            if (pedigree.Patches?.Count == 0) pedigree.Patches = null;

            if (pedigree.Ancestors?.Count == 0) pedigree.Ancestors = null;
            if (pedigree.Ancestors != null)
            foreach (var component in pedigree.Ancestors)
                CleanupEmptyXmlArrays(component);

            if (pedigree.Descendants?.Count == 0) pedigree.Descendants = null;
            if (pedigree.Descendants != null)
            foreach (var component in pedigree.Descendants)
                CleanupEmptyXmlArrays(component);

            if (pedigree.Variants?.Count == 0) pedigree.Variants = null;
            if (pedigree.Variants != null)
            foreach (var component in pedigree.Variants)
                CleanupEmptyXmlArrays(component);
        }
    }
}
