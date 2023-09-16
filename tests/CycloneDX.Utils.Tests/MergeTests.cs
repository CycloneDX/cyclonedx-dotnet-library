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
using Snapshooter;
using Snapshooter.Xunit;
using CycloneDX;
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class MergeTests
    {
        [Fact]
        public void FlatMergeToolsTest()
        {
            #pragma warning disable 618
            var sbom1 = new Bom
            {
                Metadata = new Metadata
                {
                    Tools = new ToolChoices
                    {
                        Tools = new List<Tool>
                        {
                            new Tool
                            {
                                Name = "Tool1",
                                Version = "1"
                            }
                        }
                    }
                }
            };
            var sbom2 = new Bom
            {
                Metadata = new Metadata
                {
                    Tools = new ToolChoices
                    {
                        Tools = new List<Tool>
                        {
                            new Tool
                            {
                                Name = "Tool2",
                                Version = "1"
                            }
                        }
                    }
                }
            };
            #pragma warning restore 618

            var result = CycloneDXUtils.FlatMerge(sbom1, sbom2);

            Snapshot.Match(result);
        }

        [Fact]
        public void FlatMergeComponentsTest()
        {
            var sbom1 = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component1",
                        Version = "1"
                    }
                }
            };
            var sbom2 = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component2",
                        Version = "1"
                    }
                }
            };

            var result = CycloneDXUtils.FlatMerge(sbom1, sbom2);

            Snapshot.Match(result);
        }

        [Fact]
        public void FlatMergeVulnerabilitiesTest()
        {
            var sbom1 = new Bom
            {
                Vulnerabilities = new List<Vulnerability>
                {
                    new Vulnerability
                    {
                        Id = "cve1",
                        Affects = new List<Affects>
                        {
                            new Affects
                            {
                                Ref = "ref1"
                            }
                        }
                    }
                }
            };
            var sbom2 = new Bom
            {
                Vulnerabilities = new List<Vulnerability>
                {
                    new Vulnerability
                    {
                        Id = "cve2",
                        Affects = new List<Affects>
                        {
                            new Affects
                            {
                                Ref = "ref2"
                            }
                        }
                    }
                }
            };

            var result = CycloneDXUtils.FlatMerge(sbom1, sbom2);

            Snapshot.Match(result);
        }

        [Fact]
        public void HierarchicalMergeComponentsTest()
        {
            var subject = new Component
            {
                Name = "Thing",
                Version = "1",
            };

            var sbom1 = new Bom
            {
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Name = "System1",
                        Version = "1",
                        BomRef = "System1@1"
                    }
                },
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component1",
                        Version = "1",
                        BomRef = "Component1@1"
                    }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency
                    {
                        Ref = "System1@1",
                        Dependencies = new List<Dependency>
                        {
                            new Dependency
                            {
                                Ref = "Component1@1"
                            }
                        }
                    }
                },
                Compositions = new List<Composition>
                {
                    new Composition
                    {
                        Aggregate = Composition.AggregateType.Complete,
                        Assemblies = new List<string>
                        {
                            "System1@1"
                        },
                        Dependencies = new List<string>
                        {
                            "System1@1"
                        }
                    }
                }
            };
            var sbom2 = new Bom
            {
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Name = "System2",
                        Version = "1",
                        BomRef = "System2@1"
                    }
                },
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component2",
                        Version = "1",
                        BomRef = "Component2@1"
                    }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency
                    {
                        Ref = "System2@1",
                        Dependencies = new List<Dependency>
                        {
                            new Dependency
                            {
                                Ref = "Component2@1"
                            }
                        }
                    }
                },
                Compositions = new List<Composition>
                {
                    new Composition
                    {
                        Aggregate = Composition.AggregateType.Complete,
                        Assemblies = new List<string>
                        {
                            "System2@1"
                        },
                        Dependencies = new List<string>
                        {
                            "System2@1"
                        }
                    }
                }
            };

            var result = CycloneDXUtils.HierarchicalMerge(new [] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }

        [Fact]
        public void HierarchicalMergeVulnerabilitiesTest()
        {
            var subject = new Component
            {
                Name = "Thing",
                Version = "1",
            };

            var sbom1 = new Bom
            {
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Name = "System1",
                        Version = "1",
                        BomRef = "System1@1"
                    }
                },
                Vulnerabilities = new List<Vulnerability>
                {
                    new Vulnerability
                    {
                        Id = "cve1",
                        Affects = new List<Affects>
                        {
                            new Affects
                            {
                                Ref = "ref1"
                            }
                        }
                    }
                }
            };
            var sbom2 = new Bom
            {
                Metadata = new Metadata
                {
                    Component = new Component
                    {
                        Name = "System2",
                        Version = "1",
                        BomRef = "System2@1"
                    }
                },
                Vulnerabilities = new List<Vulnerability>
                {
                    new Vulnerability
                    {
                        Id = "cve2",
                        Affects = new List<Affects>
                        {
                            new Affects
                            {
                                Ref = "ref2"
                            }
                        }
                    }
                }
            };

            var result = CycloneDXUtils.HierarchicalMerge(new [] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }
    }
}
