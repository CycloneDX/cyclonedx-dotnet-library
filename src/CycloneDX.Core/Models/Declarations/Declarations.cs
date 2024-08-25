using CycloneDX.Models;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    public class Declarations
    {
        [XmlArray("assessors")]
        [XmlArrayItem("assessor")]
        public List<Assessor> Assessors { get; set; }

        [XmlArray("attestations")]
        [XmlArrayItem("attestation")]
        public List<Attestation> Attestations { get; set; }

        [XmlArray("claims")]
        [XmlArrayItem("claim")]
        public List<Claim> Claims { get; set; }

        [XmlArray("evidence")]
        [XmlArrayItem("evidence")]
        public List<Evidence> Evidence { get; set; }

        public Targets Targets { get; set; }

        public Affirmation Affirmation { get; set; }
    }

    public class Assessor
    {
        [XmlElement("bom-ref")]
        public string BomRef { get; set; }

        public bool ThirdParty { get; set; }

        public OrganizationalEntity Organization { get; set; }
    }

    public class Attestation
    {
        public string Summary { get; set; }

        [XmlElement("assessor")]
        public string Assessor { get; set; }

        [XmlArray("map")]
        [XmlArrayItem("map")]
        public List<Map> Map { get; set; }
        
    }

    public class Map
    {
        [XmlElement("requirement")]
        public string Requirement { get; set; }

        [XmlArray("claims")]
        [XmlArrayItem("claim")]
        public List<string> Claims { get; set; }

        [XmlArray("counterClaims")]
        [XmlArrayItem("counterClaim")]
        public List<string> CounterClaims { get; set; }

        public Conformance Conformance { get; set; }

        public Confidence Confidence { get; set; }
    }

    public class Conformance
    {
        public double Score { get; set; }

        public string Rationale { get; set; }

        [XmlArray("mitigationStrategies")]
        [XmlArrayItem("mitigationStrategy")]
        public List<string> MitigationStrategies { get; set; }
    }

    public class Confidence
    {
        public double Score { get; set; }

        public string Rationale { get; set; }
    }

    public class Claim
    {
        [XmlElement("bom-ref")]
        public string BomRef { get; set; }

        [XmlElement("target")]
        public string Target { get; set; }

        public string Predicate { get; set; }

        [XmlArray("mitigationStrategies")]
        [XmlArrayItem("mitigationStrategy")]
        public List<string> MitigationStrategies { get; set; }

        public string Reasoning { get; set; }

        [XmlArray("evidence")]
        [XmlArrayItem("evidence")]
        public List<string> Evidence { get; set; }

        [XmlArray("counterEvidence")]
        [XmlArrayItem("counterEvidence")]
        public List<string> CounterEvidence { get; set; }

        [XmlArray("externalReferences")]
        [XmlArrayItem("externalReference")]
        public List<ExternalReference> ExternalReferences { get; set; }     
    }

    public class Evidence
    {
        [XmlElement("bom-ref")]
        public string BomRef { get; set; }

        public string PropertyName { get; set; }

        public string Description { get; set; }

        [XmlArray("data")]
        [XmlArrayItem("data")]
        public List<Data> Data { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public OrganizationalContact Author { get; set; }

        public OrganizationalContact Reviewer { get; set; }
        
    }

    public class Data
    {
        public string Name { get; set; }

        public DataContents Contents { get; set; }

        public DataClassification Classification { get; set; }

        [XmlArray("sensitiveData")]
        [XmlArrayItem("sensitiveDataItem")]
        public List<string> SensitiveData { get; set; }

        public DataGovernance Governance { get; set; }
    }

    public class DataContents
    {
        public AttachedText Attachment { get; set; }

        public string Url { get; set; }
    }

    public class Targets
    {
        [XmlArray("organizations")]
        [XmlArrayItem("organization")]
        public List<OrganizationalEntity> Organizations { get; set; }

        [XmlArray("components")]
        [XmlArrayItem("component")]
        public List<Component> Components { get; set; }

        [XmlArray("services")]
        [XmlArrayItem("service")]
        public List<Service> Services { get; set; }
    }

    public class Affirmation
    {
        public string Statement { get; set; }

        [XmlArray("signatories")]
        [XmlArrayItem("signatory")]
        public List<Signatory> Signatories { get; set; }

    }

    public class Signatory
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public OrganizationalEntity Organization { get; set; }

        public ExternalReference ExternalReference { get; set; }
    }
}
