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
using System.Text.RegularExpressions;
using CycloneDX.Models;
using CycloneDX.Models.Vulnerabilities;

namespace CycloneDX
{
    public static class BomUtils
    {
        internal static DateTime? UtcifyDateTime(DateTime? value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value.Value.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            }
            else if (value.Value.Kind == DateTimeKind.Local)
            {
                return value.Value.ToUniversalTime();
            }
            else
            {
                return value;
            }
        }

        internal static Bom GetBomForSerialization(Bom bom)
        {
            if (bom.SpecVersion == SpecificationVersionHelpers.CurrentVersion)
            {
                return bom;
            }
            else
            {
                var downgradedBom = CopyBomAndDowngrade(bom);
                return downgradedBom;
            }
        }

        internal static Bom CopyBomAndDowngrade(Bom bom)
        {
            var bomCopy = bom.Copy();

            // we downgrade stuff starting with lowest spec first
            // this will remove entire classes of things and will save unnecessary processing further down
            if (bomCopy.SpecVersion < SpecificationVersion.v1_1)
            {
                bomCopy.SerialNumber = null;
                bomCopy.ExternalReferences = null;

                EnumerateAllComponents(bomCopy, (component) => {
                    component.BomRef = null;
                    component.Pedigree = null;
                    component.ExternalReferences = null;
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_2)
            {
                bomCopy.Metadata = null;
                bomCopy.Dependencies = null;
                bomCopy.Services = null;

                EnumerateAllComponents(bomCopy, (component) => {
                    component.Author = null;
                    component.MimeType = null;
                    component.Supplier = null;
                    component.Swid = null;

                    if (component.Pedigree != null)
                    {
                        component.Pedigree.Patches = null;
                    }
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_3)
            {
                bomCopy.Compositions = null;
                if (bomCopy.Metadata != null)
                {
                    bomCopy.Metadata.Licenses = null;
                    bomCopy.Metadata.Properties = null;
                }
                EnumerateAllComponents(bomCopy, (component) => {
                    component.Properties = null;
                    component.Evidence = null;
                    if (component.ExternalReferences != null)
                    {
                        foreach (var extRef in component.ExternalReferences)
                        {
                            extRef.Hashes = null;
                        }
                    }
                });
                EnumerateAllServices(bomCopy, (service) => {
                    service.Properties = null;
                    if (service.ExternalReferences != null)
                    {
                        foreach (var extRef in service.ExternalReferences)
                        {
                            extRef.Hashes = null;
                        }
                    }
                });
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_4)
            {
                EnumerateAllComponents(bomCopy, (component) => {
                    component.ReleaseNotes = null;
                    if (component.Version == null)
                    {
                        component.Version = "0.0.0";
                    }
                });
                EnumerateAllServices(bomCopy, (service) => {
                    service.ReleaseNotes = null;
                });
                bomCopy.Vulnerabilities = null;
            }

            if (bomCopy.SpecVersion < SpecificationVersion.v1_5)
            {
                bomCopy.Annotations = null;
                bomCopy.Properties = null;
                bomCopy.Formulation = null;

                if (bomCopy.Metadata != null) bomCopy.Metadata.Lifecycles = null;

                if (bomCopy.Compositions != null)
                {
                    foreach (var composition in bomCopy.Compositions)
                    {
                        composition.BomRef = null;
                        composition.Vulnerabilities = null;
                    }
                }
                
                EnumerateAllToolChoices(bomCopy, (toolchoice) =>
                {
                    toolchoice.Components = null;
                    toolchoice.Services = null;
                });

                EnumerateAllComponents(bomCopy, (component) =>
                {
                    component.ModelCard = null;
                    component.Data = null;
                    if ((int)component.Type > 8) component.Type = Component.Classification.Library;
                });
                
                EnumerateAllServices(bomCopy, (service) =>
                {
                    service.TrustZone = null;
                    if (service.Data != null)
                    {
                        foreach (var data in service.Data)
                        {
                            data.Name = null;
                            data.Description = null;
                            data.Governance = null;
                            data.Source = null;
                            data.Destination = null;
                        }
                    }
                });
                
                EnumerateAllVulnerabilities(bomCopy, (vulnerability) =>
                {
                    vulnerability.Rejected = null;
                    vulnerability.ProofOfConcept = null;
                    vulnerability.Workaround = null;
                    if (vulnerability.Analysis != null)
                    {
                        vulnerability.Analysis.FirstIssued = null;
                        vulnerability.Analysis.LastUpdated = null;
                    }

                    if (vulnerability.Ratings != null)
                    {
                        var i = 0;
                        while (i < vulnerability.Ratings.Count)
                        {
                            if (vulnerability.Ratings[i].Method == ScoreMethod.CVSSV4 ||
                                vulnerability.Ratings[i].Method == ScoreMethod.SSVC)
                            {
                                vulnerability.Ratings.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                });
                
                EnumerateAllEvidence(bomCopy, (evidence) =>
                {
                    evidence.Identity = null;
                    evidence.Occurrences = null;
                    evidence.Callstack = null;
                });
                
                EnumerateAllLicenseChoices(bomCopy, (licenseChoice) =>
                {
                    licenseChoice.BomRef = null;
                });
                
                EnumerateAllLicenses(bomCopy, (license) =>
                {
                    license.BomRef = null;
                    license.Licensing = null;
                    license.Properties = null;
                });

                EnumerateAllOrganizationalEntity(bomCopy, (orgEntity) =>
                {
                    orgEntity.BomRef = null;
                });

                EnumerateAllOrganizationalContact(bomCopy, (orgContact) =>
                {
                    orgContact.BomRef = null;
                });
            }

            // triggers a bunch of stuff, don't remove unless you know what you are doing
            bomCopy.SpecVersion = bomCopy.SpecVersion;

            return bomCopy;
        }

        public static Bom Copy(this Bom bom)
        {
            var protoBom = Protobuf.Serializer.SerializeForDeepCopy(bom);
            var bomCopy = Protobuf.Serializer.Deserialize(protoBom);
            return bomCopy;
        }

        public static void EnqueueMany<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (items != null)
                foreach (var item in items)
                    queue.Enqueue(item);
        }

        public static void EnumerateAllComponents(Bom bom, Action<Component> callback)
        {
            var q = new Queue<Component>();

            q.Enqueue(bom.Metadata?.Component);
            q.EnqueueMany(bom.Metadata?.Tools?.Components);
            q.EnqueueMany(bom.Components);

            while (q.Count > 0)
            {
                var currentComponent = q.Dequeue();
                if (currentComponent != null)
                {
                    callback(currentComponent);
                    
                    q.EnqueueMany(currentComponent.Components);
                    q.EnqueueMany(currentComponent.Pedigree?.Ancestors);
                    q.EnqueueMany(currentComponent.Pedigree?.Descendants);
                    q.EnqueueMany(currentComponent.Pedigree?.Variants);
                }
            }
        }

        public static void EnumerateAllServices(Bom bom, Action<Service> callback)
        {
            var q = new Queue<Service>();
            
            q.EnqueueMany(bom.Metadata?.Tools?.Services);
            q.EnqueueMany(bom.Services);

            while (q.Count > 0)
            {
                var currentService = q.Dequeue();
                if (currentService != null)
                {
                    callback(currentService);

                    q.EnqueueMany(currentService.Services);
                }
            }
        }

        public static void EnumerateAllVulnerabilities(Bom bom, Action<Vulnerability> callback)
        {
            var q = new Queue<Vulnerability>();

            if (bom.Vulnerabilities != null)
            {
                foreach (var vulnerability in bom.Vulnerabilities)
                {
                    q.Enqueue(vulnerability);
                }
            }

            while (q.Count > 0)
            {
                var currentVulnerability = q.Dequeue();
                
                callback(currentVulnerability);
            }
        }
        public static void EnumerateAllEvidence(Bom bom, Action<Evidence> callback)
        {
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Evidence != null) callback(component.Evidence);
            });
        }
        
        public static void EnumerateAllLicenses(Bom bom, Action<License> callback)
        {
            EnumerateAllLicenseChoices(bom, (licenseChoice) =>
            {
                if (licenseChoice.License != null) callback(licenseChoice.License);
            });
        }

        public static void EnumerateAllLicenseChoices(Bom bom, Action<LicenseChoice> callback)
        {
            if (bom.Metadata?.Licenses != null)
            {
                foreach (var license in bom.Metadata.Licenses)
                {
                    callback(license);
                }
                    
            }
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Licenses != null)
                {
                    foreach (var license in component.Licenses)
                    {
                        callback(license);
                    }
                }
            });
            
            EnumerateAllServices(bom, (service) =>
            {
                if (service.Licenses != null)
                {
                    foreach (var license in service.Licenses)
                    {
                        callback(license);
                    }
                }
            });

            EnumerateAllEvidence(bom, (evidence) =>
            {
                if (evidence.Licenses != null)
                {
                    foreach (var license in evidence.Licenses)
                    {
                        callback(license);
                    }
                }
            });
        }

