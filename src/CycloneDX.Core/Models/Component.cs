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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

using CycloneDX.Utils.ListMergeHelper;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("component")]
    [ProtoContract]
    public class Component: IEquatable<Component>
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
            OperationSystem,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "file")]
            File,
            [XmlEnum(Name = "container")]
            Container,
            [XmlEnum(Name = "firmware")]
            Firmware
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

        [XmlElement("author")]
        [ProtoMember(5)]
        public string Author { get; set; }

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

        [XmlElement("licenses")]
        [ProtoMember(13)]
        public List<LicenseChoice> Licenses { get; set; }

        [XmlElement("copyright")]
        [ProtoMember(14)]
        public string Copyright { get; set; }

        [XmlElement("cpe")]
        [ProtoMember(15)]
        public string Cpe { get; set; }

        [XmlElement("purl")]
        [ProtoMember(16)]
        public string Purl { get; set; }

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

        [XmlArray("components")]
        [ProtoMember(21)]
        public List<Component> Components { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(22)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() { return Properties?.Count > 0; }
        
        [XmlElement("evidence")]
        [ProtoMember(23)]
        public Evidence Evidence { get; set; }

        [XmlElement("releaseNotes")]
        [ProtoMember(24)]
        public ReleaseNotes ReleaseNotes { get; set; }
        public bool ShouldSerializeReleaseNotes() { return ReleaseNotes != null; }

        public bool Equals(Component obj)
        {
            return CycloneDX.Json.Serializer.Serialize(this) == CycloneDX.Json.Serializer.Serialize(obj);
        }

        public override int GetHashCode()
        {
            return CycloneDX.Json.Serializer.Serialize(this).GetHashCode();
        }

        public bool mergeWith(Component obj)
        {
            if (this.Equals(obj))
                // Contents are identical, nothing to do:
                return true;

            if (
                (this.BomRef != null && BomRef.Equals(obj.BomRef)) ||
                (this.Group == obj.Group && this.Name == obj.Name && this.Version == obj.Version)
            ) {
                // Objects seem equivalent according to critical arguments;
                // merge the attribute values with help of reflection:
                PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Instance);

                // Use a temporary clone instead of mangling "this" object right away:
                Component tmp = new Component();
                tmp = JsonSerializer.Deserialize<Component>(CycloneDX.Json.Serializer.Serialize(this));
                bool mergedOk = true;

                foreach (PropertyInfo property in properties)
                {
                    switch (property.PropertyType)
                    {
                        case Type _ when property.PropertyType == typeof(ComponentScope):
                            {
                                // Not nullable!
                                ComponentScope tmpItem = (ComponentScope)property.GetValue(tmp, null);
                                ComponentScope objItem = (ComponentScope)property.GetValue(obj, null);
                                if (tmpItem == objItem)
                                {
                                    continue;
                                }
                                else
                                {
                                    // Per CycloneDX spec v1.4, absent value "SHOULD" be treated as "required"
                                    if (tmpItem == ComponentScope.Required || tmpItem == ComponentScope.Null)
                                    {
                                        if (objItem != ComponentScope.Excluded)
                                            // keep absent==required; upgrade optional objItem to value of tmp
                                            property.SetValue(tmp, ComponentScope.Required);
                                            continue;
                                    }

                                    if (objItem == ComponentScope.Required || objItem == ComponentScope.Null)
                                    {
                                        if (tmpItem != ComponentScope.Excluded)
                                            // set required; upgrade optional tmpItem (if such)
                                            property.SetValue(tmp, ComponentScope.Required);
                                            continue;
                                    }
                                }

                                // Here throw some exception or trigger creation of new object with a
                                // new bom-ref - and a new identification in the original document to
                                // avoid conflicts; be sure then to check for other entries that have
                                // everything same except bom-ref (match the expected new pattern)?..
                                mergedOk = false;
                            }
                            break;

                        case Type _ when property.PropertyType == typeof(List<object>):
                            {
                                var listTmp = ((List<object>)(property.GetValue(tmp, null)));
                                var listObj = ((List<object>)(property.GetValue(obj, null)));
/*
                                if (listObj == null || listObj.Count == 0)
                                {
                                    // Keep whatever "this" version of the list as the only one relevant
                                    break;
                                }

                                if (listTmp == null || listTmp.Count == 0)
                                {
                                    // Keep whatever "other" version of the list as the only one relevant
                                    property.SetValue(tmp, listObj);
                                    break;
                                }
*/
                                var propertyMerger = new ListMergeHelper<object>();
                                property.SetValue(tmp, propertyMerger.Merge(listTmp, listObj));
/*
                                foreach (var objItem in ((List<object>)(property.GetValue(obj, null))))
                                {
                                    if (objItem is null)
                                        continue;

                                    bool listHit = false;
                                    foreach (var tmpItem in ((List<object>)(property.GetValue(tmp, null))))
                                    {
                                        if (tmpItem != null && tmpItem == objItem)
                                        {
                                            listHit = true;
                                            var method = property.GetValue(tmp, null).GetType().GetMethod("mergeWith");
                                            if (method != null)
                                            {
                                                try
                                                {
                                                    if (!((bool)method.Invoke(property.GetValue(tmp, null), new object[] {property.GetValue(obj, null)})))
                                                        mergedOk = false;
                                                }
                                                catch (System.Exception exc)
                                                {
                                                    Console.WriteLine($"SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmpItem.ToString()} and {objItem.ToString()}: {exc.ToString()}");
                                                    mergedOk = false;
                                                }
                                            } // else: no method, just trust equality - avoid "Add" to merge below
                                            else
                                            {
                                                Console.WriteLine($"SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmpItem.ToString()} and {objItem.ToString()}: no such method");
                                            }
                                        }
                                    }

                                    if (!listHit)
                                    {
                                        (((List<object>)property.GetValue(tmp, null))).Add(objItem);
                                    }
                                }
*/
                            }
                            break;

                        default:
                            {
                                var method = property.GetValue(tmp, null).GetType().GetMethod("mergeWith");
                                if (method != null)
                                {
                                    try
                                    {
                                        if (!((bool)method.Invoke(property.GetValue(tmp, null), new object[] {property.GetValue(obj, null)})))
                                            mergedOk = false;
                                    }
                                    catch (System.Exception exc)
                                    {
                                        // That property's class lacks a mergeWith(), gotta trust the equality:
                                        if (property.GetValue(tmp, null) == property.GetValue(obj, null))
                                            continue;
                                        Console.WriteLine($"FAILED MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmp.ToString()} and {obj.ToString()}: {exc.ToString()}");
                                        mergedOk = false;
                                    }
                                }
                                else
                                {
                                    // That property's class lacks a mergeWith(), gotta trust the equality:
                                    Console.WriteLine($"SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmp.ToString()} and {obj.ToString()}: no such method");
                                    if (property.GetValue(tmp, null) == property.GetValue(obj, null))
                                        continue;
                                    mergedOk = false;
                                }
                            }
                            break;
                    }
                }

                if (mergedOk) {
                    // No failures, only now update the current object:
                    foreach (PropertyInfo property in properties)
                    {
                        property.SetValue(this, property.GetValue(tmp, null));
                    }
                }

                return mergedOk;
            }

            // Merge was not applicable or otherwise did not succeed
            return false;
        }
    }
}