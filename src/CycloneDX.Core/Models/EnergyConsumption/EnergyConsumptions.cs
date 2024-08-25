using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CycloneDX.Models
{
    [ProtoContract]
    public class EnergyConsumption
    {
        [XmlElement("activity")]
        [ProtoMember(1)]
        public ActivityType Activity { get; set; }

        [XmlElement("energyProviders")]
        [ProtoMember(2)]
        public List<EnergyProvider> EnergyProviders { get; set; }

        [XmlElement("activityEnergyCost")]
        [ProtoMember(3)]
        public EnergyMeasure ActivityEnergyCost { get; set; }

        [XmlElement("co2CostEquivalent")]
        [ProtoMember(4)]
        public Co2Measure Co2CostEquivalent { get; set; }

        [XmlElement("co2CostOffset")]
        [ProtoMember(5)]
        public Co2Measure Co2CostOffset { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        [ProtoMember(6)]
        public List<Property> Properties { get; set; }
        public bool ShouldSerializeProperties() => Properties?.Count > 0;
    }
    [ProtoContract]
    public enum ActivityType
    {
        Null,
        [XmlEnum("design")]
        Design,

        [XmlEnum("data-collection")]
        Data_Collection,

        [XmlEnum("data-preparation")]
        Data_Preparation,

        [XmlEnum("training")]
        Training,

        [XmlEnum("fine-tuning")]
        Fine_Tuning,

        [XmlEnum("validation")]
        Validation,

        [XmlEnum("deployment")]
        Deployment,

        [XmlEnum("inference")]
        Inference,

        [XmlEnum("other")]
        Other
    }
    [ProtoContract]
    public enum EnergySource
    {
        Null,
        [XmlEnum("coal")]
        Coal,

        [XmlEnum("oil")]
        Oil,

        [XmlEnum("natural-gas")]
        Natural_Gas,

        [XmlEnum("nuclear")]
        Nuclear,

        [XmlEnum("wind")]
        Wind,

        [XmlEnum("solar")]
        Solar,

        [XmlEnum("geothermal")]
        Geothermal,

        [XmlEnum("hydropower")]
        Hydropower,

        [XmlEnum("biofuel")]
        Biofuel,

        [XmlEnum("unknown")]
        Unknown,

        [XmlEnum("other")]
        Other
    }
    [ProtoContract]
    public enum CO2Unit
    { 
        Null,
        [XmlEnum("tCO2eq")]
        tCO2eq
    }
    [ProtoContract]
    public enum EnergyUnit
    { 
        Null,
        [XmlEnum("kWh")]
        kWh
    }

    [ProtoContract]
    public class EnergyMeasure
    {
        [XmlElement("value")]
        [ProtoMember(1)]
        public decimal Value { get; set; }

        [XmlElement("unit")]
        [ProtoMember(2)]
        public EnergyUnit Unit { get; set; }
    }
    [ProtoContract]
    public class Co2Measure
    {
        [XmlElement("value")]
        [ProtoMember(1)]
        public decimal Value { get; set; }

        [XmlElement("unit")]
        [ProtoMember(2)]
        public CO2Unit Unit { get; set; }
    }
    [ProtoContract]
    public class EnergyProvider
    {
        [XmlElement("description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        [XmlElement("organization")]
        [ProtoMember(3)]
        public OrganizationalEntity Organization { get; set; }

        [XmlElement("energySource")]
        [ProtoMember(4)]
        public EnergySource EnergySource { get; set; }

        [XmlElement("energyProvided")]
        [ProtoMember(5)]
        public EnergyMeasure EnergyProvided { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("externalReference")]
        [ProtoMember(6)]
        public List<ExternalReference> ExternalReferences { get; set; }
        public bool ShouldSerializeExternalReferences() => ExternalReferences?.Count > 0;

        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }
    }
    [ProtoContract]
    public class PostalAddress
    {
        [XmlAttribute("bom-ref")]
        [ProtoMember(1)]
        public string BomRef { get; set; }

        [XmlElement("country")]
        [ProtoMember(2)]
        public string Country { get; set; }

        [XmlElement("region")]
        [ProtoMember(3)]
        public string Region { get; set; }

        [XmlElement("locality")]
        [ProtoMember(4)]
        public string Locality { get; set; }

        [XmlElement("postOfficeBoxNumber")]
        [ProtoMember(5)]
        public string PostOfficeBoxNumber { get; set; }

        [XmlElement("postalCode")]
        [ProtoMember(6)]
        public string PostalCode { get; set; }

        [XmlElement("streetAddress")]
        [ProtoMember(7)]
        public string StreetAddress { get; set; }


    }
}
