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

using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Xml;

namespace CycloneDX.Spdx.Serialization
{
    public static class XmlSerializer
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
        };

        /// <summary>
        /// Serializes a Spdx XML to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Models.v2_3.SpdxDocument document)
        {
            Contract.Requires(document != null);

            using (var ms = new MemoryStream())
            {
                Serialize(document, ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Serializes a Spdx XML writing the output to a stream.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v2_3.SpdxDocument document, Stream outputStream)
        {
            Contract.Requires(document != null);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.v2_3.SpdxDocument));

            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, document);
            }
        }

        /// <summary>
        /// Deserializes a Spdx XML document from a string.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Models.v2_3.SpdxDocument Deserialize(string xmlString)
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
        /// Deserializes a Spdx XML document from a stream.
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <returns></returns>
        public static Models.v2_3.SpdxDocument Deserialize(Stream xmlStream)
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

        private static Models.v2_3.SpdxDocument Deserialize(MemoryStream xmlStream)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.v2_3.SpdxDocument));

            var document = (Models.v2_3.SpdxDocument)serializer.Deserialize(xmlStream);

            return document;
        }
    }
}
