// This file is part of CycloneDX Library for .NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// SPDX-License-Identifier: Apache-2.0
// Copyright (c) OWASP Foundation. All Rights Reserved.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CycloneDX.Models;
using CycloneDX.Xml;
using Xunit;

namespace CycloneDX.Core.Tests.Xml
{
    public class ThreadSafetyTests
    {
        [Fact]
        public async Task SerializeAndDeserialize_AreThreadSafe_UnderParallelLoad()
        {
            var licenseXml = File.ReadAllText(Path.Join("Resources", "v1.7", "valid-license-expression-with-licensing-1.7.xml"));
            var toolXml = File.ReadAllText(Path.Join("Resources", "v1.7", "valid-metadata-tool-1.7.xml"));
            var serviceXml = File.ReadAllText(Path.Join("Resources", "v1.7", "valid-service-1.7.xml"));
            var errors = new ConcurrentQueue<Exception>();

            const int taskCount = 32;
            const int iterationsPerTask = 200;

            var tasks = new Task[taskCount];
            for (var i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var input = i % 3 == 0 ? licenseXml : i % 3 == 1 ? toolXml : serviceXml;

                    for (var j = 0; j < iterationsPerTask; j++)
                    {
                        try
                        {
                            var bom = Serializer.Deserialize(input);
                            var roundTrip = Serializer.Serialize(bom);
                            var bomAgain = Serializer.Deserialize(roundTrip);

                            Assert.NotNull(bomAgain);
                            Assert.Equal(bom.SpecVersion, bomAgain.SpecVersion);
                        }
                        catch (Exception ex)
                        {
                            errors.Enqueue(ex);
                            break;
                        }
                    }
                });
            }

            await Task.WhenAll(tasks);

            Assert.True(errors.IsEmpty, errors.TryPeek(out var ex) ? ex.ToString() : "Unknown parallel serialization error");
        }

        [Fact]
        public void GetElementSerializer_IsThreadSafe_UnderParallelCalls()
        {
            var method = typeof(Serializer)
                .GetMethod("GetElementSerializer", BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(typeof(Component));

            Assert.NotNull(method);

            var errors = new ConcurrentQueue<Exception>();

            Parallel.For(0, 5000, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
            {
                try
                {
                    var key = "component-" + (i % 1000);
                    var serializer = method.Invoke(null, [SpecificationVersion.v1_7, key]);
                    Assert.NotNull(serializer);
                }
                catch (Exception ex)
                {
                    errors.Enqueue(ex);
                }
            });

            Assert.True(errors.IsEmpty, errors.TryPeek(out var ex) ? ex.ToString() : "Unknown serializer-cache race");
        }
    }
}
