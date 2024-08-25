using System;
using System.Collections.Generic;
using System.Text;

namespace CycloneDX.Models
{
    public class Signature
    {
        public string Algorithm { get; set; }
        public string KeyId { get; set; }
        public string PublicKey { get; set; }
        public List<string> CertificatePath { get; set; }
        public string Value { get; set; }
    }
}
