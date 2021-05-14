using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Snapshooter;
using Snapshooter.Xunit;
using Xunit;

namespace CycloneDX.Tests.Protobuf.v1_3.Utils
{
    public sealed class LinuxOnlyForCITheory : TheoryAttribute
    {
        public LinuxOnlyForCITheory() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && IsCI()) {
                Skip = "Skip on non-Linux when running in CI";
            }
        }
        
        private static bool IsCI()
            => Environment.GetEnvironmentVariable("CI") == "true";
    }
    
    public class TempDirectory : IDisposable
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

    public class TempDirectoryWithProtoSchema : TempDirectory
    {
        public TempDirectoryWithProtoSchema()
        {
            var assembly = typeof(CycloneDX.Protobuf.Deserializer).GetTypeInfo().Assembly;
            using (var schemaStream = assembly.GetManifestResourceStream($"CycloneDX.Core.Schemas.bom-1.3.proto"))
            {
                using (var fileStream = File.Create(Path.Join(DirectoryPath, "bom-1.3.proto")))
                {
                    schemaStream.Seek(0, SeekOrigin.Begin);
                    schemaStream.CopyTo(fileStream);
                }
            }
        }
    }

    public class ProtocResult
    {
        public string Output { get; set; }
        public string Errors { get; set; }
        public int ExitCode { get; set; }
    }

    public class ProtocRunner
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
