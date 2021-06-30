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
// Copyright (c) Steve Springett. All Rights Reserved.

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using CycloneDX;

namespace CycloneDX.Xml
{

    public static class Serializer
    {
        public static void Serialize(Models.v1_3.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_3.Bom));
            serializer.Serialize(outputStream, bom);
        }

        public static void Serialize(Models.v1_2.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_2.Bom));
            serializer.Serialize(outputStream, bom);
        }

        public static void Serialize(Models.v1_1.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_1.Bom));
            serializer.Serialize(outputStream, bom);
        }

        public static void Serialize(Models.v1_0.Bom bom, Stream outputStream)
        {
            Contract.Requires(bom != null);

            var serializer = new XmlSerializer(typeof(Models.v1_0.Bom));
            serializer.Serialize(outputStream, bom);
        }

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
