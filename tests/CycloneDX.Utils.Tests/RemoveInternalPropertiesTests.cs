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
using System.Collections.Generic;
using Xunit;
using CycloneDX;
using CycloneDX.Models.v1_3;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class RemoveInternalPropertiesTests
    {
        [Fact]
        public void MetadataPropertiesTest()
        {
            var bom = new Bom
            {
                Metadata = new Metadata
                {
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Name = "cdx:test",
                            Value = "test",
                        },
                        new Property
                        {
                            Name = "internal:test",
                            Value = "test",
                        },
                    }
                }
            };

            CycloneDXUtils.RemoveInternalProperties(bom);

            Assert.Collection<Property>(
                bom.Metadata.Properties,
                item =>
                {
                    Assert.Equal("cdx:test", item.Name);
                    Assert.Equal("test", item.Value);
                }
            );
        }

        [Fact]
        public void ComponentPropertiesTest()
        {
            var bom = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Properties = new List<Property>
                        {
                            new Property
                            {
                                Name = "cdx:test",
                                Value = "test",
                            },
                            new Property
                            {
                                Name = "internal:test",
                                Value = "test",
                            },
                        }
                    },
                }
            };

            CycloneDXUtils.RemoveInternalProperties(bom);

            Assert.Collection<Property>(
                bom.Components[0].Properties,
                item =>
                {
                    Assert.Equal("cdx:test", item.Name);
                    Assert.Equal("test", item.Value);
                }
            );
        }


        [Fact]
        public void SubComponentPropertiesTest()
        {
            var bom = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Components = new List<Component>
                        {
                            new Component
                            {
                                Properties = new List<Property>
                                {
                                    new Property
                                    {
                                        Name = "cdx:test",
                                        Value = "test",
                                    },
                                    new Property
                                    {
                                        Name = "internal:test",
                                        Value = "test",
                                    },
                                }
                            },
                        },
                    },
                }
            };

            CycloneDXUtils.RemoveInternalProperties(bom);

            Assert.Collection<Property>(
                bom.Components[0].Components[0].Properties,
                item =>
                {
                    Assert.Equal("cdx:test", item.Name);
                    Assert.Equal("test", item.Value);
                }
            );
        }


        [Fact]
        public void ServicePropertiesTest()
        {
            var bom = new Bom
            {
                Services = new List<Service>
                {
                    new Service
                    {
                        Properties = new List<Property>
                        {
                            new Property
                            {
                                Name = "cdx:test",
                                Value = "test",
                            },
                            new Property
                            {
                                Name = "internal:test",
                                Value = "test",
                            },
                        }
                    },
                }
            };

            CycloneDXUtils.RemoveInternalProperties(bom);

            Assert.Collection<Property>(
                bom.Services[0].Properties,
                item =>
                {
                    Assert.Equal("cdx:test", item.Name);
                    Assert.Equal("test", item.Value);
                }
            );
        }
    }
}
