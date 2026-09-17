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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CycloneDX.Core.Models;
using ProtoBuf;

namespace CycloneDX.Models
{
    [XmlType("dependency")]
    [ProtoContract]
    public class Dependency : IEquatable<Dependency>
#if NET8_0_OR_GREATER
        , IMergeable<Dependency>, IEquivalent<Dependency>
#endif
    {
        [XmlAttribute("ref")]
        [ProtoMember(1)]
        public string Ref { get; set; }

        [XmlElement("dependency")]
        [ProtoMember(2)]
        public List<Dependency> Dependencies { get; set; }

        [XmlElement("provides")]
        public List<Provides> Provides { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ProtoMember(3)]
        public List<string> Provides_Protobuf
        {
            get
            {
                if (Provides == null)
                {
                    return null;
                }
                return Provides.Select((provides) => provides.Ref).ToList();
            }
            set
            {
                if (value == null)
                {
                    Provides = null;
                    return;
                }
                Provides = value.Select((reference) => new Provides { Ref = reference }).ToList();
            }
        }
        public bool ShouldSerializeProvides_Protobuf() { return Provides?.Count > 0; }

        public override bool Equals(object obj)
        {
            var other = obj as Dependency;
            if (other == null)
            {
                return false;
            }

            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(other, Json.Serializer.SerializerOptionsForHash);
        }

        public bool Equals(Dependency obj)
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(obj, Json.Serializer.SerializerOptionsForHash);
        }

        public override int GetHashCode()
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash).GetHashCode();
        }

#if NET8_0_OR_GREATER
        /// <summary>Two Dependency entries describe the same graph node if they share a Ref.</summary>
        public bool Equivalent(Dependency other, MergeStrategy strategy)
        {
            return other != null && !string.IsNullOrEmpty(Ref) && Ref == other.Ref;
        }

        /// <summary>
        /// Without this override, Dependency only merges via the
        /// IMergeable&lt;T&gt; default (exact equality) -- so two BOMs
        /// describing the same component with different direct-dependency
        /// lists (e.g. a module built standalone vs. as part of a larger
        /// build) would each contribute their own `&lt;dependency ref="X"&gt;`
        /// entry, leaving the merged document with two different entries
        /// for the same Ref instead of one combined entry. This unions the
        /// two Dependencies (sub-)lists instead, gated by
        /// <see cref="MergeStrategy.MergeSubsetDependencies"/>: when that's
        /// false, a genuine difference in dependsOn contents is treated as
        /// a conflict (refuse, keep both entries) rather than silently
        /// combined.
        /// </summary>
        public bool MergeWith(Dependency other, MergeStrategy strategy)
        {
            if (other is null)
            {
                return false;
            }
            if (Equals(other))
            {
                return true;
            }
            if (!Equivalent(other, strategy))
            {
                return false;
            }

            if (!strategy.MergeSubsetDependencies && !DependsOnSetsEqual(Dependencies, other.Dependencies))
            {
                return false;
            }

            Dependencies = MergeableListHelper.Merge(Dependencies, other.Dependencies, strategy);
            Provides = MergeableListHelper.MergeSingle(Provides, other.Provides);
            return true;
        }

        private static bool DependsOnSetsEqual(List<Dependency> a, List<Dependency> b)
        {
            var aRefs = a?.Select(d => d.Ref).ToHashSet() ?? new HashSet<string>();
            var bRefs = b?.Select(d => d.Ref).ToHashSet() ?? new HashSet<string>();
            return aRefs.SetEquals(bRefs);
        }
#endif
    }

    [ProtoContract]
    public class Provides
    {
        [XmlAttribute("ref")]
        [ProtoMember(1)]
        public string Ref { get; set; }
    }
}