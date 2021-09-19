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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CycloneDX;

namespace CycloneDX.Xml
{
    /// <summary>
    /// Contains all XML serialization methods.
    /// </summary>
    public static class Serializer
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };
        
        /// <summary>
        /// Serializes a v1.3 CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v1_3.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_3.Bom));
            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, bom);
            }
        }

        /// <summary>
        /// Serializes a v1.2 CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v1_2.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_2.Bom));
            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, bom);
            }
        }

        /// <summary>
        /// Serializes a v1.1 CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v1_1.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_1.Bom));
            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, bom);
            }
        }

        /// <summary>
        /// Serializes a v1.0 CycloneDX BOM writing the output to a stream.
        /// </summary>
        /// <param name="bom"></param>
        /// <param name="outputStream"></param>
        public static void Serialize(Models.v1_0.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_0.Bom));
            using (var xmlWriter = XmlWriter.Create(outputStream, WriterSettings))
            {
                serializer.Serialize(xmlWriter, bom);
            }
        }

        /// <summary>
        /// Serializes a v1.3 CycloneDX BOM to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Models.v1_3.Bom bom)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_3.Bom));

            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, bom);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Serializes a v1.2 CycloneDX BOM to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Models.v1_2.Bom bom)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_2.Bom));

            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, bom);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Serializes a v1.1 CycloneDX BOM to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Models.v1_1.Bom bom)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_1.Bom));

            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, bom);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Serializes a v1.0 CycloneDX BOM to a string.
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public static string Serialize(Models.v1_0.Bom bom)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_0.Bom));

            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, bom);
                return writer.ToString();
            }
        }
    }
}
