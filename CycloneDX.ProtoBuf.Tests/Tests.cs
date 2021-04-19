using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Xml;
using CycloneDX.Json;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CycloneDX.ProtoBuf.Tests
{
    public class Tests
    {
        private readonly ITestOutputHelper output;
        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-hashes")]
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-swid")]
        [InlineData("valid-component-swid-full")]
        [InlineData("valid-component-types")]
        [InlineData("valid-dependency")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-id-full")]
        [InlineData("valid-license-name")]
        [InlineData("valid-license-name-full")]
        [InlineData("valid-metadata-author")]
        [InlineData("valid-metadata-manufacture")]
        [InlineData("valid-metadata-supplier")]
        [InlineData("valid-metadata-timestamp")]
        [InlineData("valid-metadata-tool")]
        [InlineData("valid-minimal-viable")]
        [InlineData("valid-patch")]
        [InlineData("valid-service")]
        [InlineData("valid-service-empty-objects")]
        public void ProtoBufRoundTripTest_v1_3(string filename)
        {
            var resourceFilename = Path.Join("Resources", filename + "-1.3.xml");
            var xmlBom = File.ReadAllText(resourceFilename);
            var inputBom = XmlBomDeserializer.Deserialize_v1_3(xmlBom);
            
            var stream = new MemoryStream();
            ProtoBufBomSerializer.Serialize(stream, inputBom);
            var outputBom = ProtoBufBomDeserializer.Deserialize(stream);
            xmlBom = XmlBomSerializer.Serialize(outputBom);

            Snapshot.Match(xmlBom, SnapshotNameExtension.Create(filename));
        }
        
        [Theory]
        [InlineData("bom")]
        [InlineData("valid-component-hashes")]
        [InlineData("valid-component-ref")]
        [InlineData("valid-component-swid")]
        [InlineData("valid-component-swid-full")]
        [InlineData("valid-component-types")]
        [InlineData("valid-dependency")]
        [InlineData("valid-empty-components")]
        [InlineData("valid-license-expression")]
        [InlineData("valid-license-id")]
        [InlineData("valid-license-name")]
        [InlineData("valid-metadata-author")]
        [InlineData("valid-metadata-manufacture")]
        [InlineData("valid-metadata-supplier")]
        [InlineData("valid-metadata-timestamp")]
        [InlineData("valid-metadata-timestamp-with-offset")]
        [InlineData("valid-metadata-tool")]
        [InlineData("valid-minimal-viable")]
        [InlineData("valid-patch")]
        [InlineData("valid-service")]
        [InlineData("valid-service-empty-objects")]
        public void ValidateProtoBufTest_v1_3(string filename)
        {
            var assembly = typeof(CycloneDX.ProtoBuf.ProtoBufBomDeserializer).GetTypeInfo().Assembly;
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.ProtoBuf.Schemas.bom-1.3.proto"))
            using (var tempDir = new TempDirectory())
            {
                using (var fileStream = File.Create(Path.Join(tempDir.DirectoryPath, "bom-1.3.proto")))
                {
                    schemaStream.Seek(0, SeekOrigin.Begin);
                    schemaStream.CopyTo(fileStream);
                }

                var resourceFilename = Path.Join("Resources", filename + "-1.3.json");
                var jsonBom = File.ReadAllText(resourceFilename);
                var inputBom = JsonBomDeserializer.Deserialize_v1_3(jsonBom);
            
                var stream = new MemoryStream();
                ProtoBufBomSerializer.Serialize(stream, inputBom);

                var protoBom = stream.ToArray();
            
                var runner = new ProtocRunner();
                var result = runner.Run(tempDir.DirectoryPath, protoBom, new string[]
                {
                    "--proto_path=./",
                    "--decode=org.cyclonedx.schema.bom._1_3.Bom",
                    "bom-1.3.proto"
                });

                if (result.ExitCode == 0)
                {
                    Snapshot.Match(result.Output, SnapshotNameExtension.Create(filename));
                }
                else
                {
                    output.WriteLine(result.Output);
                    output.WriteLine(result.Errors);
                    Assert.Equal(0, result.ExitCode);
                }
            }
        }
        
        class TempDirectory : IDisposable
        {
            private readonly string tempPath;
            private readonly string tempDirName;

            public TempDirectory()
            {
                tempPath = Path.GetTempPath();
                tempDirName = Path.GetRandomFileName();
                Directory.CreateDirectory(DirectoryPath);
            }

            public void Dispose()
            {
                Directory.Delete(DirectoryPath, true);
            }

            public string DirectoryPath => Path.Join(tempPath, tempDirName);
        }

        internal class ProtocResult
        {
            public string Output { get; set; }
            public string Errors { get; set; }
            public int ExitCode { get; set; }
        }
        
        internal class ProtocRunner
        {
            internal ProtocResult Run(string workingDirectory, byte[] input, string[] arguments)
            {
                var psi = new ProcessStartInfo("protoc", string.Join(" ", arguments))
                {
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
            
                var p = new Process();
                try
                {
                    p.StartInfo = psi;
                    p.Start();

                    var output = new StringBuilder();
                    var errors = new StringBuilder();
                    var outputTask = ConsumeStreamReaderAsync(p.StandardOutput, output);
                    var errorTask = ConsumeStreamReaderAsync(p.StandardError, errors);

                    // have to use the base stream for binary data
                    var stdIn = p.StandardInput.BaseStream;
                    stdIn.Write(input, 0, input.Length);
                    stdIn.Close();

                    var processExited = p.WaitForExit(20000);

                    if (processExited == false)
                    {
                        p.Kill();

                        return new ProtocResult
                        {
                            Output = output.ToString(),
                            Errors = errors.ToString(),
                            ExitCode = -1
                        };
                    }

                    Task.WaitAll(outputTask, errorTask);

                    return new ProtocResult
                    {
                        Output = output.ToString(),
                        Errors = errors.ToString(),
                        ExitCode = p.ExitCode
                    };
                }
                catch
                {
                    return new ProtocResult
                    {
                        Output = "",
                        Errors = "Unable to execute protoc, ensure you have the protobuf compiler installed",
                        ExitCode = -1
                    };
                }
                finally
                {
                    p.Dispose();
                }
            }
        
            private static async Task ConsumeStreamReaderAsync(StreamReader reader, StringBuilder lines)
            {
                await Task.Yield();

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.AppendLine(line);
                }
            }
        }
    }
}
