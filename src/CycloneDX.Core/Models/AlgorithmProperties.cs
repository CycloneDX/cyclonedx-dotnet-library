using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CycloneDX.Core.Models
{
    [ProtoContract]
    public class AlgorithmProperties
    {
        #region primitive
        [XmlIgnore]
        [ProtoMember(1)]
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
        [ProtoMember(2)]
        public string ParameterSetIdentifier { get; set; }
        [XmlElement("curve")]
        [ProtoMember(3)]
        public string Curve { get; set; }
        //[XmlIgnore]
        [XmlElement("executionEnvironment")]
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
        [XmlIgnore]
        [JsonIgnore]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<string> CertificationLevel_Protobuf
        {
            get
            {
                if (CertificationLevel == null)
                {
                    return null;
                }
                return CertificationLevel.Select((certificationLevel) =>
                {
                    return CertificationLevelExtensions.CertificationLevelToString(certificationLevel);
                }).ToList();
            }
            set
            {
                if (value == null)
                {
                    CertificationLevel = null;
                    return;
                }
                CertificationLevel = value.Select((certificationLevel) =>
                {
                    return CertificationLevelExtensions.CertificationLevelFromString(certificationLevel);
                }).ToList();
            }
        }

        [ProtoMember(7)]
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
        [ProtoMember(8)]
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

        [XmlArray("cryptoFunctions")]
        [XmlArrayItem("cryptoFunction")]
        [ProtoMember(9)]
        public List<CryptoFunction> CryptoFunctions { get; set; }

        [XmlElement("classicalSecurityLevel")]
        [ProtoMember(10)]
        public int ClassicalSecurityLevel { get; set; }
        [XmlElement("nistQuantumSecurityLevel")]
        [ProtoMember(11)]
        public int NistQuantumSecurityLevel { get; set; }
        

    }

    public enum PaddingScheme
    {       
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
        [XmlEnum("pkcs5")]
        PKCS5,
        [XmlEnum("pkcs7")]
        PKCS7,
        [XmlEnum("pkcs1v15")]
        PKCS1v15,
        [XmlEnum("oaep")]
        OAEP,
        [XmlEnum("raw")]
        Raw
    }

    public enum AlgorithmMode
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
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
        Ctr
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

    public static class CertificationLevelExtensions {
        public static CertificationLevel CertificationLevelFromString(string value)
        {
            switch (value)
            {
                case "none": return CertificationLevel.None;
                case "fips140-1-l1": return CertificationLevel.FIPS140_1_L1;
                case "fips140-1-l2": return CertificationLevel.FIPS140_1_L2;
                case "fips140-1-l3": return CertificationLevel.FIPS140_1_L3;
                case "fips140-1-l4": return CertificationLevel.FIPS140_1_L4;
                case "fips140-2-l1": return CertificationLevel.FIPS140_2_L1;
                case "fips140-2-l2": return CertificationLevel.FIPS140_2_L2;
                case "fips140-2-l3": return CertificationLevel.FIPS140_2_L3;
                case "fips140-2-l4": return CertificationLevel.FIPS140_2_L4;
                case "fips140-3-l1": return CertificationLevel.FIPS140_3_L1;
                case "fips140-3-l2": return CertificationLevel.FIPS140_3_L2;
                case "fips140-3-l3": return CertificationLevel.FIPS140_3_L3;
                case "fips140-3-l4": return CertificationLevel.FIPS140_3_L4;
                case "cc-eal1": return CertificationLevel.CC_EAL1;
                case "cc-eal1+": return CertificationLevel.CC_EAL1plus;
                case "cc-eal2": return CertificationLevel.CC_EAL2;
                case "cc-eal2+": return CertificationLevel.CC_EAL2plus;
                case "cc-eal3": return CertificationLevel.CC_EAL3;
                case "cc-eal3+": return CertificationLevel.CC_EAL3plus;
                case "cc-eal4": return CertificationLevel.CC_EAL4;
                case "cc-eal4+": return CertificationLevel.CC_EAL4plus;
                case "cc-eal5": return CertificationLevel.CC_EAL5;
                case "cc-eal5+": return CertificationLevel.CC_EAL5plus;
                case "cc-eal6": return CertificationLevel.CC_EAL6;
                case "cc-eal6+": return CertificationLevel.CC_EAL6plus;
                case "cc-eal7": return CertificationLevel.CC_EAL7;
                case "cc-eal7+": return CertificationLevel.CC_EAL7plus;
                case "other": return CertificationLevel.Other;
                case "unknown": return CertificationLevel.Unknown;
                default: return CertificationLevel.Null;
            }
        }

        public static string CertificationLevelToString(CertificationLevel level)
        {
            switch (level)
            {
                case CertificationLevel.None: return "none";
                case CertificationLevel.FIPS140_1_L1: return "fips140-1-l1";
                case CertificationLevel.FIPS140_1_L2: return "fips140-1-l2";
                case CertificationLevel.FIPS140_1_L3: return "fips140-1-l3";
                case CertificationLevel.FIPS140_1_L4: return "fips140-1-l4";
                case CertificationLevel.FIPS140_2_L1: return "fips140-2-l1";
                case CertificationLevel.FIPS140_2_L2: return "fips140-2-l2";
                case CertificationLevel.FIPS140_2_L3: return "fips140-2-l3";
                case CertificationLevel.FIPS140_2_L4: return "fips140-2-l4";
                case CertificationLevel.FIPS140_3_L1: return "fips140-3-l1";
                case CertificationLevel.FIPS140_3_L2: return "fips140-3-l2";
                case CertificationLevel.FIPS140_3_L3: return "fips140-3-l3";
                case CertificationLevel.FIPS140_3_L4: return "fips140-3-l4";
                case CertificationLevel.CC_EAL1: return "cc-eal1";
                case CertificationLevel.CC_EAL1plus: return "cc-eal1+";
                case CertificationLevel.CC_EAL2: return "cc-eal2";
                case CertificationLevel.CC_EAL2plus: return "cc-eal2+";
                case CertificationLevel.CC_EAL3: return "cc-eal3";
                case CertificationLevel.CC_EAL3plus: return "cc-eal3+";
                case CertificationLevel.CC_EAL4: return "cc-eal4";
                case CertificationLevel.CC_EAL4plus: return "cc-eal4+";
                case CertificationLevel.CC_EAL5: return "cc-eal5";
                case CertificationLevel.CC_EAL5plus: return "cc-eal5+";
                case CertificationLevel.CC_EAL6: return "cc-eal6";
                case CertificationLevel.CC_EAL6plus: return "cc-eal6+";
                case CertificationLevel.CC_EAL7: return "cc-eal7";
                case CertificationLevel.CC_EAL7plus: return "cc-eal7+";
                case CertificationLevel.Other: return "other";
                case CertificationLevel.Unknown: return "unknown";
                default: return "null";
            }
        }
    }


    public enum ImplementationPlatform
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
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
        Ppc64le
    }

    public enum ExecutionEnvironment
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
        [XmlEnum("software-plain-ram")]
        Software_Plain_Ram,
        [XmlEnum("software-encrypted-ram")]
        Software_Encypted_Ram,
        [XmlEnum("software-tee")]
        Software_TEE,
        [XmlEnum("hardware")]
        Hardware
    }

    public enum Primitive
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
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
        Combiner
    }

    public enum CryptoFunction
    {
        Null,
        [XmlEnum("unknown")]
        Unknown,
        [XmlEnum("other")]
        Other,
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
    }
}
