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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

using CycloneDX;

namespace CycloneDX.Xml
{
    /// <summary>
    /// Contains all XML validation methods.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Validate the stream contents represent a valid CycloneDX XML document.
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="specificationVersion"></param>
        /// <returns></returns>
        public static Models.ValidationResult Validate(Stream xmlStream, SpecificationVersion specificationVersion)
        {
            var validationMessages = new List<string>();

            var schemaVersionString = specificationVersion.ToString().Substring(1).Replace('_', '.');
            var expectedNamespaceUri = $"http://cyclonedx.org/schema/bom/{schemaVersionString}";

            var assembly = typeof(Validator).GetTypeInfo().Assembly;
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-{schemaVersionString}.xsd"))
            using (var spdxStream = assembly.GetManifestResourceStream("CycloneDX.Core.Schemas.spdx.xsd"))
            {
                var settings = new XmlReaderSettings();

                settings.Schemas.Add(XmlSchema.Read(schemaStream, null));
                settings.Schemas.Add(XmlSchema.Read(spdxStream, null));
                settings.ValidationType = ValidationType.Schema;
            
                using (var reader = XmlReader.Create(xmlStream, settings))
                {
                    var document = new XmlDocument();

                    try
                    {
                        document.Load(reader);

                        if (document.DocumentElement.NamespaceURI != expectedNamespaceUri)
                        {
                            validationMessages.Add($"Invalid namespace URI: expected {expectedNamespaceUri} actual {document.DocumentElement.NamespaceURI}");
                        }
                    }
                    catch (XmlSchemaValidationException exc)
                    {
                        var lineInfo = ((IXmlLineInfo)reader);
                        if (lineInfo.HasLineInfo()) {
                            validationMessages.Add($"Validation failed at line number {lineInfo.LineNumber} and position {lineInfo.LinePosition}: {exc.Message}");
                        }
                        else
                        {
                            validationMessages.Add($"Validation failed at position {xmlStream.Position}: {exc.Message}");
                        }
                    }
                    catch (XmlException exc)
                    {
                        validationMessages.Add(exc.Message);
                    }
                }
            }

            return new Models.ValidationResult
            {
                Valid = validationMessages.Count == 0,
                Messages = validationMessages
            };
        }

        /// <summary>
        /// Validate the string contents represent a valid CycloneDX XML document.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="schemaVersion"></param>
        /// <returns></returns>
        public static Models.ValidationResult Validate(string xmlString, SpecificationVersion schemaVersion)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlString);
                writer.Flush();
                stream.Position = 0;
                return Validate(stream, schemaVersion);
            }
        }
    }
}
