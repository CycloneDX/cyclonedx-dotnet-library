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

using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models.v1_3
{
    [ProtoContract]
    public class Pedigree
    {
        [XmlArray("ancestors")]
        [XmlArrayItem("component")]
        [ProtoMember(1)]
        public List<Component> Ancestors { get; set; }

        [XmlArray("descendants")]
        [XmlArrayItem("component")]
        [ProtoMember(2)]
        public List<Component> Descendants { get; set; }

        [XmlArray("variants")]
        [XmlArrayItem("component")]
        [ProtoMember(3)]
        public List<Component> Variants { get; set; }

        [XmlArray("commits")]
        [XmlArrayItem("commit")]
        [ProtoMember(4)]
        public List<Commit> Commits { get; set; }

        [XmlArray("patches")]
        [XmlArrayItem("patch")]
        [ProtoMember(5)]
        public List<Patch> Patches { get; set; }

        [XmlElement("notes")]
        [ProtoMember(6)]
        public string Notes { get; set; }

        public Pedigree() {}

        public Pedigree(v1_1.Pedigree pedigree)
        {
            if (pedigree.Ancestors != null)
            {
                Ancestors = new List<Component>();
                foreach (var ancestor in pedigree.Ancestors)
                {
                    Ancestors.Add(new Component(ancestor));
                }
            }
            if (pedigree.Descendants != null)
            {
                Descendants = new List<Component>();
                foreach (var descendant in pedigree.Descendants)
                {
                    Descendants.Add(new Component(descendant));
                }
            }
            if (pedigree.Variants != null)
            {
                Variants = new List<Component>();
                foreach (var variant in pedigree.Variants)
                {
                    Variants.Add(new Component(variant));
                }
            }
            if (pedigree.Commits != null)
            {
                Commits = new List<Commit>();
                foreach (var commit in pedigree.Commits)
                {
                    Commits.Add(new Commit(commit));
                }
            }
            Notes = pedigree.Notes;
        }

        public Pedigree(v1_2.Pedigree pedigree)
        {
            if (pedigree.Ancestors != null)
            {
                Ancestors = new List<Component>();
                foreach (var ancestor in pedigree.Ancestors)
                {
                    Ancestors.Add(new Component(ancestor));
                }
            }
            if (pedigree.Descendants != null)
            {
                Descendants = new List<Component>();
                foreach (var descendant in pedigree.Descendants)
                {
                    Descendants.Add(new Component(descendant));
                }
            }
            if (pedigree.Variants != null)
            {
                Variants = new List<Component>();
                foreach (var variant in pedigree.Variants)
                {
                    Variants.Add(new Component(variant));
                }
            }
            if (pedigree.Commits != null)
            {
                Commits = new List<Commit>();
                foreach (var commit in pedigree.Commits)
                {
                    Commits.Add(new Commit(commit));
                }
            }
            if (pedigree.Patches != null)
            {
                Patches = new List<Patch>();
                foreach (var patch in pedigree.Patches)
                {
                    Patches.Add(new Patch(patch));
                }
            }
            Notes = pedigree.Notes;
        }
    }
}
