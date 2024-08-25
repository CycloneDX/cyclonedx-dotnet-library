using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    [XmlType("cryptoProperties")]
    [ProtoContract]
    public class CryptoProperties
    {
        [XmlElement("assetType")]
        [ProtoMember(1)]
        public AssetType AssetType { get; set; }
        [XmlElement("algorithmProperties")]
        [ProtoMember(2)]
        public AlgorithmProperties AlgorithmProperties { get; set; }
        [XmlElement("certificateProperties")]
        [ProtoMember(3)]

        public CertificateProperties CertificateProperties { get; set; }
        [XmlElement("relatedCryptoMaterialProperties")]
        [ProtoMember(4)]
        public RelatedCryptoMaterialProperties RelatedCryptoMaterialProperties { get; set; }
        [XmlElement("protocolProperties")]
        [ProtoMember(5)]
        public ProtocolProperties ProtocolProperties { get; set; }
        [XmlElement("oid")]
        [ProtoMember(6)]
        public string ObjectIdentifier { get; set; }
    }

    [ProtoContract]
    public class ProtocolProperties
    {
        [XmlElement("type")]
        [ProtoMember(1)]
        public ProtocolType Type { get; set; }

        [XmlElement("version")]
        [ProtoMember(2)]
        public string Version { get; set; }
        [XmlIgnore]
        [ProtoMember(3)]
        public List<CipherSuite> CipherSuites { get; set; }
        [XmlElement("cipherSuites")]
        [JsonIgnore]
        public CipherSuiteCollection CipherSuites_XML
        {
            get { return new CipherSuiteCollection { CipherSuites = this.CipherSuites }; }
            set { this.CipherSuites = new List<CipherSuite>(value.CipherSuites); }
        }


        [XmlElement("ikev2TransformTypes")]
        [ProtoMember(4)]
        public Ikev2TransformTypes Ikev2TransformTypes { get; set; }
    }

    public enum ProtocolType
    {
    //    Null,
        [XmlEnum("tls")]
        Tls,
        [XmlEnum("ssh")]
        Ssh,
        [XmlEnum("ipsec")]
        Ipsec,
        [XmlEnum("ike")]
        Ike,
        [XmlEnum("sstp")]
        Sstp,
        [XmlEnum("wpa")]
        Wpa,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown
    }

    [ProtoContract]
    public class Ikev2TransformTypes
    {
        [XmlElement("encr")]
        [ProtoMember(1)]
        public List<string> EncryptionAlgorithms { get; set; }

        [XmlElement("prf")]
        [ProtoMember(2)]
        public List<string> PseudorandomFunctions { get; set; }

        [XmlElement("integ")]
        [ProtoMember(3)]
        public List<string> IntegrityAlgorithms { get; set; }

        [XmlElement("ke")]
        [ProtoMember(4)]
        public List<string> KeyExchangeMethods { get; set; }

        [XmlElement("esn")]
        [ProtoMember(5)]
        public List<bool> ExtendedSequenceNumbers { get; set; }

        [XmlElement("auth")]
        [ProtoMember(6)]
        public List<string> AuthenticationMethods { get; set; }
    }


    public class CipherSuiteCollection
    {
        [XmlElement("cipherSuite")]
        public List<CipherSuite> CipherSuites { get; set; }
    }

    [ProtoContract]
    public class CipherSuite
    {
        [XmlElement("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [XmlIgnore]
        [ProtoMember(2)]
        public List<string> Algorithms { get; set; }

        [XmlElement("algorithms")]
        [JsonIgnore]
        public CipherSuiteAlgorithmCollection Algorithms_XML
        {
            get
            {
                return new CipherSuiteAlgorithmCollection { Algorithms = this.Algorithms };
            }
            set 
            {
                this.Algorithms = new List<string>(value.Algorithms);
            }
        }
        [XmlIgnore]
        [ProtoMember(3)]
        public List<string> Identifiers { get; set; }
        [XmlElement("identifiers")]
        [JsonIgnore]
        public CipherSuiteIdentifierCollection Identifiers_XML
        {
            get
            {
                return new CipherSuiteIdentifierCollection { Identifiers = this.Identifiers };
            }
            set
            {
                this.Identifiers = new List<string>(value.Identifiers);
            }
        }
    }

    public class CipherSuiteAlgorithmCollection
    {
        [XmlElement("algorithm")]
        public List<string> Algorithms { get; set; }
    }
    public class CipherSuiteIdentifierCollection
    {
        [XmlElement("identifier")]
        public List<string> Identifiers { get; set; }
    }



    public enum AssetType
    {
   //     Null,
        [XmlEnum("algorithm")]
        Algorithm,
        [XmlEnum("certificate")]
        Certificate,
        [XmlEnum("protocol")]
        Protocol,
        [XmlEnum("related-crypto-material")]
        Related_Crypto_Material,

    }

    [ProtoContract]
    public class RelatedCryptoMaterialProperties
    {
        [XmlElement("type")]
        [ProtoMember(1)]
        public RelatedCryptoMaterialType Type { get; set; }
        [XmlElement("id")]
        [ProtoMember(2)]
        public string Id { get; set; }
        [XmlElement("state")]
        [ProtoMember(3)]
        public KeyState State { get; set; }
        [XmlElement("algorithmRef")]
        [ProtoMember(4)]
        public string AlgorithmRef { get; set; }
        [XmlElement("creationDate")]
        [ProtoMember(5)]
        public DateTime CreationDate { get; set; }
        [XmlElement("activationDate")]
        [ProtoMember(6)]
        public DateTime ActivationDate { get; set; }
        [XmlElement("updateDate")]
        [ProtoMember(7)]
        public DateTime UpdateDate { get; set; }
        [XmlElement("expirationDate")]
        [ProtoMember(8)]
        public DateTime ExpirationDate { get; set; }
        [XmlElement("value")]
        [ProtoMember(9)]
        public string Value { get; set; }
        [XmlElement("size")]
        [ProtoMember(10)]
        public int Size { get; set; }
        [XmlElement("format")]
        [ProtoMember(11)]
        public string Format { get; set; }
        [XmlElement("securedBy")]
        [ProtoMember(12)]
        public SecuredBy SecuredBy { get; set; }

    }

    [ProtoContract]
    public class SecuredBy
    {
        [XmlElement("mechanism")]
        [ProtoMember(1)]

        public string Mechanism { get; set; }
        [XmlElement("algorithmRef")]
        [ProtoMember(2)]

        public string AlgorithmRef { get; set; }


    }


    public enum RelatedCryptoMaterialType
    {
  //      Null,
        [XmlEnum("private-key")]
        Private_Key,
        [XmlEnum("public-key")]
        Public_Key,
        [XmlEnum("secret-key")]
        Secret_Key,
        [XmlEnum("key")]
        Key,
        [XmlEnum("ciphertext")]
        Ciphertext,
        [XmlEnum("signature")]
        Signature,
        [XmlEnum("digest")]
        Digest,
        [XmlEnum("initialization-vector")]
        Initialization_Vector,
        [XmlEnum("nonce")]
        Nonce,
        [XmlEnum("seed")]
        Seed,
        [XmlEnum("salt")]
        Salt,
        [XmlEnum("shared-secret")]
        Shared_Secret,
        [XmlEnum("tag")]
        Tag,
        [XmlEnum("additional-data")]
        Additional_Data,
        [XmlEnum("password")]
        Password,
        [XmlEnum("credential")]
        Credential,
        [XmlEnum("token")]
        Token,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown
    }

    public enum KeyState
    {
        [XmlEnum("pre-activation")]
        Pre_Activation,
        [XmlEnum("active")]
        Active,
        [XmlEnum("suspended")]
        Suspended,
        [XmlEnum("deactivated")]
        Deactivated,
        [XmlEnum("compromised")]
        Compromised,
        [XmlEnum("destroyed")]
        Destroyed
    }

    [ProtoContract]
    public class CertificateProperties
    {
        [XmlElement("subjectName")]
        [ProtoMember(1)]
        public string SubjectName { get; set; }
        [XmlElement("issuerName")]
        [ProtoMember(2)]
        public string IssuerName { get; set; }
        [XmlElement("notValidBefore")]
        [ProtoMember(3)]
        public DateTime NotValidBefore { get; set; }
        [XmlElement("notValidAfter")]
        [ProtoMember(4)]
        public DateTime NotValidAfter { get; set; }
        [XmlElement("signatureAlgorithmRef")]
        [ProtoMember(5)]
        public string SignatureAlgorithmRef { get; set; }
        [XmlElement("subjectPublicKeyRef")]
        [ProtoMember(6)]
        public string SubjectPublicKeyRef { get; set; }
        [XmlElement("certificateFormat")]
        [ProtoMember(7)]
        public string CertificateFormat { get; set; }
        [XmlElement("certificateExtension")]
        [ProtoMember(8)]
        public string CertificateExtension { get; set; }
    }


}
