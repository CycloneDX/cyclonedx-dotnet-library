// This file is part of the CycloneDX Tool for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CycloneDX;

namespace CycloneDX.Xml
{

    public static class XmlBomDeserializer
    {
        public static Models.v1_2.Bom Deserialize(string bom)
        {
            Contract.Requires(bom != null);

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(bom);
                writer.Flush();
                stream.Position = 0;
                return Deserialize(stream);
            }
        }

        public static Models.v1_2.Bom Deserialize(Stream stream)
        {
            return Deserialize_v1_2(stream);
        }

        public static Models.v1_2.Bom Deserialize_v1_2(string bom)
        {
            Contract.Requires(bom != null);

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(bom);
                writer.Flush();
                stream.Position = 0;
                return Deserialize_v1_2(stream);
            }
        }

        public static Models.v1_2.Bom Deserialize_v1_2(Stream stream)
        {
            Contract.Requires(stream != null);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(typeof(Models.v1_2.Bom));
            var bom = (Models.v1_2.Bom)serializer.Deserialize(stream);

            CleanupEmptyXmlArrays(bom);

            return bom;
        }

        public static void CleanupEmptyXmlArrays(Models.v1_2.Bom bom)
        {
            if (bom.Metadata?.Authors?.Count == 0) bom.Metadata.Authors = null;   
            if (bom.Metadata?.Tools?.Count == 0) bom.Metadata.Tools = null;   
            if (bom.Components?.Count == 0) bom.Components = null;   
            if (bom.Services?.Count == 0) bom.Services = null;   
            if (bom.ExternalReferences?.Count == 0) bom.ExternalReferences = null;   
            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;

            if (bom.Components != null)
            foreach (var component in bom.Components)
                CleanupEmptyXmlArrays(component);
            
            if (bom.Dependencies != null)
            foreach (var dependency in bom.Dependencies)
                if (dependency.Dependencies?.Count == 0) dependency.Dependencies = null;

            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;
        }

        public static void CleanupEmptyXmlArrays(Models.v1_2.Component component)
        {
            if (component.Hashes?.Count == 0) component.Hashes = null;
            if (component.ExternalReferences?.Count == 0) component.ExternalReferences = null;
            if (component.Components?.Count == 0) component.Components = null;

            if (component.Components != null)
            foreach (var subComponent in component.Components)
                CleanupEmptyXmlArrays(subComponent);
            
            if (component.Pedigree != null) CleanupEmptyXmlArrays(component.Pedigree);
        }

        public static void CleanupEmptyXmlArrays(Models.v1_2.Pedigree pedigree)
        {
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
