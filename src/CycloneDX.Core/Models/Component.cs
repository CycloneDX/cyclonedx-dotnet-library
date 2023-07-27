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
            if (!int.TryParse(System.Environment.GetEnvironmentVariable("CYCLONEDX_DEBUG_MERGE"), out int iDebugLevel) || iDebugLevel < 0)
                iDebugLevel = 0;

            if (this.Equals(obj))
            {
                if (iDebugLevel >= 1)
                    Console.WriteLine($"Component.mergeWith(): SKIP: contents are identical, nothing to do");
                return true;
            }

            if (
                (this.BomRef != null && BomRef.Equals(obj.BomRef)) ||
                (this.Group == obj.Group && this.Name == obj.Name && this.Version == obj.Version)
            ) {
                // Objects seem equivalent according to critical arguments;
                // merge the attribute values with help of reflection:
                if (iDebugLevel >= 1)
                    Console.WriteLine($"Component.mergeWith(): items seem related - investigate properties: {this.BomRef} / {this.Group} : {this.Name} : {this.Version}");
                PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance); // BindingFlags.DeclaredOnly
                if (iDebugLevel >= 1)
                    Console.WriteLine($"Component.mergeWith(): items seem related - investigate properties: num {properties.Length}: {properties.ToString()}");

                // Use a temporary clone instead of mangling "this" object right away;
                // note serialization seems to skip over "nonnullable" values in some cases
                Component tmp = new Component();
                foreach (PropertyInfo property in properties)
                {
                    try {
                        property.SetValue(tmp, property.GetValue(this, null));
                    } catch (System.Exception) {
                        // no-op
                    }
                }
                /* tmp = JsonSerializer.Deserialize<Component>(CycloneDX.Json.Serializer.Serialize(this)); */
                bool mergedOk = true;

                foreach (PropertyInfo property in properties)
                {
                    if (iDebugLevel >= 2)
                        Console.WriteLine($"Component.mergeWith(): <{property.PropertyType}>'{property.Name}'");
                    switch (property.PropertyType)
                    {
                        case Type _ when property.PropertyType == typeof(Nullable):
                            break;
/*
                        case Type _ when property.PropertyType == typeof(Nullable[]):
                            break;
*/
                        case Type _ when property.PropertyType == typeof(ComponentScope):
                            {
                                // Not nullable!
                                ComponentScope tmpItem;
                                try
                                {
                                    tmpItem = (ComponentScope)property.GetValue(tmp, null);
                                }
                                catch (System.Exception)
                                {
                                    // Unspecified => required per CycloneDX spec v1.4?..
                                    tmpItem = ComponentScope.Null;
                                }

                                ComponentScope objItem;
                                try
                                {
                                    objItem = (ComponentScope)property.GetValue(obj, null);
                                }
                                catch (System.Exception)
                                {
                                    objItem = ComponentScope.Null;
                                }

                                if (iDebugLevel >= 3)
                                    Console.WriteLine($"Component.mergeWith(): SCOPE: '{tmpItem}' and '{objItem}'");

                                // Per CycloneDX spec v1.4, absent value "SHOULD" be treated as "required"
                                if (tmpItem != ComponentScope.Excluded && objItem != ComponentScope.Excluded)
                                {
                                    // keep absent==required; upgrade optional objItem
                                    property.SetValue(tmp, ComponentScope.Required);
                                    if (iDebugLevel >= 3)
                                        Console.WriteLine($"Component.mergeWith(): SCOPE: set 'Required'");
                                    continue;
                                }

                                if ((tmpItem == ComponentScope.Excluded && objItem == ComponentScope.Optional) || (objItem == ComponentScope.Excluded && tmpItem == ComponentScope.Optional))
                                {
                                    // downgrade optional objItem to excluded
                                    property.SetValue(tmp, ComponentScope.Excluded);
                                    if (iDebugLevel >= 3)
                                        Console.WriteLine($"Component.mergeWith(): SCOPE: set 'Excluded'");
                                    continue;
                                }


/*
                                if (tmpItem == ComponentScope.Required || tmpItem == ComponentScope.Null)
                                {
                                    if (objItem != ComponentScope.Excluded)
                                    {
                                        // keep absent==required; upgrade optional objItem to value of tmp
                                        property.SetValue(tmp, ComponentScope.Required);
                                        continue;
                                    }
                                }

                                if (objItem == ComponentScope.Required || objItem == ComponentScope.Null)
                                {
                                    if (tmpItem != ComponentScope.Excluded)
                                    {
                                        // set required; upgrade optional tmpItem (if such)
                                        property.SetValue(tmp, ComponentScope.Required);
                                        continue;
                                    }
                                }
*/

                                // Here throw some exception or trigger creation of new object with a
                                // new bom-ref - and a new identification in the original document to
                                // avoid conflicts; be sure then to check for other entries that have
                                // everything same except bom-ref (match the expected new pattern)?..
                                if (iDebugLevel >= 1)
                                    Console.WriteLine($"Component.mergeWith(): WARNING: can not merge two bom-refs with scope excluded and required");
                                mergedOk = false;
                            }
                            break;

                        case Type _ when (property.Name == "NonNullableModified"):
                            {
                                // Not nullable!
                                bool tmpItem = (bool)property.GetValue(tmp, null);
                                bool objItem = (bool)property.GetValue(obj, null);

                                if (iDebugLevel >= 3)
                                    Console.WriteLine($"Component.mergeWith(): MODIFIED BOOL: '{tmpItem}' and '{objItem}'");
                                if (objItem)
                                    property.SetValue(tmp, true);
                            }
                            break;

                        case Type _ when (property.PropertyType == typeof(List<Object>) || property.PropertyType.ToString().StartsWith("System.Collections.Generic.List")):
                            {
                                // https://www.experts-exchange.com/questions/22600200/Traverse-generic-List-using-C-Reflection.html
                                var propValTmp = property.GetValue(tmp);
                                var propValObj = property.GetValue(obj);
                                if (propValTmp == null && propValObj == null)
                                {
                                    if (iDebugLevel >= 3)
                                        Console.WriteLine($"Component.mergeWith(): LIST?: got <null> in tmp and <null> in obj");
                                    continue;
                                }

                                var LType = (propValTmp == null ? propValObj.GetType() : propValTmp.GetType());
                                var propCount = LType.GetProperty("Count");
                                var methodGetItem = LType.GetMethod("get_Item");
                                var methodAdd = LType.GetMethod("Add");
                                if (methodGetItem == null || propCount == null || methodAdd == null)
                                {
                                    if (iDebugLevel >= 1)
                                        Console.WriteLine($"Component.mergeWith(): WARNING: is this really a LIST - it lacks a get_Item() or Add() method, or a Count property");
                                    mergedOk = false;
                                    continue;
                                }

                                int propValTmpCount = (propValTmp == null ? -1 : (int)propCount.GetValue(propValTmp, null));
                                int propValObjCount = (propValObj == null ? -1 : (int)propCount.GetValue(propValObj, null));
                                if (iDebugLevel >= 4)
                                    Console.WriteLine($"Component.mergeWith(): LIST?: got {propValTmp}=>{propValTmpCount} in tmp and {propValObj}=>{propValObjCount} in obj");

                                if (propValObj == null || propValObjCount == 0 || propValObjCount == null)
                                {
                                    continue;
                                }

                                if (propValTmp == null || propValTmpCount == 0 || propValTmpCount == null)
                                {
                                    property.SetValue(tmp, propValObj);
                                    continue;
                                }

                                var TType = methodGetItem.Invoke(propValObj, new object[] { 0 }).GetType();
                                var methodMergeWith = TType.GetMethod("mergeWith");

                                for (int o = 0; o < propValObjCount; o++)
                                {
                                    var objItem = methodGetItem.Invoke(propValObj, new object[] { o });
                                    if (objItem is null)
                                        continue;

                                    bool listHit = false;
                                    for (int t = 0; t < propValTmpCount; t++)
                                    {
                                        var tmpItem = methodGetItem.Invoke(propValTmp, new object[] { t });
                                        if (tmpItem != null)
                                        {
                                            listHit = true;
                                            if (methodMergeWith != null)
                                            {
                                                try
                                                {
                                                    if (iDebugLevel >= 4)
                                                        Console.WriteLine($"Component.mergeWith(): Call futher {TType.ToString()}.mergeWith() for '{property.Name}': merge of {tmpItem?.ToString()} and {objItem?.ToString()}");
                                                    if (!((bool)methodMergeWith.Invoke(tmpItem, new object[] {objItem})))
                                                        mergedOk = false;
                                                }
                                                catch (System.Exception exc)
                                                {
                                                    if (iDebugLevel >= 4)
                                                        Console.WriteLine($"Component.mergeWith(): SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmpItem?.ToString()} and {objItem?.ToString()}: {exc.ToString()}");
                                                    mergedOk = false;
                                                }
                                            } // else: no method, just trust equality - avoid "Add" to merge below
                                            else
                                            {
                                                if (iDebugLevel >= 6)
                                                    Console.WriteLine($"Component.mergeWith(): SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmpItem?.ToString()} and {objItem?.ToString()}: no such method: will add to list");
                                            }
                                        } // else: tmpitem considered not equal, should be added
                                    }

                                    if (!listHit)
                                    {
                                        methodAdd.Invoke(propValTmp, new object[] {objItem});
                                        propValTmpCount = (int)propCount.GetValue(propValTmp, null);
                                    }
                                }
                            }
                            break;

                        default:
                            {
                                if (
                                    /* property.CustomAttributes.Any(x => x.AttributeType.Name == "NullableAttribute") || */
                                    property.PropertyType.ToString().StartsWith("System.Nullable")
                                )
                                {
                                    // e.g. <System.Nullable`1[CycloneDX.Models.Component+ComponentScope]>'Scope' helper
                                    // followed by <System.Nullable`1[CycloneDX.Models.Component+ComponentScope]>'Scope'
                                    // which we specially handle above
                                    if (iDebugLevel >= 4)
                                        Console.WriteLine($"Component.mergeWith(): SKIP NullableAttribute");
                                    continue;
                                }

                                if (iDebugLevel >= 3)
                                    Console.WriteLine($"Component.mergeWith(): DEFAULT TYPES");
                                var propValTmp = property.GetValue(tmp, null);
                                var propValObj = property.GetValue(obj, null);
                                if (propValObj == null)
                                {
                                    continue;
                                }

                                if (propValTmp == null)
                                {
                                    property.SetValue(tmp, propValObj);
                                    continue;
                                }

                                var TType = propValTmp.GetType();
                                var methodEquals = TType.GetMethod("Equals", 0, new Type[] { TType });
                                bool propsSeemEqual = false;
                                bool propsSeemEqualLearned = false;

                                try
                                {
                                    if (methodEquals != null)
                                    {
                                        if (iDebugLevel >= 5)
                                            Console.WriteLine($"Component.mergeWith(): try methodEquals()");
                                        propsSeemEqual = (bool)methodEquals.Invoke(propValTmp, new object[] {propValObj});
                                        propsSeemEqualLearned = true;
                                    }
                                }
                                catch (System.Exception exc)
                                {
                                    // no-op
                                    if (iDebugLevel >= 5)
                                        Console.WriteLine($"Component.mergeWith(): can not check Equals() {propValTmp.ToString()} and {propValObj.ToString()}: {exc.ToString()}");
                                }

                                try
                                {
                                    if (!propsSeemEqualLearned)
                                    {
                                        // Fall back to generic equality check which may be useless
                                        if (iDebugLevel >= 4)
                                            Console.WriteLine($"Component.mergeWith(): MIGHT SKIP MERGE: items say they are equal");
                                        propsSeemEqual = propValTmp.Equals(propValObj);
                                        propsSeemEqualLearned = true;
                                    }
                                }
                                catch (System.Exception exc)
                                {
                                    // no-op
                                }

                                try
                                {
                                    if (!propsSeemEqualLearned)
                                    {
                                        // Fall back to generic equality check which may be useless
                                        if (iDebugLevel >= 4)
                                            Console.WriteLine($"Component.mergeWith(): SKIP MERGE: items say they are equal");
                                        propsSeemEqual = (propValTmp == propValObj);
                                        propsSeemEqualLearned = true;
                                    }
                                }
                                catch (System.Exception exc)
                                {
                                    // no-op
                                }

                                if (!propsSeemEqual)
                                {
                                    if (iDebugLevel >= 4)
                                        Console.WriteLine($"Component.mergeWith(): items say they are not equal");
                                }

                                var methodMergeWith = TType.GetMethod("mergeWith");
                                if (methodMergeWith != null)
                                {
                                    try
                                    {
                                        if (!((bool)methodMergeWith.Invoke(propValTmp, new object[] {propValObj})))
                                            mergedOk = false;
                                    }
                                    catch (System.Exception exc)
                                    {
                                        // That property's class lacks a mergeWith(), gotta trust the equality:
                                        if (propsSeemEqual)
                                            continue;
                                        if (iDebugLevel >= 4)
                                            Console.WriteLine($"Component.mergeWith(): FAILED MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmp.ToString()} and {obj.ToString()}: {exc.ToString()}");
                                        mergedOk = false;
                                    }
                                }
                                else
                                {
                                    // That property's class lacks a mergeWith(), gotta trust the equality:
                                    if (propsSeemEqual)
                                        continue;
                                    if (iDebugLevel >= 6)
                                        Console.WriteLine($"Component.mergeWith(): SKIP MERGE: can not {this.GetType().ToString()}.mergeWith() '{property.Name}' of {tmp.ToString()} and {obj.ToString()}: no such method");
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

                if (iDebugLevel >= 1)
                    Console.WriteLine($"Component.mergeWith(): result {mergedOk} for: {this.BomRef} / {this.Group} : {this.Name} : {this.Version}");
                return mergedOk;
            }
            else
            {
                if (iDebugLevel >= 1)
                    Console.WriteLine($"Component.mergeWith(): SKIP: items do not seem related");
            }

            // Merge was not applicable or otherwise did not succeed
            return false;
        }
    }
}