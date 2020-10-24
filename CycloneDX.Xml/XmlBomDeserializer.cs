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
using CycloneDX.Models;

namespace CycloneDX.Xml
{

    public static class XmlBomDeserializer
    {
        public static Bom Deserialize(string bom)
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

        public static Bom Deserialize(Stream stream)
        {
            Contract.Requires(stream != null);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(typeof(Bom));
            var bom = (Bom)serializer.Deserialize(stream);

            CleanupEmptyXmlArrays(bom);

            return bom;
        }

        public static void CleanupEmptyXmlArrays(Bom bom)
        {
            if (bom.Metadata?.Authors?.Count == 0) bom.Metadata.Authors = null;   
            if (bom.Metadata?.Tools?.Count == 0) bom.Metadata.Tools = null;   
            if (bom.Components?.Count == 0) bom.Components = null;   

            if (bom.Components != null)
            foreach (var component in bom.Components)
                CleanupEmptyXmlArrays(component);
            
            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;
        }

        public static void CleanupEmptyXmlArrays(Component component)
        {
            if (component.Hashes?.Count == 0) component.Hashes = null;
            if (component.ExternalReferences?.Count == 0) component.ExternalReferences = null;
            if (component.Components?.Count == 0) component.Components = null;

            if (component.Components != null)
            foreach (var subComponent in component.Components)
                CleanupEmptyXmlArrays(subComponent);
        }
    }
}
