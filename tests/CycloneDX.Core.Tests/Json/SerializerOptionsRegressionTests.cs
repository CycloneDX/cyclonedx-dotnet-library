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
using System.Collections.Generic;
using CycloneDX.Json;
using CycloneDX.Models;
using Xunit;

namespace CycloneDX.Core.Tests.Json
{
    public class SerializerOptionsRegressionTests
    {
        [Fact]
        public void ToggleUnsafeRelaxedJsonEscapingViaParameterChangesOutput()
        {
            var bom = new Bom
            {
                SpecVersion = SpecificationVersion.v1_7,
                Version = 1,
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Type = Component.Classification.Application,
                        Name = "tool<name>"
                    }
                },
                Components = new List<Component>()
            };

            var escapedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: false);
            var relaxedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: true);

            Assert.Contains("\\u003Cname\\u003E", escapedJson);
            Assert.Contains("<name>", relaxedJson);
        }

        [Fact]
        public void ToggleUnsafeRelaxedJsonEscapingViaGlobalFlagFallback()
        {
            var original = Utils.UseUnsafeRelaxedJsonEscaping;

            try
            {
                var bom = new Bom
                {
                    SpecVersion = SpecificationVersion.v1_7,
                    Version = 1,
                    Metadata = new Metadata
                    {
                        Component = new Component
                        {
                            Type = Component.Classification.Application,
                            Name = "tool<name>"
                        }
                    },
                    Components = new List<Component>()
                };

                Utils.UseUnsafeRelaxedJsonEscaping = false;
                var escapedJson = Serializer.Serialize(bom);

                Utils.UseUnsafeRelaxedJsonEscaping = true;
                var relaxedJson = Serializer.Serialize(bom);

                Assert.Contains("\\u003Cname\\u003E", escapedJson);
                Assert.Contains("<name>", relaxedJson);
            }
            finally
            {
                Utils.UseUnsafeRelaxedJsonEscaping = original;
            }
        }

        [Fact]
        public void ExplicitParameterOverridesGlobalFlag()
        {
            var original = Utils.UseUnsafeRelaxedJsonEscaping;

            try
            {
                var bom = new Bom
                {
                    SpecVersion = SpecificationVersion.v1_7,
                    Version = 1,
                    Metadata = new Metadata
                    {
                        Component = new Component
                        {
                            Type = Component.Classification.Application,
                            Name = "tool<name>"
                        }
                    },
                    Components = new List<Component>()
                };

                // Global says unsafe, but explicit parameter says safe
                Utils.UseUnsafeRelaxedJsonEscaping = true;
                var escapedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: false);
                Assert.Contains("\\u003Cname\\u003E", escapedJson);

                // Global says safe, but explicit parameter says unsafe
                Utils.UseUnsafeRelaxedJsonEscaping = false;
                var relaxedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: true);
                Assert.Contains("<name>", relaxedJson);
            }
            finally
            {
                Utils.UseUnsafeRelaxedJsonEscaping = original;
            }
        }

        [Fact]
        public void DeserializeToolChoicesObjectFormFromMetadata()
        {
            var json = @"{
  ""bomFormat"": ""CycloneDX"",
  ""specVersion"": ""1.7"",
  ""version"": 1,
  ""metadata"": {
    ""tools"": {
      ""components"": [
        {
          ""type"": ""application"",
          ""name"": ""Awesome Tool""
        }
      ],
      ""services"": [
        {
          ""name"": ""Acme Signing Server""
        }
      ]
    }
  }
}";

            var bom = Serializer.Deserialize(json);

            Assert.NotNull(bom.Metadata);
            Assert.NotNull(bom.Metadata.Tools);
            Assert.NotNull(bom.Metadata.Tools.Components);
            Assert.NotNull(bom.Metadata.Tools.Services);
            Assert.Single(bom.Metadata.Tools.Components);
            Assert.Single(bom.Metadata.Tools.Services);
            Assert.Equal("Awesome Tool", bom.Metadata.Tools.Components[0].Name);
            Assert.Equal("Acme Signing Server", bom.Metadata.Tools.Services[0].Name);
        }

        [Fact]
        public void DeserializeEvidenceIdentityObjectForm()
        {
            var json = @"{
  ""bomFormat"": ""CycloneDX"",
  ""specVersion"": ""1.7"",
  ""version"": 1,
  ""components"": [
    {
      ""type"": ""application"",
      ""name"": ""example"",
      ""evidence"": {
        ""identity"": {
          ""field"": ""purl"",
          ""confidence"": 1.0
        }
      }
    }
  ]
}";

            var bom = Serializer.Deserialize(json);

            var identity = Assert.Single(Assert.Single(bom.Components).Evidence.Identity);
            Assert.Equal(EvidenceIdentity.EvidenceFieldType.Purl, identity.Field);
            Assert.Equal(1.0f, identity.Confidence);
        }

        /// <summary>
        /// Builds a rich BOM with special characters spread across the whole
        /// object graph (metadata, components, services, vulnerabilities,
        /// external references) and verifies that the escaping parameter
        /// applies consistently to every nested element — not just the
        /// top-level fields.
        /// </summary>
        [Fact]
        public void EscapingOptionAppliesToEntireBom()
        {
            // Characters that default escaping will encode but relaxed won't
            var marker = "<script>alert('xss')</script>";

            var bom = new Bom
            {
                SpecVersion = SpecificationVersion.v1_7,
                Version = 1,
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Type = Component.Classification.Application,
                        Name = "root-" + marker
                    }
                },
                Components = new List<Component>
                {
                    new Component
                    {
                        Type = Component.Classification.Library,
                        Name = "lib-" + marker,
                        Description = "desc-" + marker,
                        Publisher = "pub-" + marker
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Name = "svc-" + marker
                    }
                },
                ExternalReferences = new List<ExternalReference>
                {
                    new ExternalReference
                    {
                        Type = ExternalReference.ExternalReferenceType.Website,
                        Url = "https://example.com/path?q=" + marker
                    }
                }
            };

            var escapedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: false);
            var relaxedJson = Serializer.Serialize(bom, unsafeRelaxedJsonEscaping: true);

            // In safe mode the angle brackets must be escaped everywhere
            Assert.DoesNotContain(marker, escapedJson);
            // Verify the encoded form is present for several nested locations
            Assert.Contains("root-\\u003Cscript\\u003E", escapedJson);
            Assert.Contains("lib-\\u003Cscript\\u003E", escapedJson);
            Assert.Contains("svc-\\u003Cscript\\u003E", escapedJson);
            Assert.Contains("desc-\\u003Cscript\\u003E", escapedJson);

            // In relaxed mode the raw marker must appear everywhere
            Assert.Contains("root-" + marker, relaxedJson);
            Assert.Contains("lib-" + marker, relaxedJson);
            Assert.Contains("svc-" + marker, relaxedJson);
            Assert.Contains("desc-" + marker, relaxedJson);
            Assert.Contains("q=" + marker, relaxedJson);
        }

        [Fact]
        public void SerializeRepeatedlyStaysWithinAllocationBudget()
        {
            var bom = new Bom
            {
                SpecVersion = SpecificationVersion.v1_7,
                Version = 1,
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Type = Component.Classification.Application,
                        Name = "example"
                    }
                },
                Components = new List<Component>()
            };

            for (var i = 0; i < 10; i++)
            {
                Serializer.Serialize(bom);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var i = 0; i < 2000; i++)
            {
                Serializer.Serialize(bom);
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.True(
                allocated < 5_000_000,
                $"Expected repeated serialization to allocate less than 5MB, actual: {allocated} bytes.");
        }
    }
}
