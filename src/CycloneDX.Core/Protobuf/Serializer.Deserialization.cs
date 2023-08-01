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
using ProtoBuf;
using CycloneDX.Models;

namespace CycloneDX.Protobuf
{
    public static partial class Serializer
    {
        /// <summary>
        /// Deserializes any supported CycloneDX Protobuf message from a stream.
        /// </summary>
        /// <param name="protobufStream"></param>
        /// <returns></returns>
        public static Bom Deserialize(Stream protobufStream)
        {
            Contract.Requires(protobufStream != null);

            var bom = ProtoBuf.Serializer.Deserialize<Bom>(protobufStream);

            if (bom.Metadata?.Timestamp != null)
            {
                bom.Metadata.Timestamp = DateTime.SpecifyKind(bom.Metadata.Timestamp.Value, DateTimeKind.Utc);
            }
            
            CleanupEmptyArrays(bom);
            
            return bom;
        }

        /// <summary>
        /// Deserializes any supported CycloneDX Protobuf message from a byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Bom Deserialize(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var ms = new MemoryStream(bytes);
            var bom = Deserialize(ms);
            return bom;
        }
        
        private static void CleanupEmptyArrays(Bom bom)
        {
            if (bom.Metadata?.Authors?.Count == 0) bom.Metadata.Authors = null;
            if (bom.Components?.Count == 0) bom.Components = null;
            if (bom.Services?.Count == 0) bom.Services = null;
            if (bom.ExternalReferences?.Count == 0) bom.ExternalReferences = null;
            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;

            if (bom.Components != null)
                foreach (var component in bom.Components)
                    CleanupEmptyArrays(component);
            
            if (bom.Dependencies != null)
                foreach (var dependency in bom.Dependencies)
                    if (dependency.Dependencies?.Count == 0) dependency.Dependencies = null;

            if (bom.Dependencies?.Count == 0) bom.Dependencies = null;
        }
        
        private static void CleanupEmptyArrays(Component component)
        {
            if (component.Hashes?.Count == 0) component.Hashes = null;
            if (component.ExternalReferences?.Count == 0) component.ExternalReferences = null;
            if (component.Components?.Count == 0) component.Components = null;

            if (component.Components != null)
            foreach (var subComponent in component.Components)
                CleanupEmptyArrays(subComponent);
            
            if (component.Pedigree != null) CleanupEmptyArrays(component.Pedigree);
        }

        private static void CleanupEmptyArrays(Pedigree pedigree)
        {
            if (pedigree.Commits?.Count == 0) pedigree.Commits = null;
            if (pedigree.Patches?.Count == 0) pedigree.Patches = null;

            if (pedigree.Ancestors?.Count == 0) pedigree.Ancestors = null;
            if (pedigree.Ancestors != null)
            foreach (var component in pedigree.Ancestors)
                CleanupEmptyArrays(component);

            if (pedigree.Descendants?.Count == 0) pedigree.Descendants = null;
            if (pedigree.Descendants != null)
            foreach (var component in pedigree.Descendants)
                CleanupEmptyArrays(component);

            if (pedigree.Variants?.Count == 0) pedigree.Variants = null;
            if (pedigree.Variants != null)
            foreach (var component in pedigree.Variants)
                CleanupEmptyArrays(component);
        }
    }
}