        public static void EnumerateAllOrganizationalEntity(Bom bom, Action<OrganizationalEntity> callback)
        {
            if (bom.Metadata?.Manufacture != null) callback(bom.Metadata.Manufacture);
            if (bom.Metadata?.Supplier != null) callback(bom.Metadata.Supplier);

            if (bom.Annotations != null)
            {
                foreach (var annotation in bom.Annotations)
                {
                    if (annotation.Annotator?.Organization != null)
                        callback(annotation.Annotator.Organization);
                }
                
            }
                
            EnumerateAllVulnerabilities(bom, (vulnerability) =>
            {
                if (vulnerability.Credits?.Organizations != null)
                {
                    foreach (var org in vulnerability.Credits.Organizations) callback(org);
                }
            });
            EnumerateAllComponents(bom, (component) =>
            {
                if (component.Supplier != null) callback(component.Supplier);
            });
            EnumerateAllServices(bom, (service) =>
            {
                if (service.Provider != null) callback(service.Provider);
            });
        }

        public static void EnumerateAllOrganizationalContact(Bom bom, Action<OrganizationalContact> callback)
        {
            EnumerateAllOrganizationalEntity(bom, (orgEntity) =>
            {
                if (orgEntity.Contact != null)
                {
                    foreach (var contact in orgEntity.Contact)
                    {
                        callback(contact);
                    }
                }
            });
            
            EnumerateAllVulnerabilities(bom, (vulnerability) =>
            {
                if (vulnerability.Credits?.Individuals != null)
                {
                    foreach (var individual in vulnerability.Credits.Individuals)
                    {
                        callback(individual);
                    }
                }
            });
        }

        public static void EnumerateAllToolChoices(Bom bom, Action<ToolChoices> callback)
        {
            if (bom.Metadata?.Tools != null)
                callback(bom.Metadata.Tools);
            EnumerateAllVulnerabilities(bom, (vuln) =>
            {
                if (vuln.Tools != null)
                    callback(vuln.Tools);
            });
        }
    }
}