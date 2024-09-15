// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the “License”);
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an “AS IS” BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;

namespace CycloneDX.Core.Tests.Protobuf
{
    public class ProtocRunner
    {
        internal ProtocTextResult Run(string workingDirectory, byte[] input, string[] arguments)
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

                    return new ProtocTextResult
                    {
                        Output = output.ToString(),
                        Errors = errors.ToString(),
                        ExitCode = -1
                    };
                }

                Task.WaitAll(outputTask, errorTask);

                return new ProtocTextResult
                {
                    Output = output.ToString(),
                    Errors = errors.ToString(),
                    ExitCode = p.ExitCode
                };
            }
            catch
            {
                return new ProtocTextResult
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

        internal ProtocBinaryResult Run(string workingDirectory, string input, string[] arguments)
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

                var output = new MemoryStream();
                var errors = new StringBuilder();
                var outputTask = ConsumeStreamAsync(p.StandardOutput.BaseStream, output);
                var errorTask = ConsumeStreamReaderAsync(p.StandardError, errors);

                p.StandardInput.Write(input);
                p.StandardInput.Close();

                var processExited = p.WaitForExit(20000);

                if (processExited == false)
                {
                    p.Kill();

                    return new ProtocBinaryResult
                    {
                        Output = output,
                        Errors = errors.ToString(),
                        ExitCode = -1
                    };
                }

                Task.WaitAll(outputTask, errorTask);

                return new ProtocBinaryResult
                {
                    Output = output,
                    Errors = errors.ToString(),
                    ExitCode = p.ExitCode
                };
            }
            catch
            {
                return new ProtocBinaryResult
                {
                    Output = null,
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
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                lines.AppendLine(line);
            }
        }

        private static async Task ConsumeStreamAsync(Stream reader, Stream output)
        {
            await Task.Yield();

            reader.CopyTo(output);
        }
    }
}
