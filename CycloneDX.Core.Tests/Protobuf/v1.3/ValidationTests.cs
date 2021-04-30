using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Snapshooter;
using Snapshooter.Xunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using CycloneDX.Xml;
using System.Text;

namespace CycloneDX.Tests.Protobuf.v1_3 
{
    public class ValidationTests
    {
        private readonly ITestOutputHelper output;
    
        public ValidationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("valid-assembly-1.3.json")]
        [InlineData("valid-bom-1.3.json")]
        [InlineData("valid-component-hashes-1.3.json")]
        [InlineData("valid-component-ref-1.3.json")]
        [InlineData("valid-component-swid-1.3.json")]
        [InlineData("valid-component-swid-full-1.3.json")]
        [InlineData("valid-component-types-1.3.json")]
        [InlineData("valid-compositions-1.3.json")]
        [InlineData("valid-dependency-1.3.json")]
        [InlineData("valid-empty-components-1.3.json")]
        [InlineData("valid-evidence-1.3.json")]
        [InlineData("valid-external-reference-1.3.json")]
        [InlineData("valid-license-expression-1.3.json")]
        [InlineData("valid-license-id-1.3.json")]
        [InlineData("valid-license-name-1.3.json")]
        [InlineData("valid-metadata-author-1.3.json")]
        [InlineData("valid-metadata-license-1.3.json")]
        [InlineData("valid-metadata-manufacture-1.3.json")]
        [InlineData("valid-metadata-supplier-1.3.json")]
        [InlineData("valid-metadata-timestamp-1.3.json")]
        [InlineData("valid-metadata-tool-1.3.json")]
        [InlineData("valid-minimal-viable-1.3.json")]
        [InlineData("valid-patch-1.3.json")]
        [InlineData("valid-properties-1.3.json")]
        [InlineData("valid-service-1.3.json")]
        [InlineData("valid-service-empty-objects-1.3.json")]
        public void ValidProtobufTest(string filename)
        {
            var assembly = typeof(CycloneDX.Protobuf.Deserializer).GetTypeInfo().Assembly;
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-1.3.proto"))
            using (var tempDir = new TempDirectory())
            {
                using (var fileStream = File.Create(Path.Join(tempDir.DirectoryPath, "bom-1.3.proto")))
                {
                    schemaStream.Seek(0, SeekOrigin.Begin);
                    schemaStream.CopyTo(fileStream);
                }

                var resourceFilename = Path.Join("Resources", "v1.3", filename);
                var jsonBom = File.ReadAllText(resourceFilename);
                var inputBom = CycloneDX.Json.Deserializer.Deserialize_v1_3(jsonBom);

                var stream = new MemoryStream();
                CycloneDX.Protobuf.Serializer.Serialize(stream, inputBom);

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
            private readonly string _tempPath;
            private readonly string _tempDirName;

            public TempDirectory()
            {
                _tempPath = Path.GetTempPath();
                _tempDirName = Path.GetRandomFileName();
                Directory.CreateDirectory(DirectoryPath);
            }

            public void Dispose()
            {
                Directory.Delete(DirectoryPath, true);
            }

            public string DirectoryPath => Path.Join(_tempPath, _tempDirName);
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
                var protocFilename = "protoc";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var enviromentPath = Environment.GetEnvironmentVariable("PATH");
                    var paths = enviromentPath.Split(';');
                    foreach (var path in paths)
                    {
                        var filename = Path.Combine(path, "protoc.exe");
                        if (File.Exists(filename))
                        {
                            protocFilename = filename;
                            break;
                        }
                    }
                }

                var psi = new ProcessStartInfo(protocFilename, string.Join(" ", arguments))
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
