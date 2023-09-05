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
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace CycloneDX.Models
{
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    [XmlType("licensing")]
    [ProtoContract]
    public class Licensing
    {
        [ProtoContract]
        public enum LicenseType
        {
            // to make working with protobuf easier
            Null,
            [XmlEnum(Name = "academic")]
            Academic,
            [XmlEnum(Name = "appliance")]
            Appliance,
            [XmlEnum(Name = "client-access")]
            Client_Access,
            [XmlEnum(Name = "concurrent-user")]
            Concurrent_User,
            [XmlEnum(Name = "core-points")]
            Core_Points,
            [XmlEnum(Name = "custom-metric")]
            Custom_Metric,
            [XmlEnum(Name = "device")]
            Device,
            [XmlEnum(Name = "evaluation")]
            Evaluation,
            [XmlEnum(Name = "named-user")]
            Named_User,
            [XmlEnum(Name = "node-locked")]
            Node_Locked,
            [XmlEnum(Name = "oem")]
            Oem,
            [XmlEnum(Name = "perpetual")]
            Perpetual,
            [XmlEnum(Name = "processor-points")]
            Processor_Points,
            [XmlEnum(Name = "subscription")]
            Subscription,
            [XmlEnum(Name = "user")]
            User,
            [XmlEnum(Name = "other")]
            Other,
        }
        
        [XmlArray("altIds")]
        [XmlArrayItem("altId")]
        [ProtoMember(1)]
        public List<string> AltIds { get; set; }

        [XmlElement("licensor")]
        [ProtoMember(2)]
        public OrganizationalEntityOrContact Licensor { get; set; }

        [XmlElement("licensee")]
        [ProtoMember(3)]
        public OrganizationalEntityOrContact Licensee { get; set; }

        [XmlElement("purchaser")]
        [ProtoMember(4)]
        public OrganizationalEntityOrContact Purchaser { get; set; }
        
        [XmlElement("purchaseOrder")]
        [ProtoMember(5)]
        public string PurchaseOrder { get; set; }

        [XmlArray("licenseTypes")]
        [XmlArrayItem("licenseType")]
        [ProtoMember(6)]
        public List<LicenseType> LicenseTypes { get; set; }
        public bool ShouldSerializeLicenseTypes() { return LicenseTypes?.Count > 0; }
        
        private DateTime? _lastRenewal;
        [XmlElement("lastRenewal")]
        [ProtoMember(7)]
        public DateTime? LastRenewal
        { 
            get => _lastRenewal;
            set { _lastRenewal = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeLastRenewal() { return LastRenewal != null; }
        
        private DateTime? _expiration;
        [XmlElement("expiration")]
        [ProtoMember(8)]
        public DateTime? Expiration
        { 
            get => _expiration;
            set { _expiration = BomUtils.UtcifyDateTime(value); }
        }
        public bool ShouldSerializeExpiration() { return Expiration != null; }
    }
}