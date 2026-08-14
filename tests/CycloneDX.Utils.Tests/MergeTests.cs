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
using System.IO;
using CycloneDX.Json;

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
        public void FlatMergeDuplicatedComponentsTest()
        {
            var sboms = new List<Bom>();
            for (int i = 0; i < 3; i++)
            {
                var bom = new Bom
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
                sboms.Add(bom);
            }
            var result = CycloneDXUtils.FlatMerge(sboms);

            Assert.Single(result.Components);
        }

        [Fact]
        public void FlatMergeDuplicatedComponentsDependenciesTest()
        {
            var sboms = new List<Bom>();
            var bom1 = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component1",
                        Version = "1",
                        BomRef = "Component1",
                    },
                    new Component
                    {
                        Name = "CommonDependencyComponent",
                        Version = "1",
                        BomRef = "CommonDependencyComponent"
                    },
                    new Component
                    {
                        Name = "OtherDependencyComponent",
                        Version = "1",
                        BomRef = "OtherDependencyComponent"
                    }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency
                    {
                        Ref = "Component1",
                        Dependencies = new List<Dependency>
                        {
                            new Dependency
                            {
                                Ref = "CommonDependencyComponent"
                            },
                            new Dependency
                            {
                                Ref = "OtherDependencyComponent"
                            }
                        }
                    },
                    new Dependency
                    {
                        Ref = "CommonDependencyComponent",
                        Dependencies = new List<Dependency>{}
                    },
                    new Dependency
                    {
                        Ref = "OtherDependencyComponent",
                        Dependencies = new List<Dependency>{}
                    }
                }
            };
            sboms.Add(bom1);
            var bom2 = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Name = "Component2",
                        Version = "1",
                        BomRef = "Component2",
                    },
                    new Component
                    {
                        Name = "CommonDependencyComponent",
                        Version = "1",
                        BomRef = "CommonDependencyComponent"
                    },
                    new Component
                    {
                        Name = "OtherDependencyComponent2",
                        Version = "1",
                        BomRef = "OtherDependencyComponent2"
                    }
                },
                Dependencies = new List<Dependency>
                {
                    new Dependency
                    {
                        Ref = "Component2",
                        Dependencies = new List<Dependency>
                        {
                            new Dependency
                            {
                                Ref = "CommonDependencyComponent"
                            },
                            new Dependency
                            {
                                Ref = "OtherDependencyComponent2"
                            }
                        }
                    },
                    new Dependency
                    {
                        Ref = "CommonDependencyComponent",
                        Dependencies = new List<Dependency>{}
                    },
                    new Dependency
                    {
                        Ref = "OtherDependencyComponent2",
                        Dependencies = new List<Dependency>{}
                    }
                }
            };
            sboms.Add(bom2);
            var result = CycloneDXUtils.FlatMerge(sboms);

            // there are 5 involved components:
            // Component1, Component2, CommonDependencyComponent,
            // OtherDependencyComponent, OtherDependencyComponent2
            Assert.Equal(5, result.Dependencies.Count);
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
        public void FlatMergeAnnotationsTest()
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
                    },
                Annotations = new List<Annotation>
                {
                    new Annotation
                    {
                        BomRef = "annotation1",
                        Subjects = new List<string> { "Component1" },
                        Text = "Annotation Text",
                        Annotator = new AnnotatorChoice
                        {
                            Individual = new OrganizationalContact
                            {
                                BomRef = "individual1",
                                Name = "individual1",
                            }
                        }
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
                    },
                Annotations = new List<Annotation>
                {
                    new Annotation
                    {
                        BomRef = "annotation2",
                        Subjects = new List<string> { "Component2" },
                        Text = "Annotation Text 2",
                        Annotator = new AnnotatorChoice
                        {
                            Organization = new OrganizationalEntity
                            {
                                BomRef = "organization1",
                                Name = "organization1",
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

            var result = CycloneDXUtils.HierarchicalMerge(new[] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }

        [Fact]
        public void HierarchicalMergeToolsComponentsTest()
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
                    },
                    Tools = new ToolChoices
                    {
                        Components = new List<Component>
                        {
                            new Component
                            {
                                Name = "ToolComponent1",
                                Version = "1",
                                BomRef = "ToolComponent1@1",
                            }
                        }
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
                    },
                    Tools = new ToolChoices
                    {
                        Components = new List<Component>
                        {
                            new Component
                            {
                                Name = "ToolComponent2",
                                Version = "1",
                                BomRef = "ToolComponent2@1",
                            }
                        }
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
            };

            var result = CycloneDXUtils.HierarchicalMerge(new[] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }

        [Fact]
        public void HierarchicalMergeDuplicatedToolsComponentsTest()
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
                    },
                    Tools = new ToolChoices
                    {
                        Components = new List<Component>
                        {
                            new Component
                            {
                                Name = "ToolComponent1",
                                Version = "1",
                            }
                        }
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
                    },
                    Tools = new ToolChoices
                    {
                        Components = new List<Component>
                        {
                            new Component
                            {
                                Name = "ToolComponent1",
                                Version = "1",
                            }
                        }
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
            };

            var result = CycloneDXUtils.HierarchicalMerge(new[] { sbom1, sbom2 }, subject);

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

            var result = CycloneDXUtils.HierarchicalMerge(new[] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }

        [Theory]
        [InlineData("valid-attestation-1.6.json")]
        [InlineData("valid-standard-1.6.json")]
        public void HierarchicalMergeTest1_6(string filename)
        {
            var subject = new Component
            {
                Type = Component.Classification.Application,
                Name = "Thing",
                Version = "1",
            };
            var resourceFilename = Path.Join("MergeTests_Resources", filename);
            var jsonString = File.ReadAllText(resourceFilename);

            var bom1 = Serializer.Deserialize(jsonString);
            var bom2 = Serializer.Deserialize(jsonString);

            bom1.Metadata = new Metadata
            {
                Component = new Component
                {
                    Type = Component.Classification.Application,
                    BomRef = "bom1",
                    Name = "bom1name"
                }
            };

            bom2.Metadata = new Metadata
            {
                Component = new Component
                {
                    Type = Component.Classification.Application,
                    BomRef = "bom2",
                    Name = "bom2name"
                }
            };

            var result = CycloneDXUtils.HierarchicalMerge(new[] { bom1, bom2 }, subject);
            result.SpecVersion = SpecificationVersion.v1_6;

            jsonString = Serializer.Serialize(result);


            var validationResult = Validator.Validate(jsonString, SpecificationVersion.v1_6);
            Assert.True(validationResult.Valid, string.Join(Environment.NewLine, validationResult.Messages));

            Snapshot.Match(jsonString, SnapshotNameExtension.Create(filename));
        }

        [Theory]
        [InlineData("valid-attestation-1.6.json")]
        [InlineData("valid-standard-1.6.json")]
        public void FlatMergeTest1_6(string filename)
        {
            var subject = new Component
            {
                Type = Component.Classification.Application,
                Name = "Thing",
                Version = "1",
            };
            var resourceFilename = Path.Join("MergeTests_Resources", filename);
            var jsonString = File.ReadAllText(resourceFilename);

            var bom1 = Serializer.Deserialize(jsonString);
            var bom2 = new Bom
            {
                Components = new List<Component>
                {
                    new Component
                    {
                        Type = Component.Classification.Application,
                        BomRef = "bom2",
                        Name = "bom2name"
                    }
                }
            };


            var result = CycloneDXUtils.FlatMerge(new[] { bom1, bom2 }, subject);
            result.SpecVersion = SpecificationVersion.v1_6;

            jsonString = Serializer.Serialize(result);

            var validationResult = Validator.Validate(jsonString, SpecificationVersion.v1_6);
            Assert.True(validationResult.Valid, string.Join(Environment.NewLine, validationResult.Messages));

            Snapshot.Match(jsonString, SnapshotNameExtension.Create(filename));
        }
      
        [Fact]
        public void HierarchicalMergeAnnotationsTest()
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
                Annotations = new List<Annotation>
                {
                    new Annotation
                    {
                        BomRef = "annotation1",
                        Subjects = new List<string> { "System1@1" },
                        Text = "Annotation Text",
                        Annotator = new AnnotatorChoice
                        {
                            Individual = new OrganizationalContact
                            {
                                BomRef = "individual1",
                                Name = "individual1",
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
                Annotations = new List<Annotation>
                {
                    new Annotation
                    {
                        BomRef = "annotation2",
                        Subjects = new List<string> { "System2@1" },
                        Text = "Annotation Text 2",
                        Annotator = new AnnotatorChoice
                        {
                            Organization = new OrganizationalEntity
                            {
                                BomRef = "organization1",
                                Name = "organization1",
                            }
                        }
                    }
                }
            };

            var result = CycloneDXUtils.HierarchicalMerge(new[] { sbom1, sbom2 }, subject);

            Snapshot.Match(result);
        }

    }
}
