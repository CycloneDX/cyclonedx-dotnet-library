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
    public class CryptoProperties
    {
        [XmlElement("assetType")]
        public AssetType AssetType { get; set; }
        [XmlElement("algorithmProperties")]
        public AlgorithmProperties AlgorithmProperties { get; set; }
        [XmlElement("certificateProperties")]
        public CertificateProperties CertificateProperties { get; set; }
        [XmlElement("relatedCryptoMaterialProperties")]
        public RelatedCryptoMaterialProperties RelatedCryptoMaterialProperties { get; set; }
        [XmlElement("protocolProperties")]
        public ProtocolProperties ProtocolProperties { get; set; }
        [XmlElement("oid")]
        public string ObjectIdentifier { get; set; }
    }

    public class ProtocolProperties
    {
        [XmlElement("type")]
        public ProtocolType Type { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }
        [XmlIgnore]
        public List<CipherSuite> CipherSuites { get; set; }
        [XmlElement("cipherSuites")]
        public CipherSuiteCollection CipherSuites_XML
        {
            get { return new CipherSuiteCollection { CipherSuites = this.CipherSuites }; }
            set { this.CipherSuites = new List<CipherSuite>(value.CipherSuites); }
        }


        [XmlElement("ikev2TransformTypes")]
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

    public class Ikev2TransformTypes
    {
        [XmlElement("encr")]
        public List<string> EncryptionAlgorithms { get; set; }

        [XmlElement("prf")]
        public List<string> PseudorandomFunctions { get; set; }

        [XmlElement("integ")]
        public List<string> IntegrityAlgorithms { get; set; }

        [XmlElement("ke")]
        public List<string> KeyExchangeMethods { get; set; }

        [XmlElement("esn")]
        public List<bool> ExtendedSequenceNumbers { get; set; }

        [XmlElement("auth")]
        public List<string> AuthenticationMethods { get; set; }
    }


    public class CipherSuiteCollection
    {
        [XmlElement("cipherSuite")]
        public List<CipherSuite> CipherSuites { get; set; }
    }

    public class CipherSuite
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public List<string> Algorithms { get; set; }

        [XmlElement("algorithms")]
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
        public List<string> Identifiers { get; set; }
        [XmlElement("identifiers")]
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
        ReleatedCryptoMaterial,

    }

    public class RelatedCryptoMaterialProperties
    {
        [XmlElement("type")]
        public RelatedCryptoMaterialType Type { get; set; }
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("state")]
        public KeyState State { get; set; }
        [XmlElement("algorithmRef")]
        public string AlgorithmRef { get; set; }
        [XmlElement("creationDate")]
        public DateTime CreationDate { get; set; }
        [XmlElement("activationDate")]
        public DateTime ActivationDate { get; set; }
        [XmlElement("updateDate")]
        public DateTime UpdateDate { get; set; }
        [XmlElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }
        [XmlElement("value")]
        public string Value { get; set; }
        [XmlElement("size")]
        public int Size { get; set; }
        [XmlElement("format")]
        public string Format { get; set; }
        [XmlElement("securedBy")]
        public SecuredBy SecuredBy { get; set; }

    }

    public class SecuredBy
    {
        [XmlElement("mechanism")]
        public string Mechanism { get; set; }
        [XmlElement("algorithmRef")]
        public string AlgorithmRef { get; set; }


    }


    public enum RelatedCryptoMaterialType
    {
  //      Null,
        [XmlEnum("private-key")]
        PrivateKey,
        [XmlEnum("public-key")]
        PublicKey,
        [XmlEnum("secret-key")]
        SecretKey,
        [XmlEnum("key")]
        Key,
        [XmlEnum("ciphertext")]
        Ciphertext,
        [XmlEnum("signature")]
        Signature,
        [XmlEnum("digest")]
        Digest,
        [XmlEnum("initialization-vector")]
        InitializationVector,
        [XmlEnum("nonce")]
        Nonce,
        [XmlEnum("seed")]
        Seed,
        [XmlEnum("salt")]
        Salt,
        [XmlEnum("shared-secret")]
        SharedSecret,
        [XmlEnum("tag")]
        Tag,
        [XmlEnum("additional-data")]
        AdditionalData,
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
        PreActivation,
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

    public class CertificateProperties
    {
        [XmlElement("subjectName")]
        public string SubjectName { get; set; }
        [XmlElement("issuerName")]
        public string IssuerName { get; set; }
        [XmlElement("notValidBefore")]
        public DateTime NotValidBefore { get; set; }
        [XmlElement("notValidAfter")]
        public DateTime NotValidAfter { get; set; }
        [XmlElement("signatureAlgorithmRef")]
        public string SignatureAlgorithmRef { get; set; }
        [XmlElement("subjectPublicKeyRef")]
        public string SubjectPublicKeyRef { get; set; }
        [XmlElement("certificateFormat")]
        public string CertificateFormat { get; set; }
        [XmlElement("certificateExtension")]
        public string CertificateExtension { get; set; }
    }


}
