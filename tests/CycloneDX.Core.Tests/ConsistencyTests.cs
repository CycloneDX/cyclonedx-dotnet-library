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
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX.Json;
using CycloneDX.Models;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Xunit.Abstractions;
using CycloneDX.Core.Models;


namespace CycloneDX.Core.Tests.Consistency.v1_6
{

    public class BinaryProtocResult
    {
        public Stream Output { get; set; }
        public string Errors { get; set; }
        public int ExitCode { get; set; }
    }
    public class ProtocRunner
    {
        internal BinaryProtocResult Run(string workingDirectory, string input, string[] arguments)
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

                    return new BinaryProtocResult
                    {
                        Output = output,
                        Errors = errors.ToString(),
                        ExitCode = -1
                    };
                }

                Task.WaitAll(outputTask, errorTask);

                return new BinaryProtocResult
                {
                    Output = output,
                    Errors = errors.ToString(),
                    ExitCode = p.ExitCode
                };
            }
            catch
            {
                return new BinaryProtocResult
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


    public class ConsistencyTests
    {
        private readonly ITestOutputHelper output;

        public ConsistencyTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("valid-annotation-1.6")]
        [InlineData("valid-assembly-1.6")]
        [InlineData("valid-attestation-1.6")]
        [InlineData("valid-bom-1.6")]
        [InlineData("valid-component-hashes-1.6")]
        [InlineData("valid-component-identifiers-1.6")]
        [InlineData("valid-component-ref-1.6")]
        [InlineData("valid-component-swid-1.6")]
        [InlineData("valid-component-swid-full-1.6")]
        [InlineData("valid-component-types-1.6")]
        [InlineData("valid-compositions-1.6")]
        [InlineData("valid-cryptography-full-1.6")]
        [InlineData("valid-cryptography-implementation-1.6")]
        [InlineData("valid-dependency-1.6")]
        [InlineData("valid-empty-components-1.6")]
        [InlineData("valid-evidence-1.6")]
        [InlineData("valid-external-reference-1.6")]
        [InlineData("valid-formulation-1.6")]
        [InlineData("valid-license-expression-1.6", Skip = "Requires resolution of https://github.com/CycloneDX/specification/issues/515")]
        [InlineData("valid-license-id-1.6")]
        [InlineData("valid-license-licensing-1.6")]
        [InlineData("valid-license-name-1.6")]
        [InlineData("valid-machine-learning-1.6")]
        [InlineData("valid-machine-learning-considerations-env-1.6")]
        [InlineData("valid-metadata-author-1.6")]
        [InlineData("valid-metadata-license-1.6")]
        [InlineData("valid-metadata-lifecycle-1.6")]
        [InlineData("valid-metadata-manufacture-1.6")]
        [InlineData("valid-metadata-manufacturer-1.6")]
        [InlineData("valid-metadata-supplier-1.6")]
        [InlineData("valid-metadata-timestamp-1.6")]
        [InlineData("valid-metadata-tool-1.6")]
        [InlineData("valid-metadata-tool-deprecated-1.6")]
        [InlineData("valid-minimal-viable-1.6")]
        [InlineData("valid-patch-1.6")]
        [InlineData("valid-properties-1.6")]
        [InlineData("valid-release-notes-1.6")]
        [InlineData("valid-saasbom-1.6")]
        [InlineData("valid-service-1.6")]
        [InlineData("valid-service-empty-objects-1.6")]
        //[InlineData("valid-signatures-1.6")]
        [InlineData("valid-standard-1.6")]
        [InlineData("valid-tags-1.6")]
        [InlineData("valid-vulnerability-1.6")]
        public void JsonProtobufConsistencyTest(string filename)
        {
            var jsonResourceFilename = Path.Join("Resources", "v1.6", filename + ".json");
            var jsonBomInput = File.ReadAllText(jsonResourceFilename);

            var bomFromJson = Serializer.Deserialize(jsonBomInput);



            //var jsonBomOutput = Serializer.Serialize(bom);

            //Snapshot.Match(jsonBomOutput, SnapshotNameExtension.Create(filename));

            using (var tempDir = new CycloneDX.Core.Tests.Protobuf.TempDirectoryWithProtoSchemas())
            {
                var protobufResourceFilename = Path.Join("Resources", "v1.6", filename + ".textproto");
                var protobufTextString = File.ReadAllText(protobufResourceFilename);
                //var inputBom = CycloneDX.Json.Serializer.Deserialize(jsonBom);

                //var stream = new MemoryStream();
                //CycloneDX.Protobuf.Serializer.Serialize(inputBom, stream);

                //var protoBom = stream.ToArray();

                var runner = new ProtocRunner();
                var result = runner.Run(tempDir.DirectoryPath, protobufTextString, new string[]
                {
                    "--proto_path=./",
                    "--encode=cyclonedx.v1_6.Bom",
                    "bom-1.6.proto"
                });

                if (result.ExitCode == 0)
                {
                    result.Output.Seek(0, SeekOrigin.Begin);
                    var bomFromProtobuf = CycloneDX.Protobuf.Serializer.Deserialize(result.Output);
                    //var byteString = System.Text.Encoding.Default.GetString((result.Output as MemoryStream).ToArray());

                    // cleanup as protobuf doesn't distinguish between a list that is null and one that is empty
                    if (bomFromJson.Components?.Count == 0)
                    {
                        bomFromJson.Components = null;
                    }
                    if (bomFromProtobuf.Components?.Count == 0)
                    {
                        bomFromProtobuf.Components = null;
                    }
                    try
                    {
                        //Assert.Equal(CycloneDX.Json.Serializer.Serialize(bomFromJson), CycloneDX.Json.Serializer.Serialize(bomFromProtobuf));
                        Assert.Equal(CycloneDX.Protobuf.Serializer.Serialize(bomFromJson), CycloneDX.Protobuf.Serializer.Serialize(bomFromProtobuf));
                    }
                    catch (Exception)
                    {
                        Snapshot.Match(bomFromJson, SnapshotNameExtension.Create(filename + ".json"));
                        Snapshot.Match(bomFromProtobuf, SnapshotNameExtension.Create(filename + ".proto"));
                        throw;
                    }
                    
                    //if (!Snapshot.Equals(bomFromJson, bomFromProtobuf))
                    //{
                    //    Snapshot.Match(bomFromJson, SnapshotNameExtension.Create(filename + ".json"));
                    //    Snapshot.Match(bomFromProtobuf, SnapshotNameExtension.Create(filename + ".proto"));
                    //    Assert.Equal(bomFromJson, bomFromProtobuf);
                    //}
                }
                else
                {
                    //output.WriteLine(result.Output);
                    output.WriteLine(result.Errors);
                    Assert.Equal(0, result.ExitCode);
                }
            }
        }

        [Theory]
        [InlineData("valid-annotation-1.6")]
        [InlineData("valid-assembly-1.6")]
        [InlineData("valid-attestation-1.6")]
        [InlineData("valid-bom-1.6")]
        [InlineData("valid-component-hashes-1.6")]
        [InlineData("valid-component-identifiers-1.6")]
        [InlineData("valid-component-ref-1.6")]
        [InlineData("valid-component-swid-1.6")]
        [InlineData("valid-component-swid-full-1.6")]
        [InlineData("valid-component-types-1.6")]
        [InlineData("valid-compositions-1.6")]
        [InlineData("valid-cryptography-full-1.6")]
        [InlineData("valid-cryptography-implementation-1.6")]
        [InlineData("valid-dependency-1.6")]
        [InlineData("valid-empty-components-1.6")]
        [InlineData("valid-evidence-1.6")]
        [InlineData("valid-external-reference-1.6")]
        [InlineData("valid-formulation-1.6")]
        [InlineData("valid-license-expression-1.6")]
        [InlineData("valid-license-id-1.6")]
        [InlineData("valid-license-licensing-1.6")]
        [InlineData("valid-license-name-1.6")]
        [InlineData("valid-machine-learning-1.6")]
        [InlineData("valid-machine-learning-considerations-env-1.6")]
        [InlineData("valid-metadata-author-1.6")]
        [InlineData("valid-metadata-license-1.6")]
        [InlineData("valid-metadata-lifecycle-1.6")]
        [InlineData("valid-metadata-manufacture-1.6")]
        [InlineData("valid-metadata-manufacturer-1.6")]
        [InlineData("valid-metadata-supplier-1.6")]
        [InlineData("valid-metadata-timestamp-1.6")]
        [InlineData("valid-metadata-tool-1.6")]
        [InlineData("valid-metadata-tool-deprecated-1.6")]
        [InlineData("valid-minimal-viable-1.6")]
        [InlineData("valid-patch-1.6")]
        [InlineData("valid-properties-1.6")]
        [InlineData("valid-release-notes-1.6")]
        [InlineData("valid-saasbom-1.6")]
        [InlineData("valid-service-1.6")]
        [InlineData("valid-service-empty-objects-1.6")]
        //[InlineData("valid-signatures-1.6")]
        [InlineData("valid-standard-1.6")]
        [InlineData("valid-tags-1.6")]
        [InlineData("valid-vulnerability-1.6")]
        public void JsonXmlConsistencyTest(string filename)
        {
            var jsonResourceFilename = Path.Join("Resources", "v1.6", filename + ".json");
            var jsonBomInput = File.ReadAllText(jsonResourceFilename);

            var bomFromJson = Serializer.Deserialize(jsonBomInput);

            var resourceFilename = Path.Join("Resources", "v1.6", filename + ".xml");
            var xmlBomInput = File.ReadAllText(resourceFilename);

            var bomFromXml = CycloneDX.Xml.Serializer.Deserialize(xmlBomInput);

            //cleanup
            if (bomFromJson.Components?.Count == 0)
            {
                bomFromJson.Components = null;
            }
            if (bomFromXml.Components?.Count == 0)
            {
                bomFromXml.Components = null;
            }

            try
            {
                //Assert.Equal(CycloneDX.Xml.Serializer.Serialize(bomFromJson), CycloneDX.Xml.Serializer.Serialize(bomFromXml));
                Assert.Equal(CycloneDX.Protobuf.Serializer.Serialize(bomFromJson), CycloneDX.Protobuf.Serializer.Serialize(bomFromXml));
            }
            catch (Exception)
            {
                Snapshot.Match(bomFromJson, SnapshotNameExtension.Create(filename + ".json"));
                Snapshot.Match(bomFromXml, SnapshotNameExtension.Create(filename + ".xml"));
                throw;
            }
        }

        [Theory]
        [InlineData(typeof(PaddingScheme))]
        [InlineData(typeof(AlgorithmMode))]
        [InlineData(typeof(CertificationLevel))]
        [InlineData(typeof(ImplementationPlatform))]
        [InlineData(typeof(ExecutionEnvironment))]
        [InlineData(typeof(Primitive))]
        [InlineData(typeof(CryptoFunction))]
        [InlineData(typeof(Component.Classification))]
        [InlineData(typeof(Component.ComponentScope))]
        [InlineData(typeof(Composition.AggregateType))]
        [InlineData(typeof(ProtocolType))]
        [InlineData(typeof(AssetType))]
        [InlineData(typeof(RelatedCryptoMaterialType))]
        [InlineData(typeof(KeyState))]
        [InlineData(typeof(Data.DataType))]
        [InlineData(typeof(DataFlowDirection))]
        [InlineData(typeof(ActivityType))]
        [InlineData(typeof(CO2Unit))]
        [InlineData(typeof(EnergySource))]
        [InlineData(typeof(EnergyUnit))]
        [InlineData(typeof(EvidenceIdentity.EvidenceFieldType))]
        [InlineData(typeof(EvidenceMethods.EvidenceTechnique))]
        [InlineData(typeof(ExternalReference.ExternalReferenceType))]
        [InlineData(typeof(Hash.HashAlgorithm))]
        [InlineData(typeof(Issue.IssueClassification))]
        [InlineData(typeof(LicenseAcknowledgementEnumeration))]
        [InlineData(typeof(Licensing.LicenseType))]
        [InlineData(typeof(Lifecycles.LifecyclePhase))]
        [InlineData(typeof(ModelCard.ModelParameterApproachType))]
        [InlineData(typeof(Output.OutputType))]
        [InlineData(typeof(Patch.PatchClassification))]
        [InlineData(typeof(Trigger.TriggerType))]
        [InlineData(typeof(Volume.VolumeMode))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.ImpactAnalysisJustification))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.ImpactAnalysisState))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.Response))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.ScoreMethod))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.Severity))]
        [InlineData(typeof(CycloneDX.Models.Vulnerabilities.Status))]
        [InlineData(typeof(WorkflowTask.TaskType))]
        [InlineData(typeof(Workspace.AccessModeType))]
        public void JsonXmlEnumConsistencyTest(Type type)
        {

            foreach(var value in type.GetEnumValues())
            {
                var jsonValue = CycloneDX.Json.Serializer.Serialize(value, type);
                // strip quotes
                jsonValue = jsonValue.Substring(1, jsonValue.Length - 2);
                jsonValue = jsonValue.Replace("\\u002B", "+");
                if (jsonValue == "null")
                {
                    continue;
                }

                string xmlValue = CycloneDX.Xml.Serializer.Serialize(value, type, SpecificationVersion.v1_6);
                // strip first tag
                xmlValue = xmlValue.Substring(xmlValue.IndexOf(">") + 1);
                // extract content of enum tag
                xmlValue = xmlValue.Substring(xmlValue.IndexOf(">") + 1, xmlValue.LastIndexOf("<") - xmlValue.IndexOf(">") - 1);

                Assert.Equal(xmlValue, jsonValue);
            }

        }

    }

}




