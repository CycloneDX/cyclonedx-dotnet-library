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
using System.Linq;
using System.Text.Json;
using CycloneDX.Models;

namespace CycloneDX.Spdx.Interop.Helpers
{
    public static class General
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
        };

        public static string Base64Encode(this string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void AddSpdxElement(this List<Property> properties, string propertyName, string spdxElement)
        {
            if (spdxElement != null)
            {
                properties.Add(new Property
                {
                    Name = propertyName,
                    Value = spdxElement,
                });
            }
        }

        public static void AddSpdxElements(this List<Property> properties, string propertyName, List<string> spdxElements)
        {
            if (spdxElements != null)
            {
                foreach (var spdxElement in spdxElements)
                {
                    properties.AddSpdxElement(propertyName, spdxElement);
                }
            }
        }

        public static void AddSpdxElement<T>(this List<Property> properties, string propertyName, T spdxElement)
        {
            if (!EqualityComparer<T>.Default.Equals(spdxElement, default(T)))
            {
                properties.Add(new Property
                {
                    Name = propertyName,
                    Value = JsonSerializer.Serialize<T>(spdxElement, _jsonOptions),
                });
            }
        }

        public static void AddSpdxElements<T>(this List<Property> properties, string propertyName, List<T> spdxElements)
        {
            if (spdxElements != null)
            {
                foreach (var spdxElement in spdxElements)
                {
                    properties.AddSpdxElement<T>(propertyName, spdxElement);
                }
            }
        }

        private static T GetSpdxElement<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value, _jsonOptions);
        }

        public static string GetSpdxElement(this List<Property> properties, string propertyName)
        {
            var result = properties.GetSpdxElements(propertyName);
            return result == null || result.Count == 0 ? null : result.First();
        }

        public static List<string> GetSpdxElements(this List<Property> properties, string propertyName)
        {
            if (properties.Exists(p => p.Name == propertyName))
            {
                var spdxElements = new List<string>();
                foreach (var p in properties.Where(p => p.Name == propertyName))
                {
                    spdxElements.Add(p.Value);
                }
                return spdxElements.Count > 0 ? spdxElements : null;
            }
            else
            {
                return null;
            }
        }

        public static T GetSpdxElement<T>(this List<Property> properties, string propertyName)
        {
            var result = properties.GetSpdxElements<T>(propertyName);
            return result == null || result.Count == 0 ? default(T) : result.First();
        }

        public static List<T> GetSpdxElements<T>(this List<Property> properties, string propertyName)
        {
            if (properties.Exists(p => p.Name == propertyName))
            {
                var spdxElements = new List<T>();
                foreach (var p in properties.Where(p => p.Name == propertyName))
                {
                    spdxElements.Add(GetSpdxElement<T>(p.Value));
                }
                return spdxElements.Count > 0 ? spdxElements : null;
            }
            else
            {
                return null;
            }
        }
    }
}
