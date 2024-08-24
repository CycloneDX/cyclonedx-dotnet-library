using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    public class AlgorithmProperties
    {
        #region primitive
        [XmlIgnore]
        public Primitive? Primitive { get; set; }
        [XmlElement("primitive"), JsonIgnore]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Primitive Primitive_XML
        {
            get { return Primitive.Value; }
            set { Primitive = value; }
        }        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePrimitive_XML()
        {
            return Primitive.HasValue;
        }
        #endregion

        [XmlElement("parameterSetIdentifier")]
        public string ParameterSetIdentifier { get; set; }
        [XmlElement("curve")]
        public string Curve { get; set; }
        //[XmlIgnore]
        [XmlElement("executionEnvironment")]
        public ExecutionEnvironment? ExecutionEnvironment { get; set; }
        //[XmlElement("executionEnvironment")]
        //public string ExecutionEnvironment_XML
        //{
        //    get => ExecutionEnvironment?.ToString();
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            ExecutionEnvironment = null;
        //        else
        //            ExecutionEnvironment = (ExecutionEnvironment)Enum.Parse(typeof(ExecutionEnvironment), value);
        //    }
        //}
        [XmlElement("implementationPlatform")]
        public ImplementationPlatform? ImplementationPlatform { get; set; }
        //[XmlElement("implementationPlatform")]
        //public string ImplementationPlatform_XML
        //{
        //    get => ImplementationPlatform?.ToString();
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            ImplementationPlatform = null;
        //        else
        //            ImplementationPlatform = (ImplementationPlatform)Enum.Parse(typeof(ImplementationPlatform), value);
        //    }
        //}
        [XmlElement("certificationLevel")]
        public List<CertificationLevel> CertificationLevel { get; set; }
        [XmlElement("mode")]
        public AlgorithmMode? Mode { get; set; }
        //[XmlElement("mode")]
        //public string Mode_XML
        //{
        //    get => Mode?.ToString();
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            Mode = null;
        //        else
        //            Mode = (AlgorithmMode)Enum.Parse(typeof(AlgorithmMode), value);
        //    }
        //}

        #region Padding
        [XmlIgnore]
        public PaddingScheme? Padding { get; set; }
        [XmlElement("padding"), JsonIgnore]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public PaddingScheme Padding_XML
        {
            get { return Padding.Value; }
            set { Padding = value; }
        }        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePadding_XML() => Padding.HasValue;
        #endregion Padding

        [XmlIgnore]        
        public List<CryptoFunction> CryptoFunctions { get; set; }


        [XmlElement("cryptoFunctions")]
        [JsonIgnore]
        public CryptoFunctionCollection CryptoFunctionSerializable_XML
        {
            get { return new CryptoFunctionCollection { CryptoFunctions = CryptoFunctions }; }
            set { CryptoFunctions = new List<CryptoFunction>(value.CryptoFunctions); }
        }

        [XmlElement("classicalSecurityLevel")]
        public int ClassicalSecurityLevel { get; set; }
        [XmlElement("nistQuantumSecurityLevel")]
        public int NistQuantumSecurityLevel { get; set; }
        

    }

    public class CryptoFunctionCollection
    {
        [XmlElement("cryptoFunction")]
        public List<CryptoFunction> CryptoFunctions { get; set; }
    }


    public enum PaddingScheme
    {       
        Null,
        [XmlEnum("pkcs5")]
        PKCS5,
        [XmlEnum("pkcs7")]
        PKCS7,
        [XmlEnum("pkcs1v15")]
        PKCS1v15,
        [XmlEnum("oaep")]
        OAEP,
        [XmlEnum("raw")]
        Raw,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown


    }

    public enum AlgorithmMode
    {
        Null,
        [XmlEnum("cbc")]
        Cbc,
        [XmlEnum("ecb")]
        Ecb,
        [XmlEnum("ccm")]
        Ccm,
        [XmlEnum("gcm")]
        Gcm,
        [XmlEnum("cfb")]
        Cfb,
        [XmlEnum("ofb")]
        Ofb,
        [XmlEnum("ctr")]
        Ctr,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown
    }

    public enum CertificationLevel
    {
        Null,
        [XmlEnum("none")]
        None,
        [XmlEnum("fips140-1-l1")]
        FIPS140_1_L1,
        [XmlEnum("fips140-1-l2")]
        FIPS140_1_L2,
        [XmlEnum("fips140-1-l3")]
        FIPS140_1_L3,
        [XmlEnum("fips140-1-l4")]
        FIPS140_1_L4,
        [XmlEnum("fips140-2-l1")]
        FIPS140_2_L1,
        [XmlEnum("fips140-2-l2")]
        FIPS140_2_L2,
        [XmlEnum("fips140-2-l3")]
        FIPS140_2_L3,
        [XmlEnum("fips140-2-l4")]
        FIPS140_2_L4,
        [XmlEnum("fips140-3-l1")]
        FIPS140_3_L1,
        [XmlEnum("fips140-3-l2")]
        FIPS140_3_L2,
        [XmlEnum("fips140-3-l3")]
        FIPS140_3_L3,
        [XmlEnum("fips140-3-l4")]
        FIPS140_3_L4,
        [XmlEnum("cc-eal1")]
        CC_EAL1,
        [XmlEnum("cc-eal1+")]
        CC_EAL1plus,
        [XmlEnum("cc-eal2")]
        CC_EAL2,
        [XmlEnum("cc-eal2+")]
        CC_EAL2plus,
        [XmlEnum("cc-eal3")]
        CC_EAL3,
        [XmlEnum("cc-eal3+")]
        CC_EAL3plus,
        [XmlEnum("cc-eal4")]
        CC_EAL4,
        [XmlEnum("cc-eal4+")]
        CC_EAL4plus,
        [XmlEnum("cc-eal5")]
        CC_EAL5,
        [XmlEnum("cc-eal5+")]
        CC_EAL5plus,
        [XmlEnum("cc-eal6")]
        CC_EAL6,
        [XmlEnum("cc-eal6+")]
        CC_EAL6plus,
        [XmlEnum("cc-eal7")]
        CC_EAL7,
        [XmlEnum("cc-eal7+")]
        CC_EAL7plus,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown

    }

    public enum ImplementationPlatform
    {
        Null,
        [XmlEnum("generic")]
        Generic,
        [XmlEnum("x86_32")]
        X86_32,
        [XmlEnum("x86_64")]
        X86_64,
        [XmlEnum("armv7-a")]
        Armv7A,
        [XmlEnum("armv7-m")]
        Armv7M,
        [XmlEnum("armv8-a")]
        Armv8A,
        [XmlEnum("armv8-m")]
        Armv8M,
        [XmlEnum("armv9-a")]
        Armv9A,
        [XmlEnum("armv9-m")]
        Armv9M,
        [XmlEnum("s390x")]
        S390X,
        [XmlEnum("ppc64")]
        Ppc64,
        [XmlEnum("ppc64le")]
        Ppc64le,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown
    }

    public enum ExecutionEnvironment
    {
        Null,
        [XmlEnum("software-plain-ram")]
        Software_Plain_Ram,
        [XmlEnum("software-encrypted-ram")]
        Software_Encypted_Ram,
        [XmlEnum("software-tee")]
        Software_TEE,
        [XmlEnum("hardware")]
        Hardware,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown

    }

    public enum Primitive
    {
        Null,
        [XmlEnum("drbg")]
        DRGB,
        [XmlEnum("mac")]
        MAC,
        [XmlEnum("block-cipher")]
        Block_Cipher,
        [XmlEnum("stream-cipher")]
        Stream_Cipher,
        [XmlEnum("signature")]
        Signature,
        [XmlEnum("hash")]
        Hash,
        [XmlEnum("pke")]
        PKE,
        [XmlEnum("xof")]
        XOF,
        [XmlEnum("kdf")]
        KDF,
        [XmlEnum("key-agree")]
        Key_Agree,
        [XmlEnum("kem")]
        KEM,
        [XmlEnum("ae")]
        AE,
        [XmlEnum("combiner")]
        Combiner,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown

    }

    public enum CryptoFunction
    {
        Null,
        [XmlEnum("generate")]
        Generate,
        [XmlEnum("keygen")]
        Keygen,
        [XmlEnum("encrypt")]
        Encrypt,
        [XmlEnum("decrypt")]
        Decrypt,
        [XmlEnum("digest")]
        Digest,
        [XmlEnum("tag")]
        Tag,
        [XmlEnum("keyderive")]
        KeyDerive,
        [XmlEnum("sign")]
        Sign,
        [XmlEnum("verify")]
        Verify,
        [XmlEnum("encapsulate")]
        Encapsulate,
        [XmlEnum("decapsulate")]
        Decapsulate,
        [XmlEnum("other")]
        Other,
        [XmlEnum("unknown")]
        Unknown
    }
}
