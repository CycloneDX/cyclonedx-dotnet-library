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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using CycloneDX.Core.Models;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("component")]
    [ProtoContract]
    public class Component: IEquatable<Component>, IHasBomRef
#if NET8_0_OR_GREATER
        , IMergeable<Component>, IEquivalent<Component>
#endif
    {
        [ProtoContract]
        public enum Classification
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "application")]
            Application,
            [XmlEnum(Name = "framework")]
            Framework,
            [XmlEnum(Name = "library")]
            Library,
            [XmlEnum(Name = "operating-system")]
            Operating_System,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "file")]
            File,
            [XmlEnum(Name = "container")]
            Container,
            [XmlEnum(Name = "firmware")]
            Firmware,
            [XmlEnum(Name = "device-driver")]
            Device_Driver,
            [XmlEnum(Name = "platform")]
            Platform,
            [XmlEnum(Name = "machine-learning-model")]
            Machine_Learning_Model,
            [XmlEnum(Name = "data")]
            Data,
            [XmlEnum(Name = "cryptographic-asset")]
            Cryptographic_Asset,
        }

        [ProtoContract]
        public enum ComponentScope
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "required")]
            Required,
            [XmlEnum(Name = "optional")]
            Optional,
            [XmlEnum(Name = "excluded")]
            Excluded
        }

        [XmlAttribute("type")]
        [ProtoMember(1, IsRequired=true)]
        public Classification Type { get; set; }

        [JsonPropertyName("mime-type")]
        [XmlAttribute("mime-type")]
        [ProtoMember(2)]
        public string MimeType { get; set; }

        [JsonPropertyName("bom-ref")]
        [XmlAttribute("bom-ref")]
        [ProtoMember(3)]
        public string BomRef { get; set; }

        [XmlElement("supplier")]
        [ProtoMember(4)]
        public OrganizationalEntity Supplier { get; set; }

        [XmlElement("manufacturer")]
        [ProtoMember(28)]
        public OrganizationalEntity Manufacturer { get; set; }
        public bool ShouldSerializeManufacturer() { return Manufacturer != null; }

        [XmlArray("authors")]
        [XmlArrayItem("author")]
        [ProtoMember(29)]
        public List<OrganizationalContact> Authors { get; set; }
        public bool ShouldSerializeAuthors() { return Authors?.Count > 0; }

        [Obsolete("This will be removed in a future version. Use @.authors or @.manufacturer instead.")]
        [XmlIgnore]
        [ProtoMember(5)]
        public string Author { get; set; }

        #pragma warning disable 618
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("author")]
        [JsonIgnore]
        public string Author_Xml { get { return Author; } set { Author = value; } }
        public bool ShouldSerializeAuthor_Xml() { return Author != null; }
        #pragma warning restore 618


        [XmlElement("publisher")]
        [ProtoMember(6)]
        public string Publisher { get; set; }

        [XmlElement("group")]
        [ProtoMember(7)]
        public string Group { get; set; }

        [XmlElement("name")]
        [ProtoMember(8)]
        public string Name { get; set; }

        [XmlElement("version")]
        [ProtoMember(9)]
        public string Version { get; set; }

        [XmlElement("versionRange")]
        [ProtoMember(33)]
        public string VersionRange { get; set; }

        [XmlElement("description")]
        [ProtoMember(10)]
        public string Description { get; set; }

        [XmlIgnore]
        [ProtoMember(11)]
        public ComponentScope? Scope { get; set; }
        [XmlElement("scope")]
        [JsonIgnore]
        public ComponentScope NonNullableScope
        {
            get
            {
                return Scope.Value;
            }
            set
            {
                Scope = value;
            }
        }
        public bool ShouldSerializeNonNullableScope() { return Scope.HasValue; }

        [XmlArray("hashes")]
        [ProtoMember(12)]
        public List<Hash> Hashes { get; set; }
        public bool ShouldSerializeHashes() { return Hashes?.Count > 0; }

        [XmlIgnore]
        [ProtoMember(13)]
        public List<LicenseChoice> Licenses { get; set; }


        [XmlElement("licenses")]
        [JsonIgnore, ProtoIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        // This is a serialization workaround
        public LicenseChoiceList LicensesSerialized
        {
            get { return Licenses != null ? new LicenseChoiceList(Licenses) : null; }
            set { Licenses = value.Licenses; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLicensesSerialized() { return Licenses?.Count > 0; }

        [XmlElement("copyright")]
        [ProtoMember(14)]
        public string Copyright { get; set; }

        [XmlArray("patentAssertions")]
        [XmlArrayItem("patentAssertion")]
        [ProtoMember(35)]
        public List<PatentAssertion> PatentAssertions { get; set; }
        public bool ShouldSerializePatentAssertions() { return PatentAssertions?.Count > 0; }

        [XmlElement("cpe")]
        [ProtoMember(15)]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        [ProtoMember(16)]
        public string Purl { get; set; }

        [XmlElement("omniborId")]
        [ProtoMember(31)]
        public List<string> OmniborId { get; set; }
        public bool ShouldSerializeOmniborId() { return OmniborId?.Count > 0; }

        [XmlElement("swhid")]
        [ProtoMember(32)]
        public List<string> Swhid { get; set; }
        public bool ShouldSerializeSwhid() { return Swhid?.Count > 0; }

        [XmlElement("swid")]
        [ProtoMember(17)]
        public Swid Swid { get; set; }

        // XML serialization doesn't like nullable value types
        [XmlIgnore]
        [ProtoMember(18)]
        public bool? Modified { get; set; }
        [XmlElement("modified")]
        [JsonIgnore]
        public bool NonNullableModified
        {
            get
            {
                return Modified.HasValue && Modified.Value;
            }
            set
            {
                Modified = value;
            }
        }
        public bool ShouldSerializeNonNullableModified() { return Modified.HasValue; }

        [XmlElement("pedigree")]
        [ProtoMember(19)]
        public Pedigree Pedigree { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("reference")]
        [ProtoMember(20)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() { return ExternalReferences?.Count > 0; }

        //In the xml format, Properties is in front of Components.
        //XML serialization uses the member order unless explicitly specified differently.
        public bool ShouldSerializeComponents() { return Components?.Count > 0; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(22)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }

        [XmlArray("components")]
        [ProtoMember(21)]
        public List<Component> Components { get; set; }
        
        [XmlElement("evidence")]
        [ProtoMember(23)]
        public Evidence Evidence { get; set; }

        [XmlElement("releaseNotes")]
        [ProtoMember(24)]
        public ReleaseNotes ReleaseNotes { get; set; }
        public bool ShouldSerializeReleaseNotes() { return ReleaseNotes != null; }
        
        [XmlElement("modelCard")]
        [ProtoMember(25)]
        public ModelCard ModelCard { get; set; }

        [XmlElement("data")]
        [ProtoMember(26)]
        public List<Data> Data { get; set; }
        public bool ShouldSerializeData() { return Data?.Count > 0; }

        [XmlElement("cryptoProperties")]
        [ProtoMember(27)]
        public CryptoProperties CryptoProperties { get; set; }

        [XmlIgnore]
        [ProtoMember(34)]
        public bool? IsExternal { get; set; }
        [XmlAttribute("isExternal")]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool NonNullableIsExternal
        {
            get => IsExternal.HasValue && IsExternal.Value;
            set => IsExternal = value;
        }
        public bool ShouldSerializeNonNullableIsExternal() { return IsExternal.HasValue; }

        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        [ProtoMember(30)]
        public List<string> Tags { get; set; }
        public bool ShouldSerializeTags() { return Tags?.Count > 0; }

        [XmlAnyElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        [JsonIgnore]
        public XmlElement XmlSignature { get; set; }
        [XmlIgnore]
        public SignatureChoice Signature { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Component;
            if (other == null)
            {
                return false;
            }

            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(other, Json.Serializer.SerializerOptionsForHash);
        }

        public bool Equals(Component obj)
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash) == JsonSerializer.Serialize(obj, Json.Serializer.SerializerOptionsForHash);
        }
    
        public override int GetHashCode()
        {
            return JsonSerializer.Serialize(this, Json.Serializer.SerializerOptionsForHash).GetHashCode();
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Cheap pre-check for "plausibly the same real-world component,
        /// worth attempting MergeWith on" -- not full equality. By spec,
        /// "type" and "name" are the two required identifying properties;
        /// "version"/"group"/"purl" are treated as equal-if-both-present
        /// (so two components with the identical type/name but only one
        /// side specifying a version are still considered equivalent,
        /// rather than assumed to be different versions of the same
        /// thing). bom-ref is deliberately NOT part of this check -- two
        /// components can describe the same real-world thing under
        /// different bom-ref values in different source documents, and
        /// reconciling bom-ref identity is the merge orchestration's job
        /// (see BomRefWalker), not this per-field check's.
        /// </summary>
        public bool Equivalent(Component other, MergeStrategy strategy)
        {
            if (other is null)
            {
                return false;
            }

            return Type == other.Type
                && !(Name is null) && !(other.Name is null) && Name == other.Name
                && (Version is null || other.Version is null || Version == other.Version)
                && (Group is null || other.Group is null || Group == other.Group)
                && (Purl is null || other.Purl is null || Purl == other.Purl);
        }

        /// <summary>
        /// Attempt to fold <paramref name="other"/>'s data into this
        /// component. Scalar fields prefer this instance's own value and
        /// fall back to <paramref name="other"/>'s only when unset; list
        /// fields are merged via <see cref="MergeableListHelper"/>; Scope
        /// is reconciled via <see cref="MergeScope"/>, which is the one
        /// place this can still refuse to merge (an Excluded/Required
        /// clash is a genuine conflict, not something to silently paper
        /// over) -- see <see cref="MergeStrategy.ComponentConflictResolution"/>.
        /// </summary>
        public bool MergeWith(Component other, MergeStrategy strategy)
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

            if (strategy.ComponentConflictResolution == ComponentConflictResolution.KeepSeparate)
            {
                return false;
            }

            if (!TryMergeScope(Scope, other.Scope, strategy.ComponentConflictResolution, out var mergedScope))
            {
                // Scope reconciliation refused (e.g. Excluded vs. Required) --
                // these are different enough real-world things to keep separate.
                return false;
            }

            Scope = mergedScope;
            MimeType ??= other.MimeType;
            Supplier = MergeableListHelper.MergeSingle(Supplier, other.Supplier);
            Manufacturer = MergeableListHelper.MergeSingle(Manufacturer, other.Manufacturer);
            Authors = MergeableListHelper.Merge(Authors, other.Authors, strategy);
#pragma warning disable 618
            Author ??= other.Author;
#pragma warning restore 618
            Publisher ??= other.Publisher;
            VersionRange ??= other.VersionRange;
            Description ??= other.Description;
            Hashes = MergeableListHelper.Merge(Hashes, other.Hashes, strategy);
            Licenses = MergeableListHelper.Merge(Licenses, other.Licenses, strategy);
            Copyright ??= other.Copyright;
            PatentAssertions = MergeableListHelper.Merge(PatentAssertions, other.PatentAssertions, strategy);
            Cpe ??= other.Cpe;
            Purl ??= other.Purl;
            OmniborId = MergeableListHelper.MergeStringList(OmniborId, other.OmniborId);
            Swhid = MergeableListHelper.MergeStringList(Swhid, other.Swhid);
            Swid = MergeableListHelper.MergeSingle(Swid, other.Swid);
            Modified = MergeableListHelper.MergeNullableBoolOr(Modified, other.Modified);
            Pedigree = MergeableListHelper.MergeSingle(Pedigree, other.Pedigree);
            ExternalReferences = MergeableListHelper.Merge(ExternalReferences, other.ExternalReferences, strategy);
            Properties = MergeableListHelper.Merge(Properties, other.Properties, strategy);
            Components = MergeableListHelper.Merge(Components, other.Components, strategy);
            Evidence = MergeableListHelper.MergeSingle(Evidence, other.Evidence);
            ReleaseNotes = MergeableListHelper.MergeSingle(ReleaseNotes, other.ReleaseNotes);
            ModelCard = MergeableListHelper.MergeSingle(ModelCard, other.ModelCard);
            Data = MergeableListHelper.MergeSingle(Data, other.Data);
            CryptoProperties = MergeableListHelper.MergeSingle(CryptoProperties, other.CryptoProperties);
            IsExternal = MergeableListHelper.MergeNullableBoolOr(IsExternal, other.IsExternal);
            Tags = MergeableListHelper.MergeStringList(Tags, other.Tags);
            XmlSignature = MergeableListHelper.MergeSingle(XmlSignature, other.XmlSignature);
            Signature = MergeableListHelper.MergeSingle(Signature, other.Signature);

            return true;
        }

        /// <summary>
        /// Reconcile two Scope values for components that are otherwise
        /// being merged into one (see MergeStrategy.ComponentConflictResolution's
        /// XML docs for the domain rules). "Both sides already equal" is
        /// handled uniformly first, including the both-Excluded case: two
        /// components that are both Excluded-scope but differ in some
        /// unrelated field should still merge, not be treated as a scope
        /// conflict just because neither side is Optional.
        /// </summary>
        /// <returns>
        /// <c>false</c> if the two scopes genuinely conflict (Excluded vs.
        /// Required/unset) and the caller should not merge these two
        /// components at all.
        /// </returns>
        public static bool TryMergeScope(ComponentScope? a, ComponentScope? b, ComponentConflictResolution resolution, out ComponentScope? merged)
        {
            if (a == b)
            {
                merged = a;
                return true;
            }

            if (resolution == ComponentConflictResolution.Squash_RenameByScope)
            {
                // Scope partitioning under this strategy is handled by a
                // dedicated pre-pass (CycloneDXUtils' RenameByScope pass)
                // that splits differently-scoped components into separate,
                // suffixed bom-refs *before* the generic per-field merge
                // ever runs -- so by the time two Components reach here with
                // different Scope values, they should not be merged at all;
                // treat it as a refusal rather than silently squashing.
                merged = null;
                return false;
            }

            bool aExcluded = a == ComponentScope.Excluded;
            bool bExcluded = b == ComponentScope.Excluded;

            if (!aExcluded && !bExcluded)
            {
                // Neither side excludes the component. Per the spec, an absent
                // (null/unset) Scope SHOULD be treated as required -- so unless
                // both sides agree on "optional", the safe reading is whichever
                // is more inclusive. Squash_UpgradeScope (the default) always
                // resolves to Required; Squash_DowngradeScope keeps the
                // narrower "optional" reading when either side actually said so.
                merged = resolution == ComponentConflictResolution.Squash_UpgradeScope
                    ? ComponentScope.Required
                    : (a == ComponentScope.Optional || b == ComponentScope.Optional)
                        ? ComponentScope.Optional
                        : (ComponentScope?)null;
                return true;
            }

            // Exactly one side is Excluded (a == b above already handled both-Excluded).
            var other = aExcluded ? b : a;
            if (other == ComponentScope.Optional || other is null)
            {
                // Excluded dominates over a merely-optional/unspecified reading.
                merged = ComponentScope.Excluded;
                return true;
            }

            // Excluded vs. Required is a genuine conflict: these describe
            // different real-world usages of the same component and should
            // not be silently squashed into one.
            merged = null;
            return false;
        }
#endif
    }
}