// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml.Serialization;

namespace CycloneDX.Xml
{
    internal static class XmlSerializerCache
    {
        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            internal CacheKey(Type type, string defaultNamespace)
            {
                Type = type;
                DefaultNamespace = defaultNamespace;
                RootName = null;
            }

            internal CacheKey(Type type, string rootName, string defaultNamespace)
            {
                Type = type;
                RootName = rootName;
                DefaultNamespace = defaultNamespace;
            }

            internal Type Type { get; }

            internal string RootName { get; }

            internal string DefaultNamespace { get; }

            internal bool HasRootName => RootName != null;

            public bool Equals(CacheKey other)
            {
                return Type == other.Type
                    && RootName == other.RootName
                    && DefaultNamespace == other.DefaultNamespace;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = Type.GetHashCode();
                    hash = (hash * 397) ^ (RootName?.GetHashCode() ?? 0);
                    hash = (hash * 397) ^ (DefaultNamespace?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }

        private static readonly ConcurrentDictionary<CacheKey, Lazy<XmlSerializer>> Serializers = new ConcurrentDictionary<CacheKey, Lazy<XmlSerializer>>();

        internal static XmlSerializer Get(Type type, string defaultNamespace)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var key = new CacheKey(type, defaultNamespace);
            return Serializers.GetOrAdd(key, CreateSerializer).Value;
        }

        internal static XmlSerializer Get(Type type, string rootName, string defaultNamespace)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (rootName == null)
            {
                throw new ArgumentNullException(nameof(rootName));
            }

            var key = new CacheKey(type, rootName, defaultNamespace);
            return Serializers.GetOrAdd(key, CreateSerializer).Value;
        }

        private static Lazy<XmlSerializer> CreateSerializer(CacheKey key)
        {
            return new Lazy<XmlSerializer>(() =>
            {
                if (key.HasRootName)
                {
                    return new XmlSerializer(key.Type, new XmlRootAttribute(key.RootName) { Namespace = key.DefaultNamespace });
                }

                return new XmlSerializer(key.Type, key.DefaultNamespace);
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }
}
