using System;
using System.IO;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;

namespace CycloneDX.Tests.Xml.v1_0
{
    public class SerializationTests
    {
        [Theory]
        [InlineData("valid-bom-1.0.xml")]
        [InlineData("valid-component-hashes-1.0.xml")]
        public void XmlRoundTripTest(string filename)
        {
            var resourceFilename = Path.Join("Resources", "v1.0", filename);
            var xmlBom = File.ReadAllText(resourceFilename);

            var bom = Deserializer.Deserialize_v1_0(xmlBom);
            xmlBom = Serializer.Serialize(bom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
    }
}
