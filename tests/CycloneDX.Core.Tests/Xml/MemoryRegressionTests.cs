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

            _ = Serializer.Deserialize(xmlContent);
            ForceGc();
            var pointB = Environment.WorkingSet;
            var peakUntilC = Math.Max(pointA, pointB);

            for (var i = 0; i < 99; i++)
            {
                _ = Serializer.Deserialize(xmlContent);
                peakUntilC = Math.Max(peakUntilC, Environment.WorkingSet);
            }

            ForceGc();
            var pointC = Environment.WorkingSet;
            peakUntilC = Math.Max(peakUntilC, pointC);

            var peakDuringD = pointC;

            for (var i = 0; i < 99 * 80; i++)
            {
                _ = Serializer.Deserialize(xmlContent);
                peakDuringD = Math.Max(peakDuringD, Environment.WorkingSet);
            }

            ForceGc();
            var pointD = Environment.WorkingSet;
            peakDuringD = Math.Max(peakDuringD, pointD);

            var peakDuringE = pointD;
            for (var i = 0; i < 8000; i++)
            {
                _ = Serializer.Deserialize(xmlContent);
                peakDuringE = Math.Max(peakDuringE, Environment.WorkingSet);
            }

            ForceGc();
            var pointE = Environment.WorkingSet;
            peakDuringE = Math.Max(peakDuringE, pointE);

            const long maxGrowthBytes = 150L * 1024 * 1024;
            const long maxPeakGrowthBytes = 250L * 1024 * 1024;
            var growth = pointC - pointB;
            var growthCToD = pointD - pointC;
            var growthDToE = pointE - pointD;
            var peakGrowth = peakDuringD - peakUntilC;
            var peakGrowthDToE = peakDuringE - peakDuringD;

            Console.WriteLine($"Memory points (MB): A={ToMb(pointA)}, B={ToMb(pointB)}, C={ToMb(pointC)}, D={ToMb(pointD)}, E={ToMb(pointE)}, Peak<=C={ToMb(peakUntilC)}, Peak@D={ToMb(peakDuringD)}, Peak@E={ToMb(peakDuringE)}, B-C delta={ToMb(growth)}, C-D delta={ToMb(growthCToD)}, D-E delta={ToMb(growthDToE)}, Peak delta={ToMb(peakGrowth)}, Peak D-E delta={ToMb(peakGrowthDToE)}");

            Assert.True(
                growth <= maxGrowthBytes,
                $"Expected point C to stay close to point B after warmup. A={ToMb(pointA)} MB, B={ToMb(pointB)} MB, C={ToMb(pointC)} MB, growth={ToMb(growth)} MB.");

            Assert.True(
                growthCToD <= maxGrowthBytes,
                $"Expected point D to stay close to point C after additional cycles. C={ToMb(pointC)} MB, D={ToMb(pointD)} MB, growth={ToMb(growthCToD)} MB.");

            Assert.True(
                growthDToE <= maxGrowthBytes,
                $"Expected point E to stay close to point D after additional cycles. D={ToMb(pointD)} MB, E={ToMb(pointE)} MB, growth={ToMb(growthDToE)} MB.");

            Assert.True(
                peakGrowth <= maxPeakGrowthBytes,
                $"Expected peak memory in D phase to stay close to peak observed before D. Peak<=C={ToMb(peakUntilC)} MB, Peak@D={ToMb(peakDuringD)} MB, peak growth={ToMb(peakGrowth)} MB.");

            Assert.True(
                peakGrowthDToE <= maxPeakGrowthBytes,
                $"Expected peak memory in E phase to stay close to peak observed in D phase. Peak@D={ToMb(peakDuringD)} MB, Peak@E={ToMb(peakDuringE)} MB, peak growth={ToMb(peakGrowthDToE)} MB.");
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

        private static long ToMb(long bytes)
        {
            return bytes / 1024 / 1024;
        }
    }
}
