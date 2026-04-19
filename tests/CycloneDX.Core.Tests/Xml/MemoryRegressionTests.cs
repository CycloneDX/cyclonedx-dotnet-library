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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CycloneDX.Xml;
using Xunit;

namespace CycloneDX.Core.Tests.Xml
{
    [Collection("XmlMemoryRegression")]
    public class MemoryRegressionTests
    {
        // Manual benchmark-style memory probe for issue #438.
        // Kept for ad-hoc long-running validation; remove Skip to use it again.
        [Fact(Skip = "Non-deterministic in shared CI environments; run manually when validating memory growth behavior.")]
        [Trait("Category", "LongRunning")]
        public void Deserialize_MemoryGrowthStabilizes_AfterWarmup()
        {
            var xmlContent = BuildLargeLicenseBom(componentCount: 600);

            ForceGc();
            var pointA = Environment.WorkingSet;

            RunBatch(xmlContent, 1);
            ForceGc();
            var pointB = Environment.WorkingSet;

            RunBatch(xmlContent, 99);
            ForceGc();
            var pointC = Environment.WorkingSet;

            RunBatch(xmlContent, 99 * 80);
            ForceGc();
            var pointD = Environment.WorkingSet;

            const long maxGrowthBytes = 150L * 1024 * 1024;
            var growth = pointC - pointB;
            var growthCToD = pointD - pointC;

            Console.WriteLine($"Memory points (MB): A={ToMegabytes(pointA)}, B={ToMegabytes(pointB)}, C={ToMegabytes(pointC)}, D={ToMegabytes(pointD)}, B-C delta={ToMegabytes(growth)}, C-D delta={ToMegabytes(growthCToD)}");

            Assert.True(
                growth <= maxGrowthBytes,
                $"Expected point C to stay close to point B after warmup. A={ToMegabytes(pointA)} MB, B={ToMegabytes(pointB)} MB, C={ToMegabytes(pointC)} MB, growth={ToMegabytes(growth)} MB.");

            Assert.True(
                growthCToD <= maxGrowthBytes,
                $"Expected point D to stay close to point C after additional cycles. C={ToMegabytes(pointC)} MB, D={ToMegabytes(pointD)} MB, growth={ToMegabytes(growthCToD)} MB.");
        }

        private static void RunBatch(string xmlContent, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                _ = Serializer.Deserialize(xmlContent);
            }
        }

        private static string BuildLargeLicenseBom(int componentCount)
        {
            var resourceFilename = Path.Join("Resources", "v1.7", "valid-license-choice-1.7.xml");
            var xmlBom = File.ReadAllText(resourceFilename);

            var document = XDocument.Parse(xmlBom);
            var root = document.Root;
            var ns = root.Name.Namespace;
            var components = root.Element(ns + "components");
            var template = components.Elements(ns + "component").First();

            components.RemoveNodes();
            for (var i = 0; i < componentCount; i++)
            {
                components.Add(new XElement(template));
            }

            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static void ForceGc()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static long ToMegabytes(long bytes)
        {
            return bytes / 1024 / 1024;
        }
    }
}
